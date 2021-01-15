
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.Enigmanormous
{
    public class WeaponSway : MonoBehaviour
    {

        #region variables
        public float intensity = 2f;
        public float smooth = 5f;
        public bool IsMine;

        private Quaternion origin_rotation;
        #endregion

        #region MonoBehavior Callbacks
        private void Start()
        {
            origin_rotation = transform.localRotation;
            
        }

        private void Update()
        {
            if (Pause.paused) return;
            UpdateSway();
        }
        #endregion

        #region Private Meth

        private void UpdateSway()
        {
            //get that mouse rotation
            float getMouseX = Input.GetAxis("Mouse X");
            float getMouseY = Input.GetAxis("Mouse Y");


            if (!IsMine)
            {
                getMouseX = 0;
                getMouseY = 0;
            }
            //calculate target rotation
            Quaternion target_Adjust_X = Quaternion.AngleAxis(-intensity * getMouseX, Vector3.up);
            Quaternion target_Adjust_Y = Quaternion.AngleAxis(intensity * getMouseY, Vector3.right);
            Quaternion target_rotation = origin_rotation * target_Adjust_X * target_Adjust_Y;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
        }
        #endregion

    }
}