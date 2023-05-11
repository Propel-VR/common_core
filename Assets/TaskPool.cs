using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace CommonCoreScripts
{

    /// <summary>
    /// Contains a pool of tasks which can either be initialized sequentially
    /// or all at once.
    /// </summary>
    public class TaskPool : MonoBehaviour
    {
        #region Supplementary Types

        public enum PoolType
        {
            Sequential,
            Unordered
        }

        public enum PoolStatus
        {
            Idle,
            Started,
            Finished
        }

        #endregion

        #region Serialized Fields and Events

        [SerializeField] 
        PoolType type;

        [Header("Tasks")]
        [SerializeField]
        List<Task> _tasks = new();

        [Header("Events")]
        public UnityEvent OnStartPool;
        public UnityEvent OnFinishPool;
        public UnityEvent OnStartTask;
        public UnityEvent OnFinishTask;

        #endregion

        #region Private Fields

        int _currentTaskIndex = -1;
        PoolStatus status = PoolStatus.Idle;

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            // subscribe to task events since starting/completing tasks is now done through the Tasks themselves
            foreach (Task task in _tasks)
            {
                task.OnTaskStarted.AddListener(() => StartTaskHandler(task));
                task.OnTaskCompleted.AddListener(() => CompleteTaskHandler(task));
            }
        }

        #endregion

        #region Public Accessors and Methods

        public List<Task> Tasks => _tasks;

        public Task CurrentTask => _currentTaskIndex >= 0 && _currentTaskIndex < _tasks.Count ? _tasks[_currentTaskIndex] : null;

        public void StartPool()
        {
            if (status != PoolStatus.Idle) return;

            status = PoolStatus.Started;

            // if the pool is sequential, start the first task
            if (type == PoolType.Sequential)
            {
                if (_tasks.Count > 0)
                {
                    _currentTaskIndex = 0;

                    _tasks[_currentTaskIndex].StartTask();
                }
            }
            else if (type == PoolType.Unordered) // start all tasks if undorderd
            {
                foreach (Task task in _tasks)
                    task.StartTask();
            }

            OnStartPool?.Invoke();
        }

        #endregion

        #region Private Methods

        void StartTaskHandler(Task task)
        {
            OnStartTask?.Invoke();
        }

        void CompleteTaskHandler(Task task)
        {
            OnFinishTask?.Invoke();

            if (type == PoolType.Sequential && task == CurrentTask)
            {
                _currentTaskIndex++;

                if (_currentTaskIndex < _tasks.Count)
                    _tasks[_currentTaskIndex].StartTask();
            }

            UpdateStatus();
        }

        void UpdateStatus()
        {
            switch (status)
            {
                case PoolStatus.Idle:
                    break;
                case PoolStatus.Started:
                    if (_tasks != null && _tasks.All(task => task.IsCompleted))
                    {
                        status = PoolStatus.Finished;
                        OnFinishPool?.Invoke();
                    }
                    break;
                case PoolStatus.Finished:
                    break;
            }
        }

        #endregion
    }

}
