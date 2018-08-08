using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

namespace Smash
{
	public class RTTUpdater : MonoBehaviour
	{
		private Text m_text = null;
		private PhotonPeer m_peer = null;

		private long m_frameCount = 0;
		private int m_fps = 0;
		private float m_dt = 0f;
		private float m_updateRate = 4f;  // 4 updates per sec.

		private void Start()
		{
			m_text = GetComponent<Text>();

			m_peer = PhotonNetwork.networkingPeer;
		}

		private void Update()
		{
			if (m_text != null && m_peer != null)
			{
				// FPS log.
				m_frameCount++;
				m_dt += Time.deltaTime;
				if (m_dt > 1f / m_updateRate)
				{
					m_fps = (int)(m_frameCount / m_dt);
					m_frameCount = 0;
					m_dt -= 1f / m_updateRate;
				}

				m_text.text = "FPS: " + m_fps + " RTT: " + m_peer.RoundTripTime;

				//m_text.text = string.Format("Rtt:{0,4} +/-{1,3}\n", m_peer.RoundTripTime, m_peer.RoundTripTimeVariance);

				// Traffic log.
				m_peer.TrafficStatsEnabled = GameManager.m_showDebugInfo;
				if (GameManager.m_showDebugInfo == true)
				{
					TrafficStatsGameLevel gls = m_peer.TrafficStatsGameLevel;
					if (gls != null)
					{
						long elapsedMs = m_peer.TrafficStatsElapsedMs / 1000;
						if (elapsedMs == 0)
						{
							elapsedMs = 1;
						}

						m_text.text += "\n";
						m_text.text += string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount, gls.TotalIncomingMessageCount, gls.TotalMessageCount);
						m_text.text += "\n";
						m_text.text += string.Format("{0}sec average:", elapsedMs);
						m_text.text += "\n";
						m_text.text += string.Format("Out {0,4} | In {1,4} | Sum {2,4}", gls.TotalOutgoingMessageCount / elapsedMs, gls.TotalIncomingMessageCount / elapsedMs, gls.TotalMessageCount / elapsedMs);

						//if (m_trafficStatsOn)
						{
							m_text.text += "\n";
							m_text.text += "Traffic Stats \n";
							m_text.text += "\n";
							m_text.text += "Incoming: \n" + m_peer.TrafficStatsIncoming.ToString();
							m_text.text += "\n";
							m_text.text += "\n";
							m_text.text += "Outgoing: \n" + m_peer.TrafficStatsOutgoing.ToString();
						}

						//if (m_healthStatsVisible)
						{
							m_text.text += "\n";
							m_text.text += "\n";
							m_text.text += "Health Stats\n";
							m_text.text += string.Format(
								"ping: {6}[+/-{7}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms",
								gls.LongestDeltaBetweenSending,
								gls.LongestDeltaBetweenDispatching,
								gls.LongestEventCallback,
								gls.LongestEventCallbackCode,
								gls.LongestOpResponseCallback,
								gls.LongestOpResponseCallbackOpCode,
								m_peer.RoundTripTime,
								m_peer.RoundTripTimeVariance,
								m_peer.ResentReliableCommands);
						}
					}
				}
			}
		}
	}
}
