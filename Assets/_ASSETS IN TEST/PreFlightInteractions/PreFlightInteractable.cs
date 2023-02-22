using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PreFlightInteractable : MonoBehaviour
{
    public Task Task;
    public bool ReadyForInteraction;

    public UnityEvent OnInteracted;

}
