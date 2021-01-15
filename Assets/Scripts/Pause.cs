using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Com.Enigmanormous
{
    public class Pause : MonoBehaviourPunCallbacks
    {
        public static bool paused = false;
        private bool disconnecting = false;
        // Start is called before the first frame update
        public void TogglePause()
        {
            if (disconnecting) return;

            paused = !paused;

            transform.GetChild(0).gameObject.SetActive(paused);
            Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

        }

        // Update is called once per frame
        public void Quit()
        {
            disconnecting = true;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
            base.OnLeftRoom();
        }
    }
}

