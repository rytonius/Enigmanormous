using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MainMenuCameraFX : MonoBehaviour
{
    public float Temperature = 100f;
    [Range(10, 50)]
    public float slideTSpeed = 10f;
    private bool _goDown = true;
    
    


    private PostProcessVolume PP;
    private ColorGrading colorGradingLayer = null;
    private Bloom bloomLayer = null;
    public int bloomDelay = 100;
    private int _delayReset = 0;
    // Start is called before the first frame update
    void Start()
    {
        PP = gameObject.GetComponent<PostProcessVolume>();
        PP.profile.TryGetSettings(out colorGradingLayer);
        PP.profile.TryGetSettings(out bloomLayer);
        colorGradingLayer.temperature.value = Temperature;

    }

    // Update is called once per frame
    void Update()
    {
        SlideFX();
        
        colorGradingLayer.temperature.value = Temperature;

        _delayReset -= 1;
        if (_delayReset <= 0)
        {
            bloomLayer.intensity.value = Random.Range(90f, 120f);
            _delayReset = bloomDelay;
        }
        
    }

    private void SlideFX()
    {
        if (Temperature == 100f) { _goDown = true; }
        if (_goDown) { Temperature = Mathf.MoveTowards(Temperature, -100f, Time.deltaTime * slideTSpeed); }

        if (Temperature == -100f) { _goDown = false;  }
        if (!_goDown){ Temperature = Mathf.MoveTowards(Temperature, 100f, Time.deltaTime * slideTSpeed); }
    }
}
