using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Smash
{
	public class CameraButton : MonoBehaviour
	{
		private void Start()
		{
			Button btn = GetComponent<Button>();
			btn.onClick.AddListener(OnBtnPressed);
		}

		public void OnBtnPressed()
		{
			if (GameManager.m_cameraType == CameraType.Default)
				GameManager.m_cameraType = CameraType.ZoomIn1;
			else if (GameManager.m_cameraType == CameraType.ZoomIn1)
				GameManager.m_cameraType = CameraType.ZoomIn2;
			else if (GameManager.m_cameraType == CameraType.ZoomIn2)
				GameManager.m_cameraType = CameraType.ZoomOut1;
			else
				GameManager.m_cameraType = CameraType.Default;
		}
	}
}
