using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class LOXInteractable : MonoBehaviour
{

    Outline outline;
    [SerializeField]
    GameObject grabUI, completeUI;

    private void Awake()
    {
        outline= GetComponent<Outline>();

        if (!outline)
            Debug.LogError(gameObject + " CONTAINS NO OUTLINE");
    }

    public void TaskStarted()
    {
        outline.OutlineMode = Outline.Mode.OutlineAll;
        grabUI.SetActive(true);
    }

    public void TaskComplete()
    {
        outline.OutlineMode = Outline.Mode.None;
        grabUI.SetActive(false);

        StartCoroutine(DisplayCompleteUI());
    }

    private IEnumerator DisplayCompleteUI()
    {
        completeUI.SetActive(true);
        yield return new WaitForSeconds(1);
        completeUI.SetActive(false);
    }

}
