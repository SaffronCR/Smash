using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;// Required when using Event data.

namespace Smash
{
	public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler // required interface when using the OnPointerDown method.
	{
		private bool m_mouseDown;
		private float m_timeMouseDown;

		public bool MouseDown
		{
			get
			{
				return m_mouseDown;
			}

			set
			{
				m_mouseDown = value;
			}
		}

		public float TimeMouseDown
		{
			get
			{
				return m_timeMouseDown;
			}

			set
			{
				m_timeMouseDown = value;
			}
		}

		private void Update()
		{
			if (m_mouseDown == true)
			{
				m_timeMouseDown += Time.deltaTime;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			m_mouseDown = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			m_mouseDown = false;
			m_timeMouseDown = 0f;
		}
	}
}
