using CamhOO;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
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

    public Transform ContextPoint;

    [SerializeField]
    private Transform _uiPos;

    private bool contextState=false;
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

    public void SetContextState(bool state)
    {
        contextState= state;
        
        if(contextState)
        {
            _rayInteractable.forceHighlight= true;
            gameObject.SetLayerRecursively(5);
        }
        else
        {
            _rayInteractable.forceHighlight = false;
            gameObject.SetLayerRecursively(7);

        }

    }

    public virtual void MakeReadyForInteract()
    {
        _readyForInteraction = true;
    }

    public void CheckComplete()
    {
        if (string.IsNullOrEmpty(FirstSelection))
        {
            FirstSelection = "Complete";
            RectificationSelection = "N/A";
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
        PreflightReportData data = new PreflightReportData(Task.name, FirstSelection, CorrectSelection, RectificationSelection, CorrectRectification, this);
        PreflightReportUI.Instance.AddItemData(data);

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
        if (string.IsNullOrEmpty(FirstSelection))
        {
            FirstSelection = "Snag Found";
        }

        if (string.IsNullOrEmpty(RectificationSelection))
        {
            RectificationSelection = "Clean";
        }
    }

    public void Service()
    {
        if (string.IsNullOrEmpty(FirstSelection))
        {
            FirstSelection = "Snag Found";
        }

        if (string.IsNullOrEmpty(RectificationSelection))
        {
            RectificationSelection = "Service";
        }
    }

    public void Replace()
    {
        if (string.IsNullOrEmpty(FirstSelection))
        {
            FirstSelection = "Snag Found";
        }

        if (string.IsNullOrEmpty(RectificationSelection))
        {
            RectificationSelection = "Replace";
        }
    }

    public void WriteUp()
    {
        if (string.IsNullOrEmpty(FirstSelection))
        {
            FirstSelection = "Snag Found";
        }

        if (string.IsNullOrEmpty(RectificationSelection))
        {
            RectificationSelection = "Create E-1";
        }
    }
}
