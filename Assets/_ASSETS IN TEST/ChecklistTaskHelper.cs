using CamhOO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Task))]
public class ChecklistTaskHelper : MonoBehaviour
{

    int checkID;

    Task task;

    private void Awake()
    {
        task=GetComponent<Task>();

        task.OnTaskCompleted.AddListener(OnComplete);
    }

    public void SetUp()
    {
        checkID = Tablet.Instance.AddCheckListItem(gameObject.name);

        Debug.Log("SET UP TASK WITH ID: " + checkID);

    }

    public void OnComplete()
    {

        Debug.Log("TASK COMPLETE FOR \""+gameObject.name+"\"");
        Tablet.Instance.CompleteItem(checkID);
    }

}
