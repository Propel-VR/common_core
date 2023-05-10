using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LM.TaskManagement
{

    public class UnityTaskEvent : UnityEvent<Task> { }

    public delegate void TaskEvent(Task task);

    public class TaskManager : MonoBehaviour
    {
        [SerializeField]
        List<Task> _tasks = new();

        public event TaskEvent OnTaskStarted;

        public event TaskEvent OnTaskCompleted;

        public void StartTask(Task task)
        {

        }
    }

}
