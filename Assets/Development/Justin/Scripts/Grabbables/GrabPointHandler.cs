using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPointHandler : MonoBehaviour
{
    GrabPointBehaviour[] grabPoints;

    private static GrabPointHandler instance;
    public static GrabPointHandler Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
        grabPoints = GetComponentsInChildren<GrabPointBehaviour>();
        
    }

    public void UnfocusAllPoints()
    {
        foreach (GrabPointBehaviour grabPoint in grabPoints)
            if(grabPoint.enabled) grabPoint.Unfocus();
    }

    public void HideAllSprites()
    {
        foreach (GrabPointBehaviour grabPoint in grabPoints)
            grabPoint.HideSprite();
    }
}
