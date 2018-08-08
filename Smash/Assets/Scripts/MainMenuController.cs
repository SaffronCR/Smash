using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Smash
{
	public class MainMenuController : Photon.PunBehaviour
	{
		public GameObject m_buttons = null;

		private bool m_isReady = false;

		private CloudRegionCode m_currentRegion = CloudRegionCode.none;

		public void OnClickEurope()
		{
			m_isReady = false;

			m_currentRegion = CloudRegionCode.eu;

			PhotonNetwork.Disconnect();
		}

		public void OnClickAustralia()
		{
			m_isReady = false;

			m_currentRegion = CloudRegionCode.au;

			PhotonNetwork.Disconnect();
		}

		public void OnClickUSAEast()
		{
			m_isReady = false;

			m_currentRegion = CloudRegionCode.us;

			PhotonNetwork.Disconnect();
		}

		public void OnClickFindRandomMatch()
		{
			PhotonNetwork.JoinRandomRoom();
		}

		public void OnClickCreateMatch()
		{
			PhotonNetwork.CreateRoom(null);
		}

		public override void OnJoinedLobby()
		{
			m_isReady = true;
		}

		public override void OnDisconnectedFromPhoton()
		{
			if (PhotonNetwork.networkingPeer != null && m_currentRegion != CloudRegionCode.none)
				PhotonNetwork.networkingPeer.ConnectToRegionMaster(m_currentRegion);
		}

		private void Update()
		{
			if (m_buttons != null)
			{
				m_buttons.SetActive(m_isReady);
			}
		}
	}
}
