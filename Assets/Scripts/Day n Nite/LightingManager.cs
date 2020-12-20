using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField]private Light DirectionalLight;
    [SerializeField]private LightingPreset Preset;
    //variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField] float _speedOfDay = 0.5f;

    private void Update()
    {
        if (Preset == null)
        {
            Debug.Log("Making a bug to fix a bug; if you see my big brother, tell him to shut the fuck up, and then break cause you fucked something up");
            return;
        }
            

        if(Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime * _speedOfDay;
            TimeOfDay %= 24; //clamp between 0-24
            UpdateLighting(TimeOfDay / 24f);

        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.Fogcolor.Evaluate(timePercent);

        if(DirectionalLight!=null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;

        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
        
    }

}
