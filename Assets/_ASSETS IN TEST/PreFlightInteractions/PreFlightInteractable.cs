using CamhOO;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayInteractable))]
public class PreFlightInteractable : ChecklistInteractable
{
    public Task Task;
    protected bool readyForInteraction;

    public UnityEvent OnInteracted;

    private RayInteractable _rayInteractable;
    protected ChecklistTaskHelper cth;


    private void Awake()
    {
        _rayInteractable = GetComponent<RayInteractable>();
        cth = Task.GetComponent<ChecklistTaskHelper>();

    }


    public virtual void MakeReadyForInteract()
    {
        readyForInteraction = true;
    }

    public void CheckComplete()
    {
        Task.CompleteTask();
        HasBeenChecked();
        cth.UpdateRectification(cth.GetID(), "N/A");
        OnInteracted.Invoke();
        
    }



    public override void TaskStarted()
    {
        _rayInteractable.forceHighlight = true;
        identifierUI.SetActive(true);
    }

    public override void TaskComplete()
    {
        _rayInteractable.forceHighlight = false;
        identifierUI.SetActive(false);
        if (completeUI != null)
            completeUI.SetActive(true);
    }


    public void HasBeenChecked()
    {
        _rayInteractable.ClearInteractions();
    }

    public virtual void UpdateRectification()
    {
        
    }
}
