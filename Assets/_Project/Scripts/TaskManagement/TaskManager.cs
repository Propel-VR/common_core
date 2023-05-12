using Autohand;
using LM.Management;
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
        #region Supplementary Types

        public enum TaskState
        {
            Idle = 0,
            Started,
            Completed
        }

        #endregion

        #region Singleton

        public static TaskManager Instance { get; private set; }

        #endregion

        Dictionary<Task, TaskState> _tasks;
        bool _hasStarted;

        #region Events

        public event TaskEvent OnTaskStarted;

        public event TaskEvent OnTaskCompleted;

        #endregion

        public void StartChecklist()
        {
            _tasks = new();

            foreach (Task task in Configurator.Instance.Configuration.tasks)
            {
                _tasks[task] = TaskState.Idle;
                task.DoOnSetup();
            }

            _hasStarted = true;
        }

        public void StartTask(Task task)
        {
            if (!_hasStarted || !_tasks.ContainsKey(task))
                return;

            if (_tasks[task] != TaskState.Idle)
                return;

            _tasks[task] = TaskState.Started;
            task.DoOnStarted();
            OnTaskStarted?.Invoke(task);
        }

        public void CompleteTask(Task task)
        {
            if (!_hasStarted || !_tasks.ContainsKey(task))
                return;

            if (_tasks[task] != TaskState.Started)
                return;

            _tasks[task] = TaskState.Completed;
            task.DoOnCompleted();
            OnTaskCompleted?.Invoke(task);
        }
    }

}
