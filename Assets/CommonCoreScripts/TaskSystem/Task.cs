using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine;

namespace CommonCoreScripts.TaskSystem
{
    public class Task : SerializedMonoBehaviour
    {
        public TaskPool ParentPool;
        
        public UnityEvent OnTaskCompleted;
        public UnityEvent OnTaskStarted;
        
        public void StartTask()
        {
            if (ParentPool == null || ParentPool.status != TaskPool.PoolStatus.Started) return;
            
            OnTaskStarted.Invoke();
        }
        
        public void CompleteTask()
        {
            if (ParentPool == null || ParentPool.status != TaskPool.PoolStatus.Started) return;
            
            ParentPool.CompleteTask(this);
            OnTaskCompleted.Invoke();
        }

        public void Message(string msg)
        {
            Debug.Log(msg);
        }
    }
}