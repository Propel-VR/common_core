using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OxgBehaviour : MonoBehaviour
{

    public float timeToHold;
    public float timeHeld;

    public ParticleSystem steam;
    public ParticleSystem liquid;
    public ParticleSystem liquidSteam;

    public void BeginStream()
    {
        steam.Play();
    }

   
    IEnumerator CheckHoldTime(float timeToHold)
    {
        yield return new WaitForSeconds(timeToHold);
        liquid.Play();
        steam.Stop();
        liquidSteam.Play();
    }
    
}
