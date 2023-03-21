using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to add task checklist items to the tablet
/// </summary>
[RequireComponent(typeof(Task))]
public class ChecklistTaskHelper : MonoBehaviour
{

    int _checkID;
    int _chapterID;

    Task task;

    [SerializeField]
    List<ChecklistInteractable> interactables;
    [SerializeField]
    bool _isHeader;
    [SerializeField]
    bool _startComplete = false;
    [SerializeField]
    bool _hasCaution = false, _hasWarning = false;
    [SerializeField]
    int _quantity = 1;
    [SerializeField]
    string _taskDesc;
    [SerializeField]
    string _smin = "";


    [SerializeField]
    bool _hasDetails=false;
    [SerializeField]
    DetailsPageData _detailsData;

    private void Awake()
    {
        task=GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);

        if (_hasDetails)
            _detailsData.smin = _smin;
    }

    public int GetID()
    {
        return _checkID;
    }

    /// <summary>
    /// funtion to add task to tablet called from chapter
    /// </summary>
    /// <param name="chapterID"></param>
    public void SetUp(int chapterID)
    {
        _chapterID = chapterID;
        _checkID = Tablet.Instance.AddCheckListItem(_isHeader, gameObject.name, _taskDesc, chapterID, _startComplete,_hasWarning,_hasCaution, _quantity, _smin,_hasDetails,_detailsData, interactables);

        Debug.Log("SET UP TASK WITH ID: " + _checkID);


    }

    /// <summary>
    /// funtion called when task is complete to update the tablet
    /// </summary>
    public void OnComplete()
    {

        Debug.Log("TASK COMPLETE FOR \""+gameObject.name+"\"");
        Tablet.Instance.CompleteItem(_checkID, _chapterID);
    }

    public void UpdateRectification(int id, string rectification)
    {
        _detailsData.rectification = rectification;

        //UPDATE CHECKLIST DATA
        Tablet.Instance.UpdateDetails(_chapterID, id, rectification);
    }

    [System.Serializable]
    public struct DetailsPageData
    {
        [HideInInspector]
        public string smin;
        public string zone, duration, checkType, rectification, warningText;
        public RequiredConditionsData[] requireConditions;
    }

    [System.Serializable]
    public struct RequiredConditionsData
    {
        public Sprite image;
        public string text;
    }

}
