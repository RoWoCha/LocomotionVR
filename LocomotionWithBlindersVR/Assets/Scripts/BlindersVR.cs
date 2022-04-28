using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BlindersVR : MonoBehaviour
{
    public enum IntensityType
    {
        Zero,
        Low,
        High
    }

    [Header("Blinders Data")]
    public PostProcessVolume postProccessVolume;
    Vignette vignettePP = null;

    public float maxIntensity_Low = 0.4f;
    public float maxIntensity_High = 1.0f;
    float maxIntensity;
    public IntensityType startIntensityType;
    
    public float lerpRate = 0.01f;

    [HideInInspector]
    public VRController playerController;

    void Start()
    {
        // Get vignette
        postProccessVolume.profile.TryGetSettings(out vignettePP);

        // Set intensity according to the starting type
        SetIntensity(startIntensityType);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate intensity target
        float targetIntensity = maxIntensity;
        if (playerController.currentSpeed < playerController.maxSpeed)
        {
            targetIntensity *= (playerController.currentSpeed / playerController.maxSpeed);
        }

        // Linear interpolation of intensity
        vignettePP.intensity.value = Mathf.Lerp(vignettePP.intensity.value, targetIntensity, lerpRate);
    }

    public void OnSnapTurn()
    {
        // Set blinders to max on snap turn
        vignettePP.intensity.value = maxIntensity;
    }

    // Sets maximum intensity accorind to the incoming type
    public void SetIntensity(IntensityType gateType)
    {
        switch (gateType)
        {
            case IntensityType.Zero:
                {
                    maxIntensity = 0.0f;
                    break;
                }
            case IntensityType.Low:
                {
                    maxIntensity = maxIntensity_Low;
                    break;
                }
            case IntensityType.High:
                {
                    maxIntensity = maxIntensity_High;
                    break;
                }
        }
    }
}
