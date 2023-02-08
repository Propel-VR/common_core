using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OxgBehaviour : MonoBehaviour
{

    public ParticleSystem steam;
    public ParticleSystem liquid;

    public void BeginStream()
    {
        steam.Play();
        StartCoroutine(WaitForTransition(Random.Range(15.0f, 30.0f)));
    }

    public void EndStream()
    {
        steam.Stop();
        liquid.Stop();
    }

   
    IEnumerator WaitForTransition(float time)
    {
        yield return new WaitForSeconds(time);
        liquid.Play();
        steam.Stop();
    }

   /*
    private void Start()
    {
        BeginStream();
    }
   */ 

}
