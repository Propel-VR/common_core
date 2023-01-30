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

    }

    public void OnComplete()
    {
        Tablet.Instance.CompleteItem(checkID);
    }

}
