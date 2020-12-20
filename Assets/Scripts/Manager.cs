using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Com.Enigmanormous
{
    public class Manager : MonoBehaviour
    {
        public string PlayerSpawn;
        public Transform[] spawn_points;
        // Start is called before the first frame update
        void Start()
        {
            Spawn();
        }

        // Update is called once per frame
        public void Spawn()
        {
            Transform spawn_point = spawn_points[Random.Range(0, spawn_points.Length)];
            PhotonNetwork.Instantiate(PlayerSpawn, spawn_point.position, spawn_point.rotation);
        }

        
    }

}
