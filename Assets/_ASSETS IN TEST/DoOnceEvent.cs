using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event that will only be called once
/// </summary>
public class DoOnceEvent : MonoBehaviour
{
    bool _hasBeenCalled=false;

    public UnityEvent EventsFired;


    public void Invoke()
    {
        if(_hasBeenCalled) return;

        EventsFired?.Invoke();

        _hasBeenCalled = true; 
    
    }


}
