using System.Collections.Generic;
using System.IO;
using Salvage.ClothingCuller.Components;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEngine;

namespace Salvage.ClothingCuller.Configuration
{
    public class OccludeeConfiguration : ScriptableObject
    {
        #region SerializeFields

        [field: SerializeField]
        public Occludee Prefab { get; private set; }

        [field: SerializeField]
        public List<OccludeeConfiguration> Occluders { get; private set; }

        [field: SerializeField, HideInInspector]
        public LODOcclusionData[] LodOcclusionData { get; private set; }

        [field: SerializeField, HideInInspector]
        public SkinnedMeshData SkinnedMeshData { get; private set; }

        #endregion

        #region Constructor

        private OccludeeConfiguration()
        {

        }

        #endregion

        #region ScriptableObject Methods



        #endregion

        #region Private Methods

#if UNITY_EDITOR

        private static LODOcclusionData[] createLODOcclusionData(Occludee occludee)
        {
            Renderer[][] renderersByLOD = occludee.GetRenderersByLOD();
            var lodOcclusionData = new LODOcclusionData[renderersByLOD.Length];

            for (int i = 0; i < renderersByLOD.Length; i++)
            {
                lodOcclusionData[i] = LODOcclusionData.Create(renderersByLOD[i]);
            }

            return lodOcclusionData;
        }

#endif

        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public static OccludeeConfiguration Create(Occludee prefab)
        {
            OccludeeConfiguration instance = CreateInstance<OccludeeConfiguration>();
            instance.Occluders = new List<OccludeeConfiguration>();
            instance.Prefab = prefab;
            instance.LodOcclusionData = createLODOcclusionData(prefab);

            return instance;
        }

        public void SaveToDisk(string targetFolderPath)
        {
            Directory.CreateDirectory(targetFolderPath);

            string targetAssetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"{targetFolderPath}/{Prefab.name}.asset");
            UnityEditor.AssetDatabase.CreateAsset(this, targetAssetPath);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        public bool IsOcclusionDataValid()
        {
            if (LodOcclusionData == null)
            {
                return true;
            }

            Renderer[][] renderersByLOD = Prefab.GetRenderersByLOD();
            if (renderersByLOD.Length != LodOcclusionData.Length)
            {
                Debug.LogError($"Occlusion Data is invalid invalid - amount of LODs does not match the amount of LODs in serialized data.", this);
                return false;
            }

            for (int i = 0; i < renderersByLOD.Length; i++)
            {
                if (!LodOcclusionData[i].IsValid(renderersByLOD[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOcclusionDataComplete(OccludeeConfiguration occluder)
        {
            if (occluder == null)
            {
                Debug.LogError($"Occlusion Data is incomplete - given {nameof(occluder)} is null.", this);
                return false;
            }

            for (int i = 0; i < LodOcclusionData.Length; i++)
            {
                if (!LodOcclusionData[i].IsComplete(occluder.LodOcclusionData.ElementAtOrLast(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public void CreateNewLODOcclusionData()
        {
            #region Defense

            if (Prefab == null)
            {
                Debug.LogError($"Unable to generate occlusion data for '{name}' - prefab is null.");
                return;
            }

            #endregion

            LodOcclusionData = createLODOcclusionData(Prefab);
        }

        public bool RemoveUnusedLODOcclusionData()
        {
            bool hasRemoved = false;

            var usedLODOcclusionDataHashCodes = new HashSet<string>();

            foreach (OccludeeConfiguration occluder in Occluders)
            {
                foreach (LODOcclusionData occlusionData in occluder.LodOcclusionData)
                {
                    usedLODOcclusionDataHashCodes.Add(occlusionData.Id);
                }
            }

            foreach (LODOcclusionData lodOcclusionData in LodOcclusionData)
            {
                if (lodOcclusionData.RemoveUnusedOcclusionData(usedLODOcclusionDataHashCodes))
                {
                    hasRemoved = true;
                }
            }

            return hasRemoved;
        }

        public void CreateSkinnedMeshData()
        {
            #region Defense

            if (Prefab == null)
            {
                return;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = Prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                return;
            }

            #endregion

            SkinnedMeshData = SkinnedMeshData.Create(skinnedMeshRenderer);
        }

        public void UpdateSkinnedMeshDataIfChanged()
        {
            #region Defense

            if (Prefab == null)
            {
                return;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = Prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                return;
            }

            #endregion

            if (SkinnedMeshData == null || SkinnedMeshData.HasChanged(skinnedMeshRenderer))
            {
                SkinnedMeshData = SkinnedMeshData.Create(skinnedMeshRenderer);
            }
        }

        public void ClearSkinnedMeshData()
        {
            SkinnedMeshData = null;
        }

#endif

        #endregion

    }
}
