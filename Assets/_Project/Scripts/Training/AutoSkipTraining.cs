using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSkipTraining : MonoBehaviour
{
    [SerializeField] EasyInteractable interactables;
    static bool hasBeenTrained = false;

    void Start ()
    {
        if(!hasBeenTrained)
        {
            hasBeenTrained = true;
            return;
        }
        else
        {
            interactables.OnInteract();
        }
    }
}
