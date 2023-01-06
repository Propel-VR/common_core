using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    [SerializeField]
    Collider _target;

    public UnityEvent OnEnter, OnExit;

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.Equals(_target))
            OnEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.Equals(_target))
            OnExit?.Invoke();
    }



}
