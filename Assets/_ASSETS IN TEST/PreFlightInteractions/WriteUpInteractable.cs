using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WriteUpInteractable : PreFlightInteractable
{

    public UnityEvent OnWriteUpInteraction,OnMakeReadyForInteraction;

    [SerializeField]
    float _interactionDelay;


    private void Start()
    {
        CorrectRectification = "Create E-1";
        CorrectSelection = "Snag Found";
    }

    public override void MakeReadyForInteract()
    {
        base.MakeReadyForInteract();
        OnMakeReadyForInteraction?.Invoke();

    }

    public void DoInteraction()
    {

        StartCoroutine(Interact());
        HasBeenChecked();
    }

    public IEnumerator Interact()
    {
        OnInteracted?.Invoke();
        yield return new WaitForSeconds(_interactionDelay);
        OnWriteUpInteraction?.Invoke();
        UpdateRectification();
    }

    public override void UpdateRectification()
    {
        _cth.UpdateRectification(_cth.GetID(),"Created E-1");
    }

}
