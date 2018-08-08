using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Smash
{
	public class OptionsButton : MonoBehaviour
	{
		public GameObject m_menuCanvas = null;

		private void Start()
		{
			Button btn = GetComponent<Button>();
			btn.onClick.AddListener(OnBtnPressed);
		}

		public void OnBtnPressed()
		{
			if (m_menuCanvas != null)
			{
				m_menuCanvas.SetActive(!m_menuCanvas.activeSelf);
			}
		}
	}
}
