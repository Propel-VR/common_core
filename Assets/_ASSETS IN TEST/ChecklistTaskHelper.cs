using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        task=GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);
    }

    public void SetUp(int chapterID)
    {
        _chapterID = chapterID;
        checkID = Tablet.Instance.AddCheckListItem(gameObject.name, chapterID, startComplete,hasWarning,hasCaution);

        Debug.Log("SET UP TASK WITH ID: " + checkID);


    }

    public void OnComplete()
    {

        Debug.Log("TASK COMPLETE FOR \""+gameObject.name+"\"");
        Tablet.Instance.CompleteItem(checkID,_chapterID);
    }

}
