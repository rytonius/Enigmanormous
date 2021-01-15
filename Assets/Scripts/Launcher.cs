using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

namespace Com.Enigmanormous
{
    
    public class Launcher : MonoBehaviourPunCallbacks
    
        
    {
        
        public bool stopLoadingText;
        //bool _canpick = false;
        //private PickLevel pickLevel;
        public GameObject buttonsAnchor;
        public void Awake()
        {

            //pickLevel = GameObject.Find("PickLevel").GetComponent<PickLevel>();
            
            Debug.Log("Awake() so syncing up scene");
            // syncs everyone up when game loads
            PhotonNetwork.AutomaticallySyncScene = true;
            
            Connect();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("On Connected To Master");
            buttonsAnchor.SetActive(true);
            //Join();
            stopLoadingText = true;
            base.OnConnectedToMaster();
            
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("On Join Random Failed");
            Create();
            base.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Join Room");
            StartGame();
            base.OnJoinedRoom();
            
        }

        public void Connect ()
        {
            Debug.Log("Trying to Connect....");
            // our game version, i think this will block users who don't know
            PhotonNetwork.GameVersion = "0.0.5";
            // call so you can connect to the server
            PhotonNetwork.ConnectUsingSettings();
        }

        public void Join()
        {
            Debug.Log("Joining a room");
            PhotonNetwork.JoinRandomRoom();
        }

        public void Create()
        {
            Debug.Log("Creating a room");
            PhotonNetwork.CreateRoom("");
        }

        public void StartGame()
        {
            Debug.Log("start game");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("detect 1 player so loading");
                //pickLevel.LevelChoose();
                //_canpick = true;
                PhotonNetwork.LoadLevel(1);

            }
        }
        /*
        public void Update()
        {
            if (_canpick == true)
            {

                if (Input.GetKeyDown(KeyCode.Alpha1)) PhotonNetwork.LoadLevel(1); 
                if (Input.GetKeyDown(KeyCode.Alpha2)) PhotonNetwork.LoadLevel(2);
                
            }
            
        }*/
    }

}
