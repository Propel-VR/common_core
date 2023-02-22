using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipeAwayInteractable : PreFlightInteractable
{

    private void OnTriggerEnter(Collider other)
    {
        if (ReadyForInteraction && other.CompareTag("Wiping"))
        {
            WipeClean();
        }
    }

    public void WipeClean()
    {
        OnInteracted?.Invoke();
    }

}
