using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangingLogo : MonoBehaviour
{

    public Text myText;
    public RectTransform RectTran;

    
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (myText != null)
        {
            Color newColor = new Color(Random.value, Random.value, Random.value);

            myText.color = newColor;
            RectTran.Rotate(new Vector3(0f, 45f, 0f) * Time.deltaTime);
        }
        


        
        
        
    }
}
