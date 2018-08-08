using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Smash
{
	[RequireComponent(typeof(PhotonView))]
	public class PlayerController : Photon.MonoBehaviour
	{
		#region structs

		public struct DataInput
		{
			public int frameCount;

			public bool moveLeft;
			public bool moveRight;
			public bool jump;

			public void Clear()
			{
				moveLeft = false;
				moveRight = false;
				jump = false;
			}
		}

		#endregion structs

		#region public variables

		[Header("Network")]
		public float m_positionMismatchThreshold = 0f;

		[Header("Player")]
		public float m_jumpSpeed = 0f;

		public float m_jumpTime = 0f;
		public float m_jumpMomentum = 0f;

		public float m_gravity = 9.8f;

		#endregion public variables

		#region private variables

		private List<DataInput> m_input = new List<DataInput>();

		private GameButton m_leftBtn = null;
		private GameButton m_rightBtn = null;
		private GameButton m_jumpBtn = null;

		private Transform m_transform = null;
		private CharacterController m_charCtrl = null;

		private Vector3 m_velocity = Vector3.zero;
		private Vector3 m_networkVelocity = Vector3.zero;
		private Vector3 m_spawnPosition = Vector3.zero;
		private Vector3 m_replicatedPosition = Vector3.zero;

		private float m_currentJumpTime = 0f;
		private float m_currentJumpMomentum = 0f;
		private bool m_isJumping = false;

		private bool m_moveLeft = false;
		private bool m_moveRight = false;

		private bool m_extrapolationActive = false;

		private Vector3 m_positionAtLastPacket = Vector3.zero;
		private float m_currentInterpTime = 0f;
		private float m_packetDifferenceTime = 0f;
		private double m_lastPacketTime = 0f;

		#endregion private variables

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (m_transform != null)
			{
				if (stream.isWriting)
				{
					// We own this player: send the others our data.
					stream.SendNext(m_transform.position);
					stream.SendNext(m_networkVelocity);
				}
				else
				{
					// Network player, receive data.
					m_replicatedPosition = (Vector3)stream.ReceiveNext();
					m_networkVelocity = (Vector3)stream.ReceiveNext();

					// We have new data to interpolate, so switch off extrapolation.
					m_extrapolationActive = false;

					// Update previous position.
					m_positionAtLastPacket = m_transform.position;

					// Update timestamps.
					m_packetDifferenceTime = (float)(info.timestamp - m_lastPacketTime);
					m_lastPacketTime = info.timestamp;
					m_currentInterpTime = 0f;
				}
			}
		}

		private void Awake()
		{
			m_transform = GetComponent<Transform>();
			m_charCtrl = GetComponent<CharacterController>();

			m_spawnPosition = m_transform.position;

			m_leftBtn = GameObject.Find("Left Button").GetComponent<GameButton>();
			m_rightBtn = GameObject.Find("Right Button").GetComponent<GameButton>();
			m_jumpBtn = GameObject.Find("Jump Button").GetComponent<GameButton>();
		}

		private void Update()
		{
			// Update for local player.
			if (photonView.isMine == true)
			{
				UpdateInput();

				UpdateLocalMovement();

				CheckFall();
			}
			else // Update for remote player.
			{
				UpdateRemoteMovement();
			}
		}

		private void UpdateInput()
		{
			DataInput input;

			input.moveLeft = (m_leftBtn.MouseDown == true || Input.GetKey(KeyCode.LeftArrow) == true);
			input.moveRight = (m_rightBtn.MouseDown == true || Input.GetKey(KeyCode.RightArrow) == true);
			input.jump = (m_jumpBtn.MouseDown == true || Input.GetKey(KeyCode.Space) == true);

			if (GameManager.m_inputLagEnabled == true)
			{
				input.frameCount = Time.frameCount + GameManager.m_inputLagFrameCount;
				m_input.Add(input);

				while (m_input.Count > 0)
				{
					if (m_input[0].frameCount < Time.frameCount)
					{
						// This input is old, remove it.
						m_input.RemoveAt(0);
					}
					else if (m_input[0].frameCount == Time.frameCount)
					{
						// Get current input for this frame.
						input = m_input[0];
						m_input.RemoveAt(0);
						break;
					}
					else
					{
						// No input for this frame.
						return;
					}
				}
			}

			// Reset input.
			m_moveLeft = false;
			m_moveRight = false;

			// Check left/right movement.
			if (input.moveLeft == true)
			{
				m_moveLeft = true;
			}
			else if (input.moveRight == true)
			{
				m_moveRight = true;
			}

			// Check jump.
			if (input.jump == true
				&&
				(m_charCtrl != null && m_charCtrl.isGrounded == true)) // You can only start jump while being on the ground.
			{
				m_isJumping = true;
				m_currentJumpTime = 0f;
				m_currentJumpMomentum = 0f;
			}
		}

		private void UpdateLocalMovement()
		{
			if (m_charCtrl != null)
			{
				// Add movement.
				if (m_moveLeft == true)
				{
					m_velocity.x += -GameManager.m_playerSpeed;
				}
				else if (m_moveRight == true)
				{
					m_velocity.x += GameManager.m_playerSpeed;
				}

				// Add jump.
				if (m_isJumping == true)
				{
					m_currentJumpTime += Time.deltaTime;
					if (m_currentJumpTime >= m_jumpTime)
					{
						m_currentJumpMomentum += Time.deltaTime;
						if (m_currentJumpMomentum >= m_jumpMomentum)
						{
							m_isJumping = false;
						}

						// Jump momentum.
						m_velocity.y += m_gravity;
					}
					else
					{
						// Jump velocity.
						m_velocity.y += m_jumpSpeed;
					}
				}

				// Set current velocity for remote clients.
				m_networkVelocity = m_velocity;

				// Add gravity.
				m_velocity.y -= m_gravity;

				// Hot fix: Make sure the controller doesn't move in the Z axis.
				m_velocity.z = (0f - m_transform.position.z);

				// Move player.
				m_charCtrl.Move(m_velocity * Time.deltaTime);

				// Reset velocity.
				m_velocity = Vector3.zero;
			}
		}

		private void CheckFall()
		{
			if (m_transform.position.y < 0f)
			{
				m_transform.position = m_spawnPosition;
			}
		}

		private void UpdateRemoteMovement()
		{
			// Fix position mismatch.
			if (m_extrapolationActive == false &&
				Vector3.Distance(m_replicatedPosition, m_transform.position) > m_positionMismatchThreshold)
			{
				Debug.Log("Player position mismatch detected");

				m_transform.position = m_replicatedPosition;
			}

			// Velocity vector.
			Vector3 velocity = Vector3.zero;

			// Case 1: Calculate extrapolation.
			if (GameManager.m_extrapolationEnabled == true && m_currentInterpTime >= m_packetDifferenceTime)
			{
				// Switch on extrapolation.
				m_extrapolationActive = true;

				// Add last replicated velocity.
				velocity += m_networkVelocity;

				// Add gravity.
				velocity.y -= m_gravity;

				// Add delta time.
				velocity *= Time.deltaTime;

				// Hot fix: Make sure the controller doesn't move in the Z axis
				velocity.z = (0f - m_transform.position.z);

				// Move Character.
				m_charCtrl.Move(velocity);
			}
			else
			{
				// Switch off extrapolation.
				m_extrapolationActive = false;

				// Case 2: Calculate interpolation.
				if (GameManager.m_interpolationEnabled == true)
				{
					m_currentInterpTime = Mathf.Clamp(m_currentInterpTime + Time.deltaTime, 0f, m_packetDifferenceTime);

					m_transform.position = Vector3.Lerp(m_positionAtLastPacket, m_replicatedPosition, m_currentInterpTime / m_packetDifferenceTime);
				}
				else
				{
					// Case 3: Just use last replicated position.
					m_transform.position = m_replicatedPosition;
				}
			}
		}
	}
}
