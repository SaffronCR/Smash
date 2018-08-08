using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace Smash
{
	public class Logger : MonoBehaviour
	{
		#region structs

		public struct DataLog
		{
			public int frameCount;

			public float roundTripTime;
		}

		#endregion structs

		#region public variables

		public Material mat;

		public Vector3 m_position = Vector3.zero;

		public int m_totalFrameCount = 100;

		public int m_numberOfDivisions = 2;

		public float m_spaceBetweenDivisions = 50f;

		public float m_highRTT = 100f;

		#endregion public variables

		#region private variables

		private List<DataLog> m_data = new List<DataLog>();

		private PhotonPeer m_peer = null;

		private int m_oldestFrameCount = 0;

		#endregion private variables

		private void Start()
		{
			m_peer = PhotonNetwork.networkingPeer;

			// Adjust to screen width.
			m_totalFrameCount = Screen.width;
		}

		private void Update()
		{
			if (GameManager.m_captureData == true)
			{
				if (Time.frameCount > m_totalFrameCount)
				{
					// Update count.
					m_oldestFrameCount++;
				}

				// Update list with info from this frame.
				if (m_peer != null)
				{
					DataLog data;
					data.frameCount = Time.frameCount;
					data.roundTripTime = m_peer.RoundTripTime;

					m_data.Add(data);
				}

				// Remove oldest data, since it's no longer needed.
				while (m_data.Count > m_totalFrameCount)
				{
					m_data.RemoveAt(0);
				}
			}
		}

		private void GL_DrawLine(Color color, Vector3 start, Vector3 end)
		{
			GL.Color(color);

			GL.Vertex3(start.x / Screen.width, start.y / Screen.height, start.z);
			GL.Vertex3(end.x / Screen.width, end.y / Screen.height, end.z);
		}

		private void OnPostRender()
		{
			if (GameManager.m_captureData == true)
			{
				if (!mat)
				{
					Debug.LogError("Please Assign a material on the inspector");
					return;
				}

				GL.PushMatrix();
				mat.SetPass(0);
				GL.LoadOrtho();

				GL.Begin(GL.LINES);

				//------------------------------------------------------
				// Draw data.

				Vector3 start = m_position;
				Vector3 end = m_position;

				for (int i = 0; i < m_data.Count; i++)
				{
					Color color = m_data[i].roundTripTime >= m_highRTT ? Color.red : Color.green;

					end.y += m_data[i].roundTripTime;

					GL_DrawLine(color, start, end);

					end.y = m_position.y;

					start.x++;
					end.x++;
				}

				//------------------------------------------------------
				// Draw helpers.

				start = end = m_position;
				end.x += m_totalFrameCount;
				GL_DrawLine(Color.white, start, end);

				for (int i = 0; i < m_numberOfDivisions; i++)
				{
					start.y += m_spaceBetweenDivisions;
					end.y += m_spaceBetweenDivisions;

					GL_DrawLine(Color.white, start, end);
				}

				//------------------------------------------------------

				GL.End();

				GL.PopMatrix();
			}
		}
	}
}
