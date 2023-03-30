using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// simple script to expose collider events on an object whgen it touches a specific collider
/// </summary>
public class ColliderEvents : MonoBehaviour
{
    [SerializeField]
    Collider _target;

    public UnityEvent OnEnter, OnExit;
    public UnityEvent OnCollision;

    
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

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke();
    }



}
