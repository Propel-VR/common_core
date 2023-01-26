using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace CommonCoreScripts.TaskSystem
{
    public class PoolManager : SerializedMonoBehaviour
    {
        private static PoolManager Instance { get; set; }

        [OdinSerialize] private List<TaskPool> _pools;
        public IEnumerable<TaskPool> RemainingPools => _pools.Where(p => p.Status != TaskPool.PoolStatus.Finished);

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
            if (!_pools.Contains(pool)) return;
            
            pool.StartPool();
        }
    }
}