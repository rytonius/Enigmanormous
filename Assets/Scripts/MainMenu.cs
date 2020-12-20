using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Enigmanormous
{


    public class MainMenu : MonoBehaviour
    {
        public Launcher launcher;


        public void JoinMatch()
        {
            //SceneManager.LoadScene("scene name");
            launcher.Join();
        }

        public void CreateMatch()
        {
            launcher.Create();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
