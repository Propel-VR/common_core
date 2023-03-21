using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// class that handles events for enable, start, etc
/// </summary>
public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] UnityEvent DoOnStart;
    [SerializeField] UnityEvent DoOnAwake;
    [SerializeField] UnityEvent DoOnEnable;
    [SerializeField] UnityEvent DoOnDisable;
    [SerializeField] UnityEvent DoOnDestroy;
    [SerializeField] UnityEvent DoOnValidate;

    

    private void OnValidate()
    {
        DoOnValidate?.Invoke();   
    }
    private void Start()
    {
        DoOnStart?.Invoke();
    }
    private void Awake()
    {
        DoOnAwake?.Invoke();
    }
    private void OnEnable()
    {
        DoOnEnable?.Invoke();
    }
    private void OnDisable()
    {
        DoOnDisable?.Invoke();
    }
    private void OnDestroy()
    {
        DoOnDestroy?.Invoke();
    }
}
