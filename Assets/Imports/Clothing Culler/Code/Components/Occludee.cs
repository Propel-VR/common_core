using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEngine;

namespace Salvage.ClothingCuller.Components
{
    public class Occludee : MonoBehaviour
    {
        public SkinnedMeshRenderer[] SkinnedMeshRenderers { get; private set; }

        private CullableLOD[] cullableLODs;

        #region SerializeFields

        [field: SerializeField]
        public OccludeeConfiguration Config { get; private set; }

        #endregion

        #region MonoBehaviour Methods

        private void Awake()
        {
            SkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            cullableLODs = createCullableLODS();
        }

        #endregion

        #region Private Methods

        private CullableLOD[] createCullableLODS()
        {
            if (Config == null)
            {
                Debug.LogError($"Unable to create cullable LODs for '{name}' - configuration is null.", this);
                return null;
            }

            if (Config.LodOcclusionData == null)
            {
                Debug.LogError($"Unable to create cullable LODs for '{name}' - occlusion data is null, did you forget to generate it?", this);
                return null;
            }

            Renderer[][] renderersByLOD = GetRenderersByLOD();
            if (renderersByLOD.Length != Config.LodOcclusionData.Length)
            {
                Debug.LogError($"Unable to create cullable LODs for '{name}' - amount of LODs does not match the amount of LODs in occlusion data.", this);
                return null;
            }

            var cullableLODs = new CullableLOD[renderersByLOD.Length];
            for (int i = 0; i < renderersByLOD.Length; i++)
            {
                var cullableLOD = CullableLOD.Create(renderersByLOD[i], Config.LodOcclusionData[i]);
                if (cullableLOD == null)
                {
                    return null;
                }

                cullableLODs[i] = cullableLOD;
            }

            return cullableLODs;
        }

        #endregion

        #region Public Methods

#if UNITY_EDITOR

        public void SetConfiguration(OccludeeConfiguration targetConfiguration)
        {
            Config = targetConfiguration;
        }

#endif

        public Renderer[][] GetRenderersByLOD()
        {
            LODGroup lodGroup = GetComponent<LODGroup>();
            if (lodGroup != null)
            {
                LOD[] lods = lodGroup.GetLODs();
                var renderersByLODGroup = new Renderer[lods.Length][];

                for (int i = 0; i < lods.Length; i++)
                {
                    renderersByLODGroup[i] = lods[i].renderers;
                }

                return renderersByLODGroup;
            }

            return new Renderer[1][] { GetComponentsInChildren<Renderer>() };
        }

        public bool? TryCull(Occludee occluder)
        {
            #region Defense

            if (Config == null)
            {
                Debug.LogError($"Unable to cull '{name}' - {nameof(Config)} is null.", this);
                return false;
            }

            if (cullableLODs == null)
            {
                Debug.LogError($"Unable to cull '{name}' - {nameof(cullableLODs)} is null.", this);
                return false;
            }

            if (occluder == null)
            {
                Debug.LogError($"Unable to cull '{name}' - given {nameof(occluder)} is null.", this);
                return false;
            }

            if (occluder.Config == null)
            {
                Debug.LogError($"Unable to cull '{name}' - given {nameof(occluder.Config)} is null.", this);
                return false;
            }

            if (occluder.cullableLODs == null)
            {
                Debug.LogError($"Unable to cull '{name}' - given {nameof(occluder.cullableLODs)} is null.", this);
                return false;
            }

            #endregion

            if (!Config.Occluders.Contains(occluder.Config))
            {
                return null;
            }

            for (int i = 0; i < cullableLODs.Length; i++)
            {
                if (!cullableLODs[i].TryCull(occluder.cullableLODs.ElementAtOrLast(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public bool? TryUncull(Occludee occluder)
        {
            #region Defense

            if (Config == null)
            {
                Debug.LogError($"Unable to uncull '{name}' - {nameof(Config)} is null.", this);
                return false;
            }

            if (cullableLODs == null)
            {
                Debug.LogError($"Unable to uncull '{name}' - {nameof(cullableLODs)} is null.", this);
                return false;
            }

            if (occluder == null)
            {
                Debug.LogError($"Unable to uncull '{name}' - given {nameof(occluder)} is null.", this);
                return false;
            }

            if (occluder.Config == null)
            {
                Debug.LogError($"Unable to uncull '{name}' - given {nameof(occluder.Config)} is null.", this);
                return false;
            }

            if (occluder.cullableLODs == null)
            {
                Debug.LogError($"Unable to cull '{name}' - given {nameof(occluder.cullableLODs)} is null.", this);
                return false;
            }

            #endregion

            if (!Config.Occluders.Contains(occluder.Config))
            {
                return null;
            }

            for (int i = 0; i < cullableLODs.Length; i++)
            {
                if (!cullableLODs[i].TryUncull(occluder.cullableLODs.ElementAtOrLast(i)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}