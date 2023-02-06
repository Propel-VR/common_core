using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxgBehaviour : MonoBehaviour
{

    public float timeToHold;
    public float timeHeld;

    public ParticleSystem steam;
    public ParticleSystem liquid;
    public ParticleSystem liquidSteam;

    public Animator anim;

    void BeginStream()
    {
        steam.Play();
        anim.SetTrigger("TransitionTrigger");
    }

   
    IEnumerator CheckHoldTime(float timeToHold)
    {
        yield return new WaitForSeconds(timeToHold);
        liquid.Play();
        steam.Stop();
        liquidSteam.Play();
    }

// Start is called before the first frame update
    void Start()
    {
        BeginStream();
        StartCoroutine(CheckHoldTime(timeToHold));
        
    }

    /*
    IEnumerator StreamAnimate()
    {
        ParticleSystem.MainModule main = steam.main;
        main.startLifetime = 1f;
        float t = 0;

        while(t < 5)
        {
            main.startLifetime = Mathf.Lerp(1f, 4f, t / 5);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    */
    /*public void BeginStream()
{
    steam.Play();

    StartCoroutine(StreamAnimate());
}

*/
}
