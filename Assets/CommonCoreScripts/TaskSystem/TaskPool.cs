using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace CommonCoreScripts.TaskSystem
{
    public class TaskPool : SerializedMonoBehaviour
    {
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
        
        [SerializeField] private PoolType type;
        [SerializeField] public PoolStatus status;
        
        public UnityEvent OnStartPool;
        public UnityEvent OnFinishPool;
        public UnityEvent OnStartTask;
        public UnityEvent OnFinishTask;

        [Tooltip("Item 1 -> Task\nItem 2 -> Done?")]
        public List<(Task, bool)> _tasks = new ();

        public void StartPool()
        {
            // if the pool is already started or finished, do nothing
            if (status != PoolStatus.Idle) return;
            
            // if the pool is sequential, start the first task
            if (type == PoolType.Sequential)
            {
                if (_tasks.FirstOrDefault() == (null, false) ) return;
                
                var task = _tasks.FirstOrDefault().Item1;
                
                task.StartTask();
                OnStartTask?.Invoke();
            }

            OnStartPool?.Invoke();
            
            status = PoolStatus.Started;
            
            UpdateStatus();
        }

        public void CompleteTask(Task task)
        {
            if (status != PoolStatus.Started) return;
            var taskIndex = _tasks.FindIndex(t => t.Item1 == task);
            if (taskIndex == -1) return;
            _tasks[taskIndex] = (task, true);
            OnFinishTask?.Invoke();
            
            if (type == PoolType.Sequential && _tasks[taskIndex] != _tasks.Last())
                _tasks[taskIndex + 1].Item1.StartTask();
            
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            switch (status)
            {
                case PoolStatus.Idle:
                    break;
                case PoolStatus.Started:
                    if (_tasks != null && _tasks.All(task => task.Item2))
                    {
                        status = PoolStatus.Finished;
                        OnFinishPool?.Invoke();
                    }
                    break;
                case PoolStatus.Finished:
                    break;
            }
        }
    }
}