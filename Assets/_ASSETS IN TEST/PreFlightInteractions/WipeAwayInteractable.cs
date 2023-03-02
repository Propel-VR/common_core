using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WipeAwayInteractable : PreFlightInteractable
{

    [SerializeField]
    LOXInteractable cloth;

    public UnityEvent OnWipedClean;

    private void OnTriggerEnter(Collider other)
    {
        if (readyForInteraction && other.CompareTag("Wiping"))
        {
            WipeClean();
        }
    }

    public override void MakeReadyForInteract()
    {
        base.MakeReadyForInteract();
        cloth.TaskStarted();

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
        cth.UpdateRectification(cth.GetID(),"Cleaned");
    }

}
