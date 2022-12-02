using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace CommonCoreScripts.TaskSystem
{
    public class TaskUnlockHandler : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public HashSet<LockedTask> lockedTasks = new ();
        
        public bool TryUnlockTask(LockedTask task)
        {
            return lockedTasks.Remove(task);
        }

        public void UnlockTask(LockedTask task)
        {
            TryUnlockTask(task);
        }
        
        public void LockTask(LockedTask task)
        {
            lockedTasks.Add(task);
        }
        
        public bool IsTaskLocked(LockedTask task)
        {
            return lockedTasks.Contains(task);
        }
    }
}