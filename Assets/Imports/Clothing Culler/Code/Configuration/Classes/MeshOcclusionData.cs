using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Salvage.ClothingCuller.ExtensionMethods;
using Salvage.ClothingCuller.Serialization;
using UnityEngine;

namespace Salvage.ClothingCuller.Configuration
{
    [Serializable]
    public class MeshOcclusionData
    {
#if UNITY_EDITOR
        [SerializeField]
        private Mesh originalSharedMesh;
#endif

        #region SerializeFields

        [SerializeField]
        private TriangleOcclusionDataByLodId occludedTrianglesByLODId;

        #endregion

        #region Constructor

        private MeshOcclusionData()
        {

        }

#if UNITY_EDITOR

        private MeshOcclusionData(Renderer renderer)
        {
            Mesh sharedMesh = renderer.GetSharedMesh();
            if (sharedMesh != null)
            {
                originalSharedMesh = sharedMesh;
            }

            occludedTrianglesByLODId = new TriangleOcclusionDataByLodId();
        }

#endif

        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public static MeshOcclusionData Create(Renderer renderer)
        {
            return new MeshOcclusionData(renderer);
        }

        public bool IsValid(Renderer renderer)
        {
            if (renderer == null)
            {
                Debug.LogError($"{nameof(MeshOcclusionData)} is invalid - given {nameof(renderer)} is null.");
                return false;
            }

            Mesh sharedMesh = renderer.GetSharedMesh();
            if (sharedMesh == null)
            {
                Debug.LogError($"{nameof(MeshOcclusionData)} is invalid - SharedMesh of given {nameof(renderer)} is null.");
                return false;
            }

            if (sharedMesh != originalSharedMesh)
            {
                Debug.LogError($"{nameof(MeshOcclusionData)} is invalid - given Mesh does not match the Mesh in serialized data.");
                return false;
            }

            return true;
        }

        public bool IsComplete(string lodId)
        {
            return occludedTrianglesByLODId.ContainsKey(lodId);
        }

        public bool RemoveUnusedOcclusionData(HashSet<string> usedLODOcclusionDataIds)
        {
            bool hasRemovedAnyData = false;

            foreach (string lodId in occludedTrianglesByLODId.Keys.ToArray())
            {
                if (!usedLODOcclusionDataIds.Contains(lodId))
                {
                    hasRemovedAnyData = true;

                    occludedTrianglesByLODId.Remove(lodId);
                }
            }

            return hasRemovedAnyData;
        }

        public bool[] GetOrCreateTriangleOcclusionData(string lodId)
        {
            if (occludedTrianglesByLODId.TryGetValue(lodId, out SerializableBoolArray serializableBoolArray))
            {
                return serializableBoolArray.Value;
            }

            serializableBoolArray = new SerializableBoolArray { Value = new bool[originalSharedMesh.triangles.Length] };
            occludedTrianglesByLODId.Add(lodId, serializableBoolArray);

            return serializableBoolArray.Value;
        }

#endif

        public bool TryGetTriangleOcclusionData(string lodId, out bool[] triangleOcclusionData)
        {
            if (occludedTrianglesByLODId.TryGetValue(lodId, out SerializableBoolArray serializableBoolArray))
            {
                triangleOcclusionData = serializableBoolArray.Value;
                return true;
            }

            triangleOcclusionData = null;
            return false;
        }

        #endregion

        #region Inner Classes

        [Serializable]
        public class TriangleOcclusionDataByLodId : SerializableDictionary<string, SerializableBoolArray>
        {

        }

        #endregion

    }
}
