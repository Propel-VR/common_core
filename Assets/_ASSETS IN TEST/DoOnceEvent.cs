using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoOnceEvent : MonoBehaviour
{
    bool hasBeenCalled=false;

    public UnityEvent EventsFired;


    public void Invoke()
    {
        if(hasBeenCalled) return;

        EventsFired?.Invoke();

        hasBeenCalled = true; 
    
    }


}
