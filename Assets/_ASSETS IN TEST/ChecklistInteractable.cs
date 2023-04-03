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

    protected bool _complete = false;
    public bool IsComplete { get { return _complete; } }

    private float _minDistance = 0.02f, _maxDistance = 40, _maxMult = 10;
    private Vector3 _startSize;

    protected void Awake()
    {
        
        _startSize = _identifierUI.transform.localScale*.4f;
        
        
    }

    private void Start()
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


            _identifierUI.transform.localScale = _startSize * (((Mathf.Clamp((_identifierUI.transform.position - Camera.main.transform.position).magnitude, _minDistance, _maxDistance)) - _minDistance) / (_maxDistance - _minDistance)) * _maxMult;

            if (_identifierUI.transform.localScale.sqrMagnitude < _startSize.sqrMagnitude)
                _identifierUI.transform.localScale = _startSize;
            
        } 

    }


    public virtual void TaskStarted()
    {
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _identifierUI.SetActive(true);
    }

    public virtual void TaskComplete()
    {
        _complete = true;
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
