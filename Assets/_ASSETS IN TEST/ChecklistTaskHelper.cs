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

    int checkID;
    int _chapterID;

    Task task;

    [SerializeField]
    List<ChecklistInteractable> interactables;

    [SerializeField]
    bool startComplete = false;
    [SerializeField]
    bool hasCaution = false, hasWarning = false;
    [SerializeField]
    int quantity = 1;
    [SerializeField]
    string taskDesc;
    [SerializeField]
    string smin = "";


    [SerializeField]
    bool hasDetails=false;
    [SerializeField]
    DetailsPageData detailsData;

    private void Awake()
    {
        task=GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);

        if (hasDetails)
            detailsData.smin = smin;
    }

    public int GetID()
    {
        return checkID;
    }

    /// <summary>
    /// funtion to add task to tablet called from chapter
    /// </summary>
    /// <param name="chapterID"></param>
    public void SetUp(int chapterID)
    {
        _chapterID = chapterID;
        checkID = Tablet.Instance.AddCheckListItem(gameObject.name, taskDesc, chapterID, startComplete,hasWarning,hasCaution, quantity, smin,hasDetails,detailsData, interactables);

        Debug.Log("SET UP TASK WITH ID: " + checkID);


    }

    /// <summary>
    /// funtion called when task is complete to update the tablet
    /// </summary>
    public void OnComplete()
    {

        Debug.Log("TASK COMPLETE FOR \""+gameObject.name+"\"");
        Tablet.Instance.CompleteItem(checkID, _chapterID);
    }

    public void UpdateRectification(int id, string rectification)
    {
        detailsData.rectification = rectification;

        //UPDATE CHECKLIST DATA
        Tablet.Instance.UpdateDetails(_chapterID, id, rectification);
    }

    public void MakeActive()
    {
        foreach (ChecklistInteractable i in interactables)
            i.TaskStarted();
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
