using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Com.Enigmanormous
{
    public class PickLevel : MonoBehaviour
    {
        public Text loadLevelText;
        // Start is called before the first frame update

        private void Awake()
        {
            loadLevelText = GetComponent<Text>();
        }
        public void LevelChoose()
        {
            loadLevelText.enabled = true;
        }
    }
}