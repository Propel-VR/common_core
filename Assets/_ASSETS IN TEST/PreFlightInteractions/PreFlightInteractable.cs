using CamhOO;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayInteractable))]
public class PreFlightInteractable : MonoBehaviour
{
    public Task Task;
    protected bool readyForInteraction;

    public UnityEvent OnInteracted;

    private RayInteractable _rayInteractable;
    protected ChecklistTaskHelper cth;

    [SerializeField]
    GameObject lookUI, completeUI;

    [SerializeField]
    Transform anchoredTo;
    Vector3 offset = Vector3.up * 0.2f;

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


    private void Update()
    {
        if (anchoredTo)
        {
            lookUI.transform.position = anchoredTo.position + offset;
            completeUI.transform.position = anchoredTo.position + offset;
        }
        if (lookUI.activeInHierarchy)
        {
            var lookPos = Camera.main.transform.position - lookUI.transform.position;
            lookPos.y = 0;
            lookUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //grabUI.transform.rotation = Quaternion.Slerp(grabUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }
        if (completeUI.activeInHierarchy)
        {

            var lookPos = Camera.main.transform.position - completeUI.transform.position;
            //lookPos.y = 0;
            completeUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //completeUI.transform.rotation = Quaternion.Slerp(completeUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }

    }

    public void TaskStarted()
    {
        _rayInteractable.forceHighlight = true;
        lookUI.SetActive(true);
    }

    public void TaskComplete()
    {
        _rayInteractable.forceHighlight = false;
        lookUI.SetActive(false);
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
