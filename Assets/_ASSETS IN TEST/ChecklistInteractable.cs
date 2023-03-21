using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistInteractable : MonoBehaviour
{

    [SerializeField]
    protected GameObject _identifierUI, _completeUI;

    [SerializeField]
    protected Transform _anchoredTo;

    [SerializeField]
    protected Vector3 _offset = Vector3.up * 0.2f;

    protected Outline _outline;


    private void Awake()
    {
        _outline = GetComponent<Outline>();

        if (!_outline)
            Debug.LogError(gameObject + " CONTAINS NO OUTLINE");
    }


    protected void Update()
    {
        if (_anchoredTo)
        {
            _identifierUI.transform.position = _anchoredTo.position + _offset;
            _completeUI.transform.position = _anchoredTo.position + _offset;
        }
        if (_identifierUI.activeInHierarchy)
        {
            var lookPos = Camera.main.transform.position - _identifierUI.transform.position;
            lookPos.y = 0;
            _identifierUI.transform.rotation = Quaternion.LookRotation(lookPos);

        }
        if (_completeUI && _completeUI.activeInHierarchy)
        {

            var lookPos = Camera.main.transform.position - _completeUI.transform.position;
            //lookPos.y = 0;
            _completeUI.transform.rotation = Quaternion.LookRotation(lookPos);
            //completeUI.transform.rotation = Quaternion.Slerp(completeUI.transform.rotation, rotation, Time.deltaTime/* x damping */);
        }

    }


    public virtual void TaskStarted()
    {
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _identifierUI.SetActive(true);
    }

    public virtual void TaskComplete()
    {
        _outline.OutlineMode = Outline.Mode.None;
        _identifierUI.SetActive(false);
        if (_completeUI != null)
            StartCoroutine(DisplayCompleteUI());
    }

    protected IEnumerator DisplayCompleteUI()
    {
        _completeUI.SetActive(true);
        yield return new WaitForSeconds(1);
        _completeUI.SetActive(false);
    }
}
