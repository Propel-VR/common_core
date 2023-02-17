using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Outline))]
public class LOXInteractable : MonoBehaviour
{

    Outline outline;
    [SerializeField]
    GameObject grabUI, completeUI;

    [SerializeField]
    Transform anchoredTo;
    Vector3 offset= Vector3.up*0.2f;

    private void Awake()
    {
        outline= GetComponent<Outline>();

        if (!outline)
            Debug.LogError(gameObject + " CONTAINS NO OUTLINE");
    }

    private void Update()
    {
        if (anchoredTo)
        {
            grabUI.transform.position = anchoredTo.position + offset;
            completeUI.transform.position = anchoredTo.position + offset;
        }
        if (grabUI.activeInHierarchy)
        {
            var lookPos = Camera.main.transform.position - grabUI.transform.position;
            lookPos.y = 0;
            grabUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //grabUI.transform.rotation = Quaternion.Slerp(grabUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }
        if(completeUI.activeInHierarchy)
        {

            var lookPos = Camera.main.transform.position - completeUI.transform.position;
            //lookPos.y = 0;
            completeUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //completeUI.transform.rotation = Quaternion.Slerp(completeUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }
    
    }

    public void TaskStarted()
    {
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        grabUI.SetActive(true);
    }

    public void TaskComplete()
    {
        outline.OutlineMode = Outline.Mode.None;
        grabUI.SetActive(false);
        if(completeUI!= null)
            StartCoroutine(DisplayCompleteUI());
    }

    private IEnumerator DisplayCompleteUI()
    {
        completeUI.SetActive(true);
        yield return new WaitForSeconds(1);
        completeUI.SetActive(false);
    }

}
