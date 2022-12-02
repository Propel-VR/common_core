using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivityEventListener : MonoBehaviour
{
    [SerializeField] ActivityType targetActivityType;
    [SerializeField] UnityEvent onActivityStart;


    private void OnEnable ()
    {
        ActivityManager.onActivityStart += OnActivityStart;

    }
    private void OnDisable ()
    {
        ActivityManager.onActivityStart -= OnActivityStart;   
    }

    private void OnActivityStart (ActivityType type)
    {
        if(type == targetActivityType)
        {
            onActivityStart?.Invoke();
        }
    }
}
