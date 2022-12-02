using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRestTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Footrest footrest = other.GetComponent<Footrest>();
        if (footrest)
        {
            footrest.OnEnterTrigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Footrest footrest = other.GetComponent<Footrest>();
        if (footrest)
        {
            footrest.OnExitTrigger();
        }
    }
}
