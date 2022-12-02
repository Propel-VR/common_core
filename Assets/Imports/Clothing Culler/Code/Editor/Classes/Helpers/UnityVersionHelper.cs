using UnityEditor;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor
{
    public static class UnityVersionHelper
    {

        #region Constructor



        #endregion

        #region Private Methods



        #endregion

        #region Public Methods

        public static bool GetIsSceneLightingOn()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return true;
            }

#if UNITY_2019_1_OR_NEWER
            return SceneView.lastActiveSceneView.sceneLighting;
#else
            return SceneView.lastActiveSceneView.m_SceneLighting;
#endif
        }

        public static void SetIsSceneLightingOn(bool isSceneLightingOn)
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

#if UNITY_2019_1_OR_NEWER
            SceneView.lastActiveSceneView.sceneLighting = isSceneLightingOn;
#else
            SceneView.lastActiveSceneView.m_SceneLighting = isSceneLightingOn;            
#endif
        }

        public static void BakeMesh(SkinnedMeshRenderer skinnedMeshRenderer, Mesh mesh, bool useScale)
        {
#if UNITY_2020_2_OR_NEWER
            skinnedMeshRenderer.BakeMesh(mesh, useScale);
#else
            skinnedMeshRenderer.BakeMesh(mesh);

            if (useScale && skinnedMeshRenderer.transform.localScale != Vector3.one)
            {
                Vector3[] vertices = mesh.vertices;

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 scaledVertex = vertices[i];
                    scaledVertex.x = scaledVertex.x * 1 / skinnedMeshRenderer.transform.localScale.x;
                    scaledVertex.y = scaledVertex.y * 1 / skinnedMeshRenderer.transform.localScale.y;
                    scaledVertex.z = scaledVertex.z * 1 / skinnedMeshRenderer.transform.localScale.z;

                    vertices[i] = scaledVertex;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }
#endif
        }

        #endregion

    }
}
