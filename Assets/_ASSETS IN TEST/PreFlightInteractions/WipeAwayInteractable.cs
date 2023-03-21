using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WipeAwayInteractable : PreFlightInteractable
{

    public UnityEvent OnWipedClean, OnMakeReadyForInteractable;

    private void OnTriggerEnter(Collider other)
    {
        if (_readyForInteraction && other.CompareTag("Wiping"))
        {
            WipeClean();
            OnMakeReadyForInteractable?.Invoke();
        }
    }

    public override void MakeReadyForInteract()
    {
        base.MakeReadyForInteract();

    }

    public void WipeClean()
    {
        HasBeenChecked();
        StartCoroutine(Clean());
    }

    public IEnumerator Clean()
    {
        yield return new WaitForSeconds(1.5f);
        OnInteracted?.Invoke();
        OnWipedClean?.Invoke();
        UpdateRectification();
    }

    public override void UpdateRectification()
    {
        _cth.UpdateRectification(_cth.GetID(),"Cleaned");
    }

}
