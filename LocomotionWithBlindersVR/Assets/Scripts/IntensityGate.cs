using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityGate : MonoBehaviour
{
    public BlindersVR.IntensityType gatetype;
    public BlindersVR blindersVR = null;

    // Sets intensity to its type on player's collision
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            blindersVR.SetIntensity(gatetype);
        }
    }
}
