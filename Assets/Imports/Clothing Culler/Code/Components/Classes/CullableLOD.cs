using Salvage.ClothingCuller.Configuration;
using UnityEngine;

namespace Salvage.ClothingCuller.Components
{
    public class CullableLOD
    {
        public readonly LODOcclusionData LodOcclusionData;

        private readonly CullableMesh[] cullableMeshes;

        #region Constructor

        private CullableLOD(LODOcclusionData lodOcclusionData, CullableMesh[] cullableMeshes)
        {
            LodOcclusionData = lodOcclusionData;

            this.cullableMeshes = cullableMeshes;
        }

        #endregion

        #region Private Methods

        private static CullableMesh[] createCullableMeshes(Renderer[] renderers, LODOcclusionData lodOcclusionData)
        {
            var cullableMeshes = new CullableMesh[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                var cullableMesh = CullableMesh.Create(renderers[i], lodOcclusionData.MeshOcclusionData[i]);
                if (cullableMesh == null)
                {
                    return null;
                }

                cullableMeshes[i] = cullableMesh;
            }

            return cullableMeshes;
        }

        #endregion

        #region Public Methods

        public static CullableLOD Create(Renderer[] renderers, LODOcclusionData lodOcclusionData)
        {
            #region Defense

            if (renderers == null)
            {
                Debug.LogError($"Unable to create {nameof(CullableLOD)} - given {nameof(renderers)} is null.");
                return null;
            }

            if (lodOcclusionData == null)
            {
                Debug.LogError($"Unable to create {nameof(CullableLOD)} - given {nameof(lodOcclusionData)} is null.");
                return null;
            }

            if (renderers.Length != lodOcclusionData.MeshOcclusionData.Length)
            {
                Debug.LogError($"Unable to create {nameof(CullableLOD)} - given amount of renderers does not match the amount of meshes in serialized data.");
                return null;
            }

            #endregion

            CullableMesh[] cullableMeshes = createCullableMeshes(renderers, lodOcclusionData);
            if (cullableMeshes == null)
            {
                return null;
            }

            return new CullableLOD(lodOcclusionData, cullableMeshes);
        }

        public bool TryCull(CullableLOD occluderCullableLOD)
        {
            foreach (CullableMesh occludeeCullableMesh in cullableMeshes)
            {
                if (!occludeeCullableMesh.TryCull(occluderCullableLOD))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryUncull(CullableLOD occluderCullableLOD)
        {
            foreach (CullableMesh occludeeCullableMesh in cullableMeshes)
            {
                if (!occludeeCullableMesh.TryUncull(occluderCullableLOD))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
