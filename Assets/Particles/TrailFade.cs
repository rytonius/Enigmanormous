using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Enigmanormous
{
    public class TrailFade : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private float speed = 10f;

        LineRenderer lr;
        // Start is called before the first frame update
        void Start()
        {
            lr = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            // move towards zero
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * speed);

            // update color
            // lr.SetColors (color, color);
            lr.startColor = color;
            lr.endColor = color;
        }
    }
}

