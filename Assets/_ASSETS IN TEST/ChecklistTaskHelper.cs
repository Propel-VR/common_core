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
    bool startComplete = false;
    [SerializeField]
    bool hasCaution = false, hasWarning = false;
    [SerializeField]
    int quantity = 1;
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

    /// <summary>
    /// funtion to add task to tablet called from chapter
    /// </summary>
    /// <param name="chapterID"></param>
    public void SetUp(int chapterID)
    {
        _chapterID = chapterID;
        checkID = Tablet.Instance.AddCheckListItem(gameObject.name, chapterID, startComplete,hasWarning,hasCaution, quantity, smin,hasDetails,detailsData);

        Debug.Log("SET UP TASK WITH ID: " + checkID);


    }

    /// <summary>
    /// funtion called when task is complete to update the tablet
    /// </summary>
    public void OnComplete()
    {

        Debug.Log("TASK COMPLETE FOR \""+gameObject.name+"\"");
        Tablet.Instance.CompleteItem(checkID,_chapterID);
    }


    [System.Serializable]
    public struct DetailsPageData
    {
        [HideInInspector]
        public string smin;
        public string zone, duration, checkType, rectification;
        public RequiredConditionsData[] requireConditions;
    }

    [System.Serializable]
    public struct RequiredConditionsData
    {
        public Sprite image;
        public string text;
    }

}
