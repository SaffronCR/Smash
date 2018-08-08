using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smash
{
	public enum GameLevels
	{
		MainMenu = 0,
		Game = 1,
	}

	public enum CameraType
	{
		None,
		Default,
		ZoomIn1,
		ZoomIn2,
		ZoomOut1,
	}

	public class GameManager : Photon.PunBehaviour
	{
		public static bool m_interpolationEnabled = true;
		public static bool m_extrapolationEnabled = true;

		public static bool m_showDebugInfo = false;

		public static bool m_captureData = false;

		public static bool m_inputLagEnabled = false;
		public static int m_inputLagFrameCount = 6;

		public static float m_playerSpeed = 8f;

		public static CameraType m_cameraType = CameraType.Default;

		private FollowCamera m_camera = null;
		private GameObject[] m_spawnpoints = null;

		private void Awake()
		{
			// Dear Unity: Don't destroy my manager, thanks.
			DontDestroyOnLoad(this);

			// Turn off v-sync.
			QualitySettings.vSyncCount = 0;

			// Set framerate.
			Application.targetFrameRate = 60;

			// Never sleep.
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		private void Start()
		{
			// This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically.
			PhotonNetwork.automaticallySyncScene = true;

			// Init multiplayer.
			PhotonNetwork.ConnectUsingSettings("0.1");
		}

		public override void OnJoinedLobby()
		{
			//PhotonNetwork.JoinRandomRoom();
		}

		private void OnPhotonRandomJoinFailed()
		{
			PhotonNetwork.CreateRoom(null);
		}

		public override void OnJoinedRoom()
		{
			if (PhotonNetwork.isMasterClient == true)
			{
				// Load Game Level.
				PhotonNetwork.LoadLevel((int)GameLevels.Game);
			}
		}

		private void OnLevelWasLoaded(int level)
		{
			if (level == (int)GameLevels.Game)
			{
				// Spawn local player.
				SpawnLocalPlayer();
			}
		}

		private void SpawnLocalPlayer()
		{
			// Get spawn point.
			m_spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
			if (m_spawnpoints.Length > 0)
			{
				// Instantiate player over the network.
				GameObject player = PhotonNetwork.Instantiate("Player", m_spawnpoints[Random.Range(0, m_spawnpoints.Length)].transform.position, Quaternion.identity, 0);

				// Set camera target.
				if (player != null)
				{
					if (m_camera == null)
					{
						GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
						if (camera != null)
						{
							m_camera = camera.GetComponent<FollowCamera>();
						}
					}

					m_camera.SetTarget(player.transform);
				}
			}
		}
	}
}
