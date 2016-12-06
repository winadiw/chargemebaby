using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement; 

namespace Com.MyCompany.MyGame
{
	public class GameManager : Photon.PunBehaviour
    {
		#region Public Variables

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;
        public GameObject PleaseWait;
        public GameObject YouWin;
        public GameObject YouLose;
        public GameObject Player1Text;
        public GameObject Player2Text;
        public GameObject RoomNameText;
        public GameObject PingText;
        public AudioClip buttonClick;
        String s1 = "";
        String s2 = "";

        #endregion

        #region MonoBehaviour CallBacks
        private void Awake()
        {
            disableYouLose();
            disableYouWin();
            SoundManager.instance.StartFirstBGM();
            //DontDestroyOnLoad(this.gameObject);
        }
        void Start()
		{
            if (playerPrefab == null)
			{
				Debug.LogError ("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			}
			else
			{
				if (Player.LocalPlayerInstance==null)
				{
					Debug.Log("We are Instantiating LocalPlayer from "+ Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    System.Random rnd = new System.Random();
                    int number = rnd.Next(1, 4); // creates a number between 1 and 12
                    Debug.Log("Random Number: " + number);
                    if(number == 1)
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(-1f, 8f, 0f), Quaternion.identity, 0);
                    else if (number == 2)
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 8f, 0f), Quaternion.identity, 0);
                    else if (number == 3)
                        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(1f, 8f, 0f), Quaternion.identity, 0);
                }
                else
                {
					Debug.Log("Ignoring scene load for "+ Application.loadedLevelName);
				}
			}

            RoomNameText.GetComponent<Text>().text = "Room: " + PhotonNetwork.room.name;
            PhotonPlayer[] otherList = PhotonNetwork.otherPlayers;
            
            foreach (PhotonPlayer player in otherList)
            {
                   s2 += player.ToString();
            }
            s1 = PhotonNetwork.playerName;
            if (Player1Text != null)
                Player1Text.GetComponent<Text>().text = s1;
            if (Player2Text != null)
                Player2Text.GetComponent<Text>().text = s2;
        }

        void Update()
        {
            PingText.GetComponent<Text>().text = PhotonNetwork.GetPing().ToString();
            if (PhotonNetwork.playerList.Length == 2)
            {
                if (PleaseWait != null)
                    PleaseWait.GetComponent<Text>().enabled = false;
            }
            else if(PhotonNetwork.playerList.Length == 1)
            {
                if(PleaseWait != null)
                    PleaseWait.GetComponent<Text>().enabled = true;
            }
        }

        #endregion

        #region Photon Messages

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.name); // not seen if you're the player connecting

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
                LoadArena();
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.name); // seen when other disconnects
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        /// 
        public void OnLeftRoom()
		{
            Debug.Log("Left Room Called!");
            SoundManager.instance.StartSecondBGM();
            SceneManager.LoadScene(0);
		}

        public void enableYouLose()
        {
            if(YouLose!=null)
             YouLose.GetComponent<Text>().enabled = true;
        }

        public void disableYouLose()
        {
            if (YouLose != null)
                YouLose.GetComponent<Text>().enabled = false;
        }

        public void enableYouWin()
        {
            if (YouWin != null)
                YouWin.GetComponent<Text>().enabled = true;
        }

        public void disableYouWin()
        {
            if (YouWin != null)
                YouWin.GetComponent<Text>().enabled = false;
        }
        public void EndRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods

        void LoadArena()
		{
			if ( ! PhotonNetwork.isMasterClient ) 
			{
				Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
			}
			Debug.Log( "PhotonNetwork : Loading Level : " + PhotonNetwork.room.playerCount );
			//PhotonNetwork.LoadLevel("Room for "+PhotonNetwork.room.playerCount);
			PhotonNetwork.LoadLevel("Room for 1");
		}

		#endregion

		#region Public Methods

		public void LeaveRoom()
		{
            Debug.Log("Leave Room Pressed!");
            SoundManager.instance.PlaySingle(buttonClick);
            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene(0);
        }

        #endregion
    }
}
