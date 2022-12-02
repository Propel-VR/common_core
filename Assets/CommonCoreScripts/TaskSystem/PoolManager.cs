using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace CommonCoreScripts.TaskSystem
{
    public class PoolManager : SerializedMonoBehaviour
    {
        public static PoolManager Instance { get; set; }

        [OdinSerialize] private List<TaskPool> pools;
        public IEnumerable<TaskPool> RemainingPools => pools.Where(p => p.status != TaskPool.PoolStatus.Finished);

        // Singleton Awake
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void StartPool(TaskPool pool)
        {
            if (!pools.Contains(pool)) return;
            
            pool.StartPool();
        }
    }
}