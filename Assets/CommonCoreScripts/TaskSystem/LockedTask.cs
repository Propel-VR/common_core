using UnityEngine;

namespace CommonCoreScripts.TaskSystem
{
    public class LockedTask : Task
    {
        public TaskUnlockHandler unlockHandler;
        
        public void StartTask()
        {
            if (!unlockHandler.IsTaskLocked(this)) base.StartTask();
        }
        
        public void CompleteTask()
        {
            if (!unlockHandler.IsTaskLocked(this)) base.CompleteTask();
        }
    }
}