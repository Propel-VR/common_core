using System;
using System.Collections.Generic;
using UnityEngine;

namespace Salvage.ClothingCuller.Configuration
{
    [Serializable]
    public class LODOcclusionData
    {
        #region SerializeFields

        [field: SerializeField]
        public MeshOcclusionData[] MeshOcclusionData { get; private set; }

        [field: SerializeField]
        public string Id { get; private set; }

        #endregion

        #region Constructor

        private LODOcclusionData()
        {

        }

#if UNITY_EDITOR

        private LODOcclusionData(Renderer[] renderers)
        {
            MeshOcclusionData = createMeshOcclusionData(renderers);
            Id = Guid.NewGuid().ToString();
        }

#endif

        #endregion

        #region Private Methods

#if UNITY_EDITOR

        private static MeshOcclusionData[] createMeshOcclusionData(Renderer[] renderers)
        {
            var meshOcclusionData = new MeshOcclusionData[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                meshOcclusionData[i] = Configuration.MeshOcclusionData.Create(renderers[i]);
            }

            return meshOcclusionData;
        }

#endif

        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public static LODOcclusionData Create(Renderer[] renderers)
        {
            return new LODOcclusionData(renderers);
        }

        public bool IsValid(Renderer[] renderers)
        {
            if (renderers.Length != MeshOcclusionData.Length)
            {
                Debug.LogError($"{nameof(LODOcclusionData)} is invalid - amount of renderers does not match the amount of renderers in serialized data.");
                return false;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                if (!MeshOcclusionData[i].IsValid(renderers[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsComplete(LODOcclusionData occluderLODOcclusionData)
        {
            foreach (MeshOcclusionData meshOcclusionData in MeshOcclusionData)
            {
                if (!meshOcclusionData.IsComplete(occluderLODOcclusionData.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public bool RemoveUnusedOcclusionData(HashSet<string> usedLODOcclusionDataIds)
        {
            bool hasRemovedAnyData = false;

            foreach (MeshOcclusionData meshOcclusionData in MeshOcclusionData)
            {
                if (meshOcclusionData.RemoveUnusedOcclusionData(usedLODOcclusionDataIds))
                {
                    hasRemovedAnyData = true;
                }
            }

            return hasRemovedAnyData;
        }

#endif
        #endregion

    }
}
