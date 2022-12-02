using UnityEngine;

namespace Salvage.ClothingCuller.ExtensionMethods
{
    public static class RendererExtensions
    {

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static Mesh GetSharedMesh(this Renderer renderer)
        {
            if (renderer == null)
            {
                return null;
            }

            switch (renderer)
            {
                case MeshRenderer meshRenderer:
                    MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter == null)
                    {
                        Debug.LogError($"Unable to get mesh from '{renderer.name}' as it doesn't have a {nameof(MeshFilter)} component.");
                        return null;
                    }

                    return meshFilter.sharedMesh;
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    return skinnedMeshRenderer.sharedMesh;
                default:
                    Debug.LogError($"Unable to get mesh for {nameof(Renderer)} of type '{renderer.GetType().Name}'.");
                    return null;
            }
        }

        public static void SetSharedMesh(this Renderer renderer, Mesh mesh)
        {
            if (renderer == null)
            {
                return;
            }

            switch (renderer)
            {
                case MeshRenderer meshRenderer:
                    MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter == null)
                    {
                        Debug.LogError($"Unable to set mesh on '{renderer.name}' as it doesn't have a {nameof(MeshFilter)} component.");
                        return;
                    }

                    meshFilter.sharedMesh = mesh;
                    break;
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    if (skinnedMeshRenderer == null)
                    {
                        return;
                    }

                    skinnedMeshRenderer.sharedMesh = mesh;
                    break;
                default:
                    Debug.LogError($"Unable to set mesh for {nameof(Renderer)} of type '{renderer.GetType().Name}'.");
                    break;
            }
        }

        #endregion

    }
}
