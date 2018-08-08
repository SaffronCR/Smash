using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smash
{
	public class FollowCamera : MonoBehaviour
	{
		public float m_speed = 0f;

		private Transform m_transform = null;
		private Transform m_target = null;

		private Vector3 m_originalPos = Vector3.zero;
		private float m_currentDistOffset = 0f;
		private float m_currentHeightOffset = 0f;

		public void SetTarget(Transform target)
		{
			m_target = target;
		}

		private void Start()
		{
			m_transform = GetComponent<Transform>();

			m_originalPos = m_transform.position;
		}

		private void LateUpdate()
		{
			if (m_target != null)
			{
				Vector3 newPos = m_transform.position;

				newPos.x = m_target.position.x;

				if (GameManager.m_cameraType == CameraType.ZoomIn1)
				{
					m_currentDistOffset = 10f;
					m_currentHeightOffset = 4f;
				}
				else if (GameManager.m_cameraType == CameraType.ZoomIn2)
				{
					m_currentDistOffset = 15f;
					m_currentHeightOffset = 2f;
				}
				else if (GameManager.m_cameraType == CameraType.ZoomOut1)
				{
					m_currentDistOffset = -10f;
					m_currentHeightOffset = 0f;
				}
				else
				{
					m_currentDistOffset = 0f;
					m_currentHeightOffset = 0f;
				}

				newPos.z = m_originalPos.z + m_currentDistOffset;

				if (GameManager.m_cameraType == CameraType.ZoomIn1 ||
					GameManager.m_cameraType == CameraType.ZoomIn2)
				{
					newPos.y = m_target.position.y + m_currentHeightOffset;
				}

				m_transform.position = Vector3.Lerp(m_transform.position, newPos, m_speed * Time.deltaTime);
			}
		}
	}
}
