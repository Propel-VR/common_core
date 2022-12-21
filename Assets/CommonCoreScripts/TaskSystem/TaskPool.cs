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
        
        [OdinSerialize] public PoolType Type { get; set; }
        [OdinSerialize] public PoolStatus Status { get; set; }

        public UnityEvent OnStartPool;
        public UnityEvent OnFinishPool;
        public UnityEvent OnStartTask;
        public UnityEvent OnFinishTask;

        [Tooltip("Item 1 -> Task\nItem 2 -> Done?")] 
        [OdinSerialize] private List<(Task, bool)> tasks = new ();

        public void StartPool()
        {
            // if the pool is already started or finished, do nothing
            if (Status != PoolStatus.Idle) return;
            
            // if the pool is sequential, start the first task
            if (Type == PoolType.Sequential)
            {
                if (tasks.FirstOrDefault() == (null, false) ) return;
                
                var task = tasks.FirstOrDefault().Item1;
                
                task.StartTask();
                OnStartTask?.Invoke();
            }

            OnStartPool?.Invoke();
            
            Status = PoolStatus.Started;
            
            UpdateStatus();
        }

        public void CompleteTask(Task task)
        {
            if (Status != PoolStatus.Started) return;
            var taskIndex = tasks.FindIndex(t => t.Item1 == task);
            if (taskIndex == -1) return;
            tasks[taskIndex] = (task, true);
            OnFinishTask?.Invoke();
            
            if (Type == PoolType.Sequential && tasks[taskIndex] != tasks.Last())
                tasks[taskIndex + 1].Item1.StartTask();
            
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            switch (Status)
            {
                case PoolStatus.Idle:
                    break;
                case PoolStatus.Started:
                    if (tasks != null && tasks.All(task => task.Item1.IsOptional || task.Item2)) // if the tasks collection is not empty and all tasks are either done or optional
                    {
                        Status = PoolStatus.Finished;
                        OnFinishPool?.Invoke();
                    }
                    break;
                case PoolStatus.Finished:
                    break;
            }
        }

        public Task FindTask(string taskName)
        {
            return tasks.FirstOrDefault(t => t.Item1.name == taskName).Item1;
        }
    }
}