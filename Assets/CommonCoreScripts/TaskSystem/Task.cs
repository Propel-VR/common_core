using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Events;
using UnityEngine;

namespace CommonCoreScripts.TaskSystem
{
    public class Task : SerializedMonoBehaviour
    {
        [Required] [OdinSerialize] public string Name { get; set; }
        [Required] [OdinSerialize] public bool IsOptional { get; set; }
        
        public TaskPool ParentPool;
        public UnityEvent OnTaskCompleted;
        public UnityEvent OnTaskStarted;

        public void StartTask()
        {
            if (ParentPool == null || ParentPool.Status != TaskPool.PoolStatus.Started) return;
            
            OnTaskStarted.Invoke();
            ParentPool.OnStartTask.Invoke();
        }
        
        public void CompleteTask()
        {
            if (ParentPool == null || ParentPool.Status != TaskPool.PoolStatus.Started) return;
            
            ParentPool.CompleteTask(this);
            OnTaskCompleted.Invoke();
        }

        public void Message(string msg)
        {
            Debug.Log(msg);
        }
    }
}