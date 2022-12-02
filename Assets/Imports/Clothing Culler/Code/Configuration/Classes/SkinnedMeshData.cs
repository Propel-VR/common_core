using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Salvage.ClothingCuller.Configuration
{
    [Serializable]
    public class SkinnedMeshData
    {
        #region SerializeFields

        [field: SerializeField]
        public bool IsInitialized { get; private set; }

        [field: SerializeField]
        public string RootBoneName { get; private set; }

        [field: SerializeField]
        public string[] BoneNames { get; private set; }

#if UNITY_EDITOR

        [SerializeField]
        private Transform originalRootBone;

        [SerializeField]
        private Transform[] originalBones;

#endif

        #endregion

        #region Constructor

        private SkinnedMeshData()
        {

        }

#if UNITY_EDITOR

        private SkinnedMeshData(SkinnedMeshRenderer originalSkinnedMeshRenderer)
        {
            IsInitialized = true;

            if (originalSkinnedMeshRenderer.rootBone != null)
            {
                RootBoneName = originalSkinnedMeshRenderer.rootBone.name;
            }
            BoneNames = getBoneNames(originalSkinnedMeshRenderer);

            originalRootBone = originalSkinnedMeshRenderer.rootBone;
            originalBones = originalSkinnedMeshRenderer.bones;
        }

#endif

        #endregion

        #region Private Methods

#if UNITY_EDITOR

        private string[] getBoneNames(SkinnedMeshRenderer originalSkinnedMeshRenderer)
        {
            string[] boneNames = new string[originalSkinnedMeshRenderer.bones.Length];

            for (int i = 0; i < originalSkinnedMeshRenderer.bones.Length; i++)
            {
                Transform bone = originalSkinnedMeshRenderer.bones[i];
                if (bone != null)
                {
                    boneNames[i] = bone.name;
                }
            }

            return boneNames;
        }

#endif

        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public static SkinnedMeshData Create(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            #region Defense

            if (skinnedMeshRenderer.sharedMesh == null)
            {
                Debug.LogError($"Unable to create {nameof(SkinnedMeshData)} - {nameof(SkinnedMeshRenderer)} does not have a sharedMesh.");
                return null;
            }

            GameObject modelGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh));
            if (modelGameObject == null)
            {
                return new SkinnedMeshData(skinnedMeshRenderer);
            }

            SkinnedMeshRenderer originalSkinnedMeshRenderer = modelGameObject.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(x => x.sharedMesh == skinnedMeshRenderer.sharedMesh);
            if (originalSkinnedMeshRenderer == null)
            {
                Debug.LogError($"Unable to create {nameof(SkinnedMeshData)} - original model does not contain a SkinnedMeshRenderer with the same sharedMesh.");
                return null;
            }

            #endregion

            return new SkinnedMeshData(originalSkinnedMeshRenderer);
        }

        public bool HasChanged(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            GameObject modelGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh));
            if (modelGameObject == null)
            {
                return true;
            }

            SkinnedMeshRenderer originalSkinnedMeshRenderer = modelGameObject.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(x => x.sharedMesh == skinnedMeshRenderer.sharedMesh);
            if (originalSkinnedMeshRenderer == null)
            {
                return true;
            }

            if (originalSkinnedMeshRenderer.rootBone != originalRootBone)
            {
                return true;
            }

            if (originalSkinnedMeshRenderer.bones.Length != originalBones.Length)
            {
                return true;
            }

            for (int i = 0; i < originalSkinnedMeshRenderer.bones.Length; i++)
            {
                if (originalSkinnedMeshRenderer.bones[i] != originalBones[i])
                {
                    return true;
                }
            }

            return false;
        }

#endif

        public void ResolveBones(SkinnedMeshRenderer sourceSkinnedMeshRenderer, Transform rigRootTransform)
        {
            var resolvedBones = new Transform[BoneNames.Length];
            bool hasResolvedAnyBones = false;

            for (int i = 0; i < BoneNames.Length; i++)
            {
                string boneName = BoneNames[i];

                if (!string.IsNullOrEmpty(boneName))
                {
                    Transform resolvedBone = FindBoneInChildren(rigRootTransform, boneName);
                    if (resolvedBone == null)
                    {
                        continue;
                    }

                    resolvedBones[i] = resolvedBone;
                    hasResolvedAnyBones = true;
                }
            }

            if (!hasResolvedAnyBones)
            {
                Debug.LogError($"Unable to resolve bones for '{sourceSkinnedMeshRenderer.name}' - {nameof(SkinnedMeshData)} contains no data.");
                return;
            }

            sourceSkinnedMeshRenderer.bones = resolvedBones;

            if (!string.IsNullOrEmpty(RootBoneName))
            {
                Transform resolvedBone = FindBoneInChildren(rigRootTransform, RootBoneName);
                if (resolvedBone == null)
                {
                    sourceSkinnedMeshRenderer.updateWhenOffscreen = true;
                    return;
                }

                sourceSkinnedMeshRenderer.rootBone = resolvedBone;
                return;
            }

            sourceSkinnedMeshRenderer.updateWhenOffscreen = true;
        }

        public Transform FindBoneInChildren(Transform transform, string boneName)
        {
            if (transform.name == boneName)
            {
                return transform;
            }

            foreach (Transform child in transform)
            {
                if (child.name == boneName)
                {
                    return child;
                }

                Transform bone = FindBoneInChildren(child, boneName);
                if (bone != null)
                {
                    return bone;
                }
            }

            return null;
        }

        #endregion

    }
}
