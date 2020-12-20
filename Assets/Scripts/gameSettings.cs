using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameSettings : MonoBehaviour
{
    public float gravityForce = 9.8f;
    // Start is called before the first frame update
    void Start()
    {
        Gravity();
    }

    // Update is called once per frame
    

    void Gravity()
    {
        Physics.gravity = new Vector3(0, -gravityForce, 0);
    }
}
