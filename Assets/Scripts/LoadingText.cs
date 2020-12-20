using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Com.Enigmanormous
{

    public class LoadingText : MonoBehaviour
    {

        public TextMeshProUGUI TextMesh;
        private Launcher launcher;

        [SerializeField] float _waitTime = 1f;


        // Start is called before the first frame update
        void Start()
        {
            launcher = FindObjectOfType<Launcher>();
            //TextMesh.text = "Loading";
            StartCoroutine("Adddots", _waitTime);
        }



        public IEnumerator Adddots()
        {
            while (!launcher.stopLoadingText)
            {
                TextMesh.text = TextMesh.text + ".";

                yield return new WaitForSeconds(_waitTime);
            }


        }

    }
}
