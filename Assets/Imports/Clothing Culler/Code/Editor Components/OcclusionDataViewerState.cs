#if UNITY_EDITOR
using Salvage.ClothingCuller.Configuration;
using UnityEngine;

namespace Salvage.ClothingCuller.EditorComponents
{
    public class OcclusionDataViewerState : MonoBehaviour
    {
        #region SerializeFields

        public OccludeeConfiguration Occludee;
        public OccludeeConfiguration Occluder;

        [HideInInspector]
        public bool IsDebugVisible;

        [HideInInspector]
        public int RaycastVisibilityOptions;

        [HideInInspector]
        public bool IsAdvancedVisible;

        [HideInInspector]
        public float PokeThroughRaycastDistance;

        [HideInInspector]
        public int AmountOfRaycastsPerVertex;

        [HideInInspector]
        public string PreviousScenePath;

        [HideInInspector]
        public Vector3 PreviousScenePivot;

        [HideInInspector]
        public Quaternion PreviousSceneRotation;

        [HideInInspector]
        public float PreviousSceneCameraDistance;

        [HideInInspector]
        public bool WasSceneViewLightingOn;

        [HideInInspector]
        public bool WasSkyboxOn;

        [HideInInspector]
        public Animator Animator;

        #endregion

        #region MonoBehaviour Methods



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods



        #endregion
    }
}
#endif