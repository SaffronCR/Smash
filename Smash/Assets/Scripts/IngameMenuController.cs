using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

namespace Smash
{
	public class IngameMenuController : MonoBehaviour
	{
		public Text m_lagText = null;
		public Text m_jitText = null;
		public Text m_lossText = null;

		public Text m_interpText = null;
		public Text m_extrapText = null;
		public Text m_simulationEnabledText = null;

		public Text m_inputLagText = null;

		public Text m_enableInputLagText = null;

		public Text m_playerSpeedText = null;

		private const int m_lagMax = 500;
		private const int m_lagIncrease = 5;

		private const int m_jitMax = 100;
		private const int m_jitIncrease = 1;

		private const int m_lossMax = 10;
		private const int m_lossIncrease = 1;

		private int m_lag = 0;
		private int m_jit = 0;
		private int m_loss = 0;

		private bool m_simulationEnabled = false;

		private PhotonPeer m_peer = null;

		public void OnClickEnable()
		{
			m_simulationEnabled = !m_simulationEnabled;
			UpdateSettings();
		}

		public void OnClickDebugInfo()
		{
			GameManager.m_showDebugInfo = !GameManager.m_showDebugInfo;
		}

		public void OnClickReduceLag()
		{
			m_lag = Mathf.Clamp(m_lag - m_lagIncrease, 0, m_lagMax);
			UpdateSettings();
		}

		public void OnClickIncreaseLag()
		{
			m_lag = Mathf.Clamp(m_lag + m_lagIncrease, 0, m_lagMax);
			UpdateSettings();
		}

		public void OnClickReduceJit()
		{
			m_jit = Mathf.Clamp(m_jit - m_jitIncrease, 0, m_jitMax);
			UpdateSettings();
		}

		public void OnClickIncreaseJit()
		{
			m_jit = Mathf.Clamp(m_jit + m_jitIncrease, 0, m_jitMax);
			UpdateSettings();
		}

		public void OnClickReduceLoss()
		{
			m_loss = Mathf.Clamp(m_loss - m_lossIncrease, 0, m_lossMax);
			UpdateSettings();
		}

		public void OnClickIncreaseLoss()
		{
			m_loss = Mathf.Clamp(m_loss + m_lossIncrease, 0, m_lossMax);
			UpdateSettings();
		}

		public void OnClickInterpolation()
		{
			GameManager.m_interpolationEnabled = !GameManager.m_interpolationEnabled;
		}

		public void OnClickExtrapolation()
		{
			GameManager.m_extrapolationEnabled = !GameManager.m_extrapolationEnabled;
		}

		public void OnClickExitGame()
		{
			PhotonNetwork.LeaveRoom();

			PhotonNetwork.LoadLevel((int)GameLevels.MainMenu);
		}

		public void OnClickLogger()
		{
			GameManager.m_captureData = !GameManager.m_captureData;
		}

		public void OnClickInputLag()
		{
			GameManager.m_inputLagEnabled = !GameManager.m_inputLagEnabled;
		}

		public void OnClickReduceInputLag()
		{
			GameManager.m_inputLagFrameCount = Mathf.Clamp(GameManager.m_inputLagFrameCount - 1, 0, 100);
			UpdateSettings();
		}

		public void OnClickIncreaseInputLag()
		{
			GameManager.m_inputLagFrameCount = Mathf.Clamp(GameManager.m_inputLagFrameCount + 1, 0, 100);
			UpdateSettings();
		}

		public void OnClickReducePlayerSpeed()
		{
			GameManager.m_playerSpeed = Mathf.Clamp(GameManager.m_playerSpeed - 1f, 0, 100f);
		}

		public void OnClickIncreasePlayerSpeed()
		{
			GameManager.m_playerSpeed = Mathf.Clamp(GameManager.m_playerSpeed + 1f, 0, 100f);
		}

		private void UpdateSettings()
		{
			if (m_peer != null)
			{
				m_peer.IsSimulationEnabled = m_simulationEnabled;

				m_peer.NetworkSimulationSettings.IncomingLag = m_lag;
				m_peer.NetworkSimulationSettings.OutgoingLag = m_lag;

				m_peer.NetworkSimulationSettings.IncomingJitter = m_jit;
				m_peer.NetworkSimulationSettings.OutgoingJitter = m_jit;

				m_peer.NetworkSimulationSettings.IncomingLossPercentage = m_loss;
				m_peer.NetworkSimulationSettings.OutgoingLossPercentage = m_loss;
			}
		}

		private void Start()
		{
			m_peer = PhotonNetwork.networkingPeer;
		}

		private void Update()
		{
			m_lagText.text = "LAG: " + m_peer.NetworkSimulationSettings.IncomingLag;
			m_jitText.text = "JIT: " + m_peer.NetworkSimulationSettings.IncomingJitter;
			m_lossText.text = "LOSS: " + m_peer.NetworkSimulationSettings.IncomingLossPercentage;

			if (GameManager.m_interpolationEnabled) m_interpText.text = "Disable Interpolation";
			else m_interpText.text = "Enable Interpolation";

			if (GameManager.m_extrapolationEnabled) m_extrapText.text = "Disable Extrapolation";
			else m_extrapText.text = "Enable Extrapolation";

			if (m_simulationEnabled) m_simulationEnabledText.text = "Disable Simulation";
			else m_simulationEnabledText.text = "Enable Simulation";

			m_inputLagText.text = "INPUT LAG: " + GameManager.m_inputLagFrameCount;
			if (GameManager.m_inputLagEnabled) m_enableInputLagText.text = "Disable Input Lag";
			else m_enableInputLagText.text = "Enable Input Lag";

			m_playerSpeedText.text = "PLAYER SPEED: " + GameManager.m_playerSpeed;
		}

		private void OnEnable()
		{
			if (m_peer != null)
			{
				m_lag = m_peer.NetworkSimulationSettings.IncomingLag;
				m_jit = m_peer.NetworkSimulationSettings.IncomingJitter;
				m_loss = m_peer.NetworkSimulationSettings.IncomingLossPercentage;
				UpdateSettings();
			}
		}
	}
}
