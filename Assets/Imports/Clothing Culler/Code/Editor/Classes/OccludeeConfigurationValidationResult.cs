using System;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.Serialization;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor
{
    [Serializable]
    public class OccludeeConfigurationValidationResult
    {
        #region SerializeFields

        [field: SerializeField]
        public bool IsOcclusionDataValid { get; private set; }

        [SerializeField]
        private IsOcclusionDataCompleteByOccluder validationResultsByOccluder;

        #endregion

        #region Constructor

        private OccludeeConfigurationValidationResult(OccludeeConfiguration occludee)
        {
            IsOcclusionDataValid = occludee.Prefab != null && occludee.IsOcclusionDataValid();

            validationResultsByOccluder = createValidationResultByOccluder(occludee);
        }

        #endregion

        #region Private Methods

        private IsOcclusionDataCompleteByOccluder createValidationResultByOccluder(OccludeeConfiguration occludee)
        {
            var validationResultByOccluder = new IsOcclusionDataCompleteByOccluder();

            foreach (OccludeeConfiguration occluder in occludee.Occluders)
            {
                validationResultByOccluder.Add(occluder, occludee.IsOcclusionDataComplete(occluder));
            }

            return validationResultByOccluder;
        }

        #endregion

        #region Public Methods

        public static OccludeeConfigurationValidationResult Create(OccludeeConfiguration occludee)
        {
            #region Defense

            if (occludee == null)
            {
                Debug.LogError($"Unable to create {nameof(OccludeeConfigurationValidationResult)} - given {nameof(occludee)} is null.");
                return null;
            }

            #endregion

            return new OccludeeConfigurationValidationResult(occludee);
        }

        public bool? IsOcclusionDataComplete(OccludeeConfiguration occluder)
        {
            if (validationResultsByOccluder.TryGetValue(occluder, out bool isComplete))
            {
                return isComplete;
            }

            return null;
        }

        #endregion

        #region Inner Classes

        [Serializable]
        public class IsOcclusionDataCompleteByOccluder : SerializableDictionary<OccludeeConfiguration, bool>
        {

        }

        #endregion
    }
}
