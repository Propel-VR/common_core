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

    public AudioSource steamAudio, steamLiquidAdio;

    public void BeginStream()
    {
        steam.Play();
        steamAudio.Play();
        StartCoroutine(CheckHoldTime());
    }

   
    IEnumerator CheckHoldTime()
    {
        yield return new WaitForSeconds(timeToHold);
        liquid.Play();
        steam.Stop();
        steamAudio.Stop();
        steamLiquidAdio.Play();
        liquidSteam.Play();
    }

    public void StopStreams()
    {
     StartCoroutine(StartLiquidStop()); 
    }


    IEnumerator StartLiquidStop()
    {
        yield return new WaitForSeconds(1.5f);
        liquid.Stop();
        steam.Stop();
        liquidSteam.Stop();
        steamAudio.Stop();
        steamLiquidAdio.Stop();
    }


}
