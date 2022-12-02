using System.Collections.Generic;
using Salvage.ClothingCuller.Configuration;
using UnityEngine;

namespace Salvage.ClothingCuller.Components
{
    public class ClothingCuller : MonoBehaviour
    {
        private HashSet<Occludee> registeredOccludees;

        #region SerializeFields

        [SerializeField]
        private ClothingCullerConfiguration config;

        [SerializeField]
        private bool modularClothingWorkflow;

        [SerializeField]
        private Transform rigRoot;

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            registeredOccludees = new HashSet<Occludee>();
        }

        private void Start()
        {
            validate();
        }

#if UNITY_EDITOR

        private void Reset()
        {
            config = ClothingCullerConfiguration.Find();
            rigRoot = tryFindRigRoot();
        }

#endif

        #endregion

        #region Private Methods

        private void validate()
        {
            if (!modularClothingWorkflow)
            {
                return;
            }

            if (config == null)
            {
                Debug.LogWarning($"Modular clothing workflow requires the {nameof(config)} field to be assigned.");
            }
            else if (!config.IsModularClothingWorkflowEnabled)
            {
                Debug.LogWarning($"Modular clothing workflow requires the global setting to be enabled from the editor preferences.");
            }

            if (rigRoot == null)
            {
                Debug.LogWarning($"Modular clothing workflow requires the {nameof(rigRoot)} field to be assigned.");
            }
        }

        private void resolveBones(Occludee occludee)
        {
            #region Defense

            if (!occludee.Config.SkinnedMeshData.IsInitialized)
            {
                Debug.LogError($"Unable to resolve bones for '{occludee.name}' - config contains no {nameof(SkinnedMeshData)}.", this);
                return;
            }

            #endregion

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in occludee.SkinnedMeshRenderers)
            {
                occludee.Config.SkinnedMeshData.ResolveBones(skinnedMeshRenderer, rigRoot);
            }
        }

        private void clearBones(Occludee occludee)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in occludee.SkinnedMeshRenderers)
            {
                skinnedMeshRenderer.rootBone = null;
                skinnedMeshRenderer.bones = null;
            }
        }

        private Transform tryFindRigRoot()
        {
            Animator animator = GetComponentInParent<Animator>();
            if (animator == null)
            {
                return null;
            }

            Transform hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (hipsBone == null)
            {
                return null;
            }

            Transform rigRoot = hipsBone;

            while (rigRoot.parent != null && rigRoot.parent != animator.transform)
            {
                rigRoot = rigRoot.parent;
            }

            return rigRoot;
        }

        #endregion

        #region Public Methods

        public void Register(Occludee occludeeToRegister, bool isModular = true)
        {
            #region Defense

            if (occludeeToRegister == null)
            {
                Debug.LogError($"Unable to register - given {nameof(occludeeToRegister)} is null.", this);
                return;
            }

            if (occludeeToRegister.Config == null)
            {
                Debug.LogError($"Unable to register - given Occludee's config is null.", this);
                return;
            }

            if (registeredOccludees.Contains(occludeeToRegister))
            {
                Debug.LogWarning($"Unable to register '{occludeeToRegister.name}' - Occludee has already been registered.", this);
                return;
            }

            #endregion

            if (isModular && modularClothingWorkflow && rigRoot != null && config != null && config.IsModularClothingWorkflowEnabled)
            {
                resolveBones(occludeeToRegister);
            }

            foreach (Occludee registeredOccludee in registeredOccludees)
            {
                bool? isSuccessful = registeredOccludee.TryCull(occludeeToRegister);
                if (isSuccessful.HasValue && !isSuccessful.Value)
                {
                    return;
                }

                isSuccessful = occludeeToRegister.TryCull(registeredOccludee);
                if (isSuccessful.HasValue && !isSuccessful.Value)
                {
                    return;
                }
            }

            registeredOccludees.Add(occludeeToRegister);
        }

        public void Deregister(Occludee occludeeToDeregister)
        {
            #region Defense

            if (occludeeToDeregister == null)
            {
                Debug.LogError($"Unable to deregister - given {nameof(occludeeToDeregister)} is null.", this);
                return;
            }

            if (!registeredOccludees.Remove(occludeeToDeregister))
            {
                Debug.LogWarning($"Unable to deregister '{occludeeToDeregister.name}' - Occludee is not registered.", this);
                return;
            }

            #endregion

            if (modularClothingWorkflow && rigRoot != null && config != null && config.IsModularClothingWorkflowEnabled)
            {
                clearBones(occludeeToDeregister);
            }

            foreach (Occludee registeredOccludee in registeredOccludees)
            {
                if (registeredOccludee == occludeeToDeregister)
                {
                    continue;
                }

                bool? isSuccessful = registeredOccludee.TryUncull(occludeeToDeregister);
                if (isSuccessful.HasValue && !isSuccessful.Value)
                {
                    return;
                }

                isSuccessful = occludeeToDeregister.TryUncull(registeredOccludee);
                if (isSuccessful.HasValue && !isSuccessful.Value)
                {
                    return;
                }
            }

            registeredOccludees.Remove(occludeeToDeregister);
        }

        #endregion
    }
}