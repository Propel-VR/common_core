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
    protected bool _readyForInteraction;

    public UnityEvent OnInteracted;

    private RayInteractable _rayInteractable;
    protected ChecklistTaskHelper _cth;

    [SerializeField]
    private Transform _uiPos;
    public Transform _UIPos
    {
        get { return _uiPos; }
    }


    protected void Awake()
    {
        base.Awake();
        _rayInteractable = GetComponent<RayInteractable>();
        _cth = Task.GetComponent<ChecklistTaskHelper>();

    }


    public virtual void MakeReadyForInteract()
    {
        _readyForInteraction = true;
    }

    public void CheckComplete()
    {
        _complete = true;

        Task.CompleteTask();
        HasBeenChecked();
        OnInteracted.Invoke();
        
    }



    public override void TaskStarted()
    {
        _rayInteractable.forceHighlight = true;
        _identifierUI.SetActive(true);
    }

    public override void TaskComplete()
    {
        _rayInteractable.forceHighlight = false;
        _identifierUI.SetActive(false);
        if (_completeUI != null)
        {
            _completeUI.transform.position = _UIPos.position;
            _completeUI.transform.rotation = _UIPos.rotation;
            _completeUI.SetActive(true);
        }
    }


    public void HasBeenChecked()
    {
        _rayInteractable.ClearInteractions();
    }

    public virtual void UpdateRectification()
    {
        
    }
}
