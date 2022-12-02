using System.Linq;
using Salvage.ClothingCuller.Configuration;
using UnityEditor;
using UnityEngine;

namespace Salvage.ClothingCuller.Editor
{
    public class ViewableLOD
    {
        public Bounds Bounds { get; }
        public readonly LODOcclusionData LodOcclusionData;

        private readonly ViewableMesh[] viewableMeshes;

        #region Constructor

        private ViewableLOD(ViewableMesh[] viewableMeshes, LODOcclusionData lodOcclusionData)
        {
            Bounds = getViewableMeshesBounds(viewableMeshes);
            LodOcclusionData = lodOcclusionData;

            this.viewableMeshes = viewableMeshes;
        }

        #endregion

        #region Private Methods

        private static ViewableMesh[] createViewableMeshes(Renderer[] renderers, Transform targetParent, Transform rigRootTransform, LODOcclusionData lodOcclusionData, ViewableLOD occludingViewableLOD, Material material)
        {
            var viewableMeshes = new ViewableMesh[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                var cullableMesh = ViewableMesh.Create(renderers[i], targetParent, rigRootTransform, lodOcclusionData?.MeshOcclusionData.ElementAtOrDefault(i), occludingViewableLOD, material);
                if (cullableMesh == null)
                {
                    return null;
                }

                viewableMeshes[i] = cullableMesh;
            }

            return viewableMeshes;
        }

        private Bounds getViewableMeshesBounds(ViewableMesh[] viewableMeshes)
        {
            Bounds bounds = viewableMeshes[0].Bounds;

            for (int i = 1; i < viewableMeshes.Length; i++)
            {
                bounds.Encapsulate(viewableMeshes[i].Bounds);
            }

            return new Bounds(bounds.center, bounds.size * 1.1f);
        }

        #endregion

        #region Public Methods

        public static ViewableLOD Create(Transform targetParent, string name, Transform rigRootTransform, Renderer[] renderers, LODOcclusionData lodOcclusionData, ViewableLOD occludingViewableLOD, Material material)
        {
            #region Defense

            if (renderers == null)
            {
                Debug.LogError($"Unable to create {nameof(ViewableLOD)} - given {nameof(renderers)} is null.");
                return null;
            }

            if (renderers.Length == 0)
            {
                Debug.LogError($"Unable to create {nameof(ViewableLOD)} - given {nameof(renderers)} contains no elements.");
                return null;
            }

            if (material == null)
            {
                Debug.LogError($"Unable to create {nameof(ViewableMesh)} - given {nameof(material)} is null.");
                return null;
            }

            #endregion

            var newGameObject = new GameObject(name) { hideFlags = HideFlags.NotEditable };
            newGameObject.transform.SetParent(targetParent);

            ViewableMesh[] viewableMeshes = createViewableMeshes(renderers, newGameObject.transform, rigRootTransform, lodOcclusionData, occludingViewableLOD, material);
            if (viewableMeshes == null)
            {
                Object.DestroyImmediate(newGameObject);
                return null;
            }

            return new ViewableLOD(viewableMeshes, lodOcclusionData);
        }

        public void Activate(bool isActive)
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.Activate(isActive);
            }
        }

        public void EnableRenderers(bool isEnabled)
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.EnableRenderer(isEnabled);
            }
        }

        public void EnableMeshColliders(bool isEnabled)
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.EnableMeshCollider(isEnabled);
            }
        }

        public bool HasOcclusionData()
        {
            return viewableMeshes.All(x => x.HasOcclusionData());
        }

        public void FocusInSceneView()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

            Bounds? boundsToFrame = null;

            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                Bounds bounds = viewableMesh.Bounds;

                if (boundsToFrame == null)
                {
                    boundsToFrame = bounds;
                    continue;
                }

                boundsToFrame.Value.Encapsulate(bounds);
            }

            if (boundsToFrame.HasValue)
            {
                SceneView.lastActiveSceneView.Frame(boundsToFrame.Value);
            }
        }

        public ViewableMesh GetViewableMesh(MeshCollider meshCollider)
        {
            return viewableMeshes.First(x => x.MeshCollider == meshCollider);
        }

        public void GenerateOcclusionData(float pokeThroughRaycastDistance, int amountOfRaycastsPerVertex)
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.GenerateOcclusionData(pokeThroughRaycastDistance, amountOfRaycastsPerVertex);
            }
        }

        public void ClearOcclusionData()
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.ClearOcclusionData();
            }
        }

        public void DrawWireFrames(Color color)
        {
            foreach (ViewableMesh viewableMesh in viewableMeshes)
            {
                viewableMesh.DrawWireFrame(color);
            }
        }

        #endregion

    }
}
