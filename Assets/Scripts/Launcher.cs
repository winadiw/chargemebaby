using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
	public class Launcher : Photon.PunBehaviour
	{
        #region Public Variables
        public AudioClip buttonClick;
		/// <summary>
		/// The PUN loglevel. 
		/// </summary>
		public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

		/// <summary>
		/// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
		/// </summary>   
		[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
		public byte MaxPlayersPerRoom = 2;

		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		public GameObject controlPanel;
		[Tooltip("The UI Label to inform the user that the connection is in progress")]
		public GameObject progressLabel;

		#endregion


		#region Private Variables

		/// <summary>
		/// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
		/// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		/// <summary>
		/// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
		/// </summary>
		string _gameVersion = "1";

		private string roomName = "";
		private int createOrJoin = 0;
		#endregion


		#region MonoBehaviour CallBacks


		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake()
		{

			// #NotImportant
			// Force LogLevel
			PhotonNetwork.logLevel = Loglevel;

			// #NotImportant
			// Force Full LogLevel
			PhotonNetwork.logLevel = PhotonLogLevel.Full;


			// #Critical
			// we don't join the lobby. There is no need to join a lobby to get the list of rooms.
			PhotonNetwork.autoJoinLobby = false;


			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.automaticallySyncScene = true;
		}


		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during initialization phase.
		/// </summary>
		void Start()
		{
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);
		}


		#endregion


		#region Public Methods

		public void createRoom()
		{
            SoundManager.instance.PlaySingle(buttonClick);
			createOrJoin = 1;
			Connect ();

		}

		public void joinRoom()
		{
            SoundManager.instance.PlaySingle(buttonClick);
            createOrJoin = 2;
			Connect ();

		}

        public void randomRoom()
        {
            SoundManager.instance.PlaySingle(buttonClick);
            createOrJoin = 3;
            Connect();
        }
        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
		{
			// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
			isConnecting = true;

			progressLabel.SetActive(true);
			controlPanel.SetActive(false);

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.connected)
			{
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                //PhotonNetwork.JoinRandomRoom();
                if (createOrJoin == 1)
                    PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
                else if (createOrJoin == 2)
                    PhotonNetwork.JoinRoom(roomName);
                else if (createOrJoin == 3)
                    PhotonNetwork.JoinRandomRoom();

            }
            else{
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings(_gameVersion);
			}
		}

		public void setRoomName(string roomName)
		{
			this.roomName = roomName;
		}
		#endregion

		#region Photon.PunBehaviour CallBacks

		public override void OnConnectedToMaster()
		{
			// we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				if(createOrJoin == 1)
					PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
				else if(createOrJoin == 2)
					PhotonNetwork.JoinRoom (roomName);
                else if (createOrJoin == 3)
                    PhotonNetwork.JoinRandomRoom();
                Debug.Log (roomName);
				// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
				//PhotonNetwork.JoinRandomRoom();
			}

			Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
		}

		public override void OnDisconnectedFromPhoton()
		{
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);

			Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");        
		}

		public void OnPhotonCreateRoomFailed()
		{
			SceneManager.LoadScene(0);
		}

		public void OnPhotonJoinRoomFailed(object[] cause)
		{
			SceneManager.LoadScene(0);
		}

		public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
		{
			Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

			// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
			PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
		}

		public override void OnJoinedRoom()
		{
			// #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
			//if (PhotonNetwork.room.playerCount == 1)
			//{
				Debug.Log("We load the 'Room for 1' ");

				// #Critical
				// Load the Room Level. 
				PhotonNetwork.LoadLevel("Room for 1");
			//}

			Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
		}

		public void OnCreatedRoom()
		{
			PhotonNetwork.LoadLevel("Room for 1");
		}
		#endregion

	}
}