using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistInteractable : MonoBehaviour
{

    [SerializeField]
    protected GameObject identifierUI, completeUI;

    [SerializeField]
    protected Transform anchoredTo;

    [SerializeField]
    protected Vector3 offset = Vector3.up * 0.2f;

    protected Outline outline;


    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (!outline)
            Debug.LogError(gameObject + " CONTAINS NO OUTLINE");
    }


    protected void Update()
    {
        if (anchoredTo)
        {
            identifierUI.transform.position = anchoredTo.position + offset;
            completeUI.transform.position = anchoredTo.position + offset;
        }
        if (identifierUI.activeInHierarchy)
        {
            var lookPos = Camera.main.transform.position - identifierUI.transform.position;
            lookPos.y = 0;
            identifierUI.transform.rotation = Quaternion.LookRotation(lookPos);

        }
        if (completeUI && completeUI.activeInHierarchy)
        {

            var lookPos = Camera.main.transform.position - completeUI.transform.position;
            //lookPos.y = 0;
            completeUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //completeUI.transform.rotation = Quaternion.Slerp(completeUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }

    }


    public virtual void TaskStarted()
    {
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        identifierUI.SetActive(true);
    }

    public virtual void TaskComplete()
    {
        outline.OutlineMode = Outline.Mode.None;
        identifierUI.SetActive(false);
        if (completeUI != null)
            StartCoroutine(DisplayCompleteUI());
    }

    protected IEnumerator DisplayCompleteUI()
    {
        completeUI.SetActive(true);
        yield return new WaitForSeconds(1);
        completeUI.SetActive(false);
    }
}
