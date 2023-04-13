using CamhOO;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
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

    [HideInInspector]
    public string FirstSelection = "", CorrectSelection="", RectificationSelection ="", CorrectRectification="";

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
        CorrectSelection = "Complete";
        CorrectRectification = "N/A";
    }


    public virtual void MakeReadyForInteract()
    {
        _readyForInteraction = true;
    }

    public void CheckComplete()
    {
        if (FirstSelection.IsNullOrEmpty())
        {
            FirstSelection = "Complete";
        }

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

        ReportUI.Instance.AddItem(Task.name, FirstSelection, CorrectSelection, RectificationSelection, CorrectRectification);

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

    public void Clean()
    {
        if (FirstSelection.IsNullOrEmpty())
        {
            FirstSelection = "Snag Found";
        }

        if (RectificationSelection.IsNullOrEmpty())
        {
            RectificationSelection = "Clean";
        }
    }

    public void Service()
    {
        if (FirstSelection.IsNullOrEmpty())
        {
            FirstSelection = "Snag Found";
        }

        if (RectificationSelection.IsNullOrEmpty())
        {
            RectificationSelection = "Service";
        }
    }

    public void Replace()
    {
        if (FirstSelection.IsNullOrEmpty())
        {
            FirstSelection = "Snag Found";
        }

        if (RectificationSelection.IsNullOrEmpty())
        {
            RectificationSelection = "Replace";
        }
    }

    public void WriteUp()
    {
        if (FirstSelection.IsNullOrEmpty())
        {
            FirstSelection = "Snag Found";
        }

        if (RectificationSelection.IsNullOrEmpty())
        {
            RectificationSelection = "Create E-1";
        }
    }
}
