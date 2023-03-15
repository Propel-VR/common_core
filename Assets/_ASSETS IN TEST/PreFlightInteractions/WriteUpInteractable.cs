using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WriteUpInteractable : PreFlightInteractable
{

    public UnityEvent OnWriteUpInteraction,OnMakeReadyForInteraction;

    [SerializeField]
    float interactionDelay;

    public override void MakeReadyForInteract()
    {
        base.MakeReadyForInteract();
        OnMakeReadyForInteraction?.Invoke();

    }

    public void DoInteraction()
    {
        HasBeenChecked();
        StartCoroutine(Interact());
    }

    public IEnumerator Interact()
    {
        yield return new WaitForSeconds(interactionDelay);
        OnInteracted?.Invoke();
        OnWriteUpInteraction?.Invoke();
        UpdateRectification();
    }

    public override void UpdateRectification()
    {
        cth.UpdateRectification(cth.GetID(),"Written Up");
    }

}
