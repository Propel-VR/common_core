using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistTaskManager : MonoBehaviour
{

    private void Start()
    {
        AddTaskToTablet(transform, false);

        Tablet.Instance.StartTasks();
    }



    private void AddTaskToTablet(Transform obj, bool printMe)
    {
        if (printMe)
        {
            ChecklistTaskHelper cth = obj.GetComponent<ChecklistTaskHelper>();
            if (cth != null)
                cth.SetUp();

        }
        

        for (int i = 0; i < obj.childCount; i++)
            AddTaskToTablet(obj.GetChild(i), true);
    }
}
