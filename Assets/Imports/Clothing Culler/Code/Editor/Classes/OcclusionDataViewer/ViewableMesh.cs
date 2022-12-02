using System;
using System.Collections.Generic;
using System.Linq;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.Editor.Configuration;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Salvage.ClothingCuller.Editor
{
    public class ViewableMesh
    {
        public Bounds Bounds { get; }
        public MeshCollider MeshCollider { get; }
        public int? SelectedVertexHandleIndex { get; private set; }
        public int? SelectedTriangleIndex { get; set; }

        private readonly Renderer renderer;
        private readonly Vector3[] sharedMeshVertices;
        private readonly Vector3[] sharedMeshNormals;
        private readonly int[] sharedMeshTriangles;
        private readonly MeshOcclusionData meshOcclusionData;
        private readonly ViewableLOD occludingViewableLOD;
        private readonly Vector3[] wireFrameLineSegments;
        private Tuple<bool, Vector3> vertexHandleIsOccludingResult;
        private Tuple<bool, Vector3>[] vertexHandleIsOccludedResults;
        private Vector3[] raycastStartPositions;
        private bool cachedIsSelectedVertexHandleOccluded;

        #region Constructor

        private ViewableMesh(Renderer originalRenderer, Transform targetParent, Transform rigRootTransform, MeshOcclusionData meshOcclusionData, ViewableLOD occludingViewableLOD, Material material)
        {
            Mesh meshCopyWithAdditionalSubMesh = createMeshCopyWithAdditionalSubMesh(originalRenderer);

            renderer = createRendererCopy(originalRenderer, targetParent, rigRootTransform);
            renderer.SetSharedMesh(meshCopyWithAdditionalSubMesh);
            renderer.sharedMaterials = getSharedMaterialsForAdditionalSubMesh(meshCopyWithAdditionalSubMesh.subMeshCount, material, OcclusionDataViewerConfiguration.OccludedMaterial.Value);

            Mesh colliderMesh = getColliderMesh(renderer);

            Bounds = getLocalSpaceBounds(renderer, colliderMesh);
            MeshCollider = renderer.gameObject.AddComponent<MeshCollider>();
            MeshCollider.sharedMesh = colliderMesh;
            sharedMeshVertices = getLocalSpaceVertices(renderer, colliderMesh);
            sharedMeshNormals = colliderMesh.normals;
            sharedMeshTriangles = colliderMesh.triangles;
            this.meshOcclusionData = meshOcclusionData;
            this.occludingViewableLOD = occludingViewableLOD;
            wireFrameLineSegments = createWireFrameLineSegments();

            recalculateOccludedSubMesh();
        }

        #endregion

        #region Private Methods

        private Mesh createMeshCopyWithAdditionalSubMesh(Renderer renderer)
        {
            Mesh copy = UnityEngine.Object.Instantiate(renderer.GetSharedMesh());

            copy.name = $"{renderer.name}_copy";
            copy.subMeshCount++;

            return copy;
        }

        private Renderer createRendererCopy(Renderer originalRenderer, Transform targetParent, Transform rigRootTransform)
        {
            Renderer rendererCopy;

            if (rigRootTransform != null)
            {
                rendererCopy = UnityEngine.Object.Instantiate(originalRenderer);
                rendererCopy.gameObject.name = originalRenderer.name;

                if (originalRenderer is SkinnedMeshRenderer originalSkinnedMeshRenderer)
                {
                    SkinnedMeshData.Create(originalSkinnedMeshRenderer)?.ResolveBones(rendererCopy as SkinnedMeshRenderer, rigRootTransform);
                }

                foreach (Component component in rendererCopy.gameObject.GetComponents<Component>())
                {
                    if (!(component is Transform) && component != rendererCopy)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                    }
                }
            }
            else
            {
                var newGO = new GameObject(originalRenderer.name);
                newGO.transform.localRotation = originalRenderer.transform.rotation;
                newGO.transform.localScale = originalRenderer.transform.lossyScale;
                newGO.AddComponent<MeshFilter>().sharedMesh = originalRenderer.GetSharedMesh();
                rendererCopy = newGO.AddComponent<MeshRenderer>();
            }

            rendererCopy.transform.SetParent(targetParent);
            rendererCopy.gameObject.hideFlags = HideFlags.NotEditable;

            return rendererCopy;
        }

        private Material[] getSharedMaterialsForAdditionalSubMesh(int subMeshCount, Material defaultMaterial, Material occludedMaterial)
        {
            var sharedMaterials = new Material[subMeshCount];

            for (int i = 0; i < subMeshCount - 1; i++)
            {
                sharedMaterials[i] = defaultMaterial;
            }

            sharedMaterials[subMeshCount - 1] = occludedMaterial;

            return sharedMaterials;
        }

        private Mesh getColliderMesh(Renderer renderer)
        {
            switch (renderer)
            {
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    Mesh copy = UnityEngine.Object.Instantiate(skinnedMeshRenderer.sharedMesh);
                    copy.name = $"{renderer.name}_collider";
                    UnityVersionHelper.BakeMesh(skinnedMeshRenderer, copy, true);
                    return copy;
                default:
                    return renderer.GetSharedMesh();
            }
        }

        private Bounds getLocalSpaceBounds(Renderer renderer, Mesh mesh)
        {
            Vector3 center = renderer.transform.TransformPoint(mesh.bounds.center);
            Vector3 size = renderer.transform.TransformPoint(mesh.bounds.size);
            size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));

            return new Bounds(center, size);
        }

        private Vector3[] getLocalSpaceVertices(Renderer renderer, Mesh sharedMesh)
        {
            Vector3[] vertices = sharedMesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = renderer.transform.TransformPoint(vertices[i]);
            }

            return vertices;
        }

        private Vector3[] createWireFrameLineSegments()
        {
            var wireFrameLineSegments = new List<Vector3>();

            for (int i = 0; i < sharedMeshTriangles.Length; i += 3)
            {
                Vector3 vertex0 = sharedMeshVertices[sharedMeshTriangles[i]];
                Vector3 vertex1 = sharedMeshVertices[sharedMeshTriangles[i + 1]];
                Vector3 vertex2 = sharedMeshVertices[sharedMeshTriangles[i + 2]];

                wireFrameLineSegments.Add(vertex0);
                wireFrameLineSegments.Add(vertex1);
                wireFrameLineSegments.Add(vertex1);
                wireFrameLineSegments.Add(vertex2);
                wireFrameLineSegments.Add(vertex2);
                wireFrameLineSegments.Add(vertex0);
            }

            return wireFrameLineSegments.ToArray();
        }

        private void createRaycastStartPositions(int amountOfRaycastsPerVertex)
        {
            if (raycastStartPositions != null && raycastStartPositions.Length == amountOfRaycastsPerVertex)
            {
                return;
            }

            raycastStartPositions = new Vector3[amountOfRaycastsPerVertex];

            float rnd = 1f;
            float offset = 2f / amountOfRaycastsPerVertex;
            float increment = Mathf.PI * (3f - Mathf.Sqrt(5f));
            Bounds bounds = Bounds.size.sqrMagnitude > occludingViewableLOD.Bounds.size.sqrMagnitude ? Bounds : occludingViewableLOD.Bounds;
            float radius = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.5f;

            for (int i = 0; i < amountOfRaycastsPerVertex; i++)
            {
                float y = ((i * offset) - 1f) + (offset / 2f);
                float r = Mathf.Sqrt(1f - Mathf.Pow(y, 2f));
                float phi = ((i + rnd) % amountOfRaycastsPerVertex) * increment;

                raycastStartPositions[i] = bounds.center + new Vector3(Mathf.Cos(phi) * r * radius, y * radius, Mathf.Sin(phi) * r * radius) * 0.5f;
            }
        }

        private void recalculateOccludedSubMesh()
        {
            if (occludingViewableLOD == null || meshOcclusionData == null || !meshOcclusionData.TryGetTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData))
            {
                return;
            }

            var occludedTriangles = new List<int>();
            for (int i = 0; i < sharedMeshTriangles.Length; i += 3)
            {
                if (!triangleOcclusionData[i / 3])
                {
                    continue;
                }

                occludedTriangles.Add(sharedMeshTriangles[i]);
                occludedTriangles.Add(sharedMeshTriangles[i + 1]);
                occludedTriangles.Add(sharedMeshTriangles[i + 2]);
            }

            Mesh sharedMesh = renderer.GetSharedMesh();
            sharedMesh.SetTriangles(occludedTriangles, sharedMesh.subMeshCount - 1);
        }

        private void raycastForSelectedVertexHandle(int vertexHandleIndex, float pokeThroughRaycastDistance, int amountOfRaycastsPerVertex)
        {
            createRaycastStartPositions(amountOfRaycastsPerVertex);

            occludingViewableLOD.EnableMeshColliders(true);

            if (vertexHandleIsOccludedResults == null)
            {
                vertexHandleIsOccludedResults = new Tuple<bool, Vector3>[raycastStartPositions.Length];
            }

            bool isOccluding = isVertexOccluding(vertexHandleIndex, pokeThroughRaycastDistance, out Vector3 hitPoint);
            vertexHandleIsOccludingResult = new Tuple<bool, Vector3>(isOccluding, hitPoint);

            for (int i = 0; i < raycastStartPositions.Length; i++)
            {
                bool isOccluded = isVertexOccluded(raycastStartPositions[i], vertexHandleIndex, out hitPoint);
                vertexHandleIsOccludedResults[i] = new Tuple<bool, Vector3>(isOccluded, hitPoint);
            }

            cachedIsSelectedVertexHandleOccluded = isOccluding || vertexHandleIsOccludedResults.All(x => x.Item1);

            occludingViewableLOD.EnableMeshColliders(false);
        }

        private bool isVertexOccluding(int vertexIndex, float maxDistance, out Vector3 endPos)
        {
            Vector3 startPos = sharedMeshVertices[vertexIndex];
            Vector3 direction = -sharedMeshNormals[vertexIndex];
            endPos = startPos + direction * maxDistance;

            if (!occludingViewableLOD.Bounds.Contains(endPos))
            {
                return false;
            }

            if (!Physics.Raycast(startPos, direction, out RaycastHit hitInfo, maxDistance))
            {
                return false;
            }

            return hitInfo.collider != MeshCollider;
        }

        private bool isVertexOccluded(Vector3 startPos, int vertexIndex, out Vector3 hitPoint)
        {
            Vector3 endPos = sharedMeshVertices[vertexIndex];
            Vector3 delta = endPos - startPos;
            Vector3 direction = (endPos - startPos).normalized;
            float maxDistance = delta.magnitude;

            if (!occludingViewableLOD.Bounds.Contains(endPos))
            {
                hitPoint = endPos;
                return false;
            }

            if (!Physics.Raycast(startPos, direction, out RaycastHit hitInfo, maxDistance))
            {
                hitPoint = endPos;
                return true;
            }

            hitPoint = hitInfo.point;

            if (hitInfo.collider != MeshCollider)
            {
                return true;
            }

            if (sharedMeshTriangles[hitInfo.triangleIndex * 3 + 0] != vertexIndex &&
                sharedMeshTriangles[hitInfo.triangleIndex * 3 + 1] != vertexIndex &&
                sharedMeshTriangles[hitInfo.triangleIndex * 3 + 2] != vertexIndex)
            {
                return true;
            }

            return false;
        }

        private bool[] generateVertexOcclusionStates(float pokeThroughRaycastDistance, int amountOfRaycastsPerVertex)
        {
            createRaycastStartPositions(amountOfRaycastsPerVertex);

            bool[] vertexOcclusionStates = new bool[sharedMeshVertices.Length];

            for (int i = 0; i < sharedMeshVertices.Length; i++)
            {
                if (i % 10 == 0)
                {
                    EditorUtility.DisplayProgressBar("Generating Occlusion Data", "Raycasting...", (float)i / sharedMeshVertices.Length);
                }

                if (isVertexOccluding(i, pokeThroughRaycastDistance, out Vector3 endPos))
                {
                    vertexOcclusionStates[i] = true;
                    continue;
                }

                foreach (Vector3 spherePosition in raycastStartPositions)
                {
                    vertexOcclusionStates[i] = isVertexOccluded(spherePosition, i, out Vector3 hitPoint);

                    if (!vertexOcclusionStates[i])
                    {
                        break;
                    }
                }

                if (!vertexOcclusionStates[i])
                {
                    continue;
                }
            }

            EditorUtility.ClearProgressBar();

            return vertexOcclusionStates;
        }

        private void updateTriangleOcclusionData(bool[] vertexOcclusionStates)
        {
            bool[] triangleOcclusionData = meshOcclusionData.GetOrCreateTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id);

            for (int i = 0; i < triangleOcclusionData.Length; i++)
            {
                triangleOcclusionData[i] = false;
            }

            for (int i = 0; i < sharedMeshTriangles.Length; i += 3)
            {
                int vertexIndex0 = sharedMeshTriangles[i];
                int vertexIndex1 = sharedMeshTriangles[i + 1];
                int vertexIndex2 = sharedMeshTriangles[i + 2];

                if (vertexOcclusionStates[vertexIndex0] && vertexOcclusionStates[vertexIndex1] && vertexOcclusionStates[vertexIndex2])
                {
                    triangleOcclusionData[i / 3] = true;
                }
            }
        }

        private Vector3 calcTriangleCenter(int triangleIndex)
        {
            Vector3 p0 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3 + 0]];
            Vector3 p1 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3 + 1]];
            Vector3 p2 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3 + 2]];
            return (p0 + p1 + p2) / 3f;
        }

        #endregion

        #region Public Methods

        public static ViewableMesh Create(Renderer renderer, Transform targetParent, Transform rigRootTransform, MeshOcclusionData meshOcclusionData, ViewableLOD occludingViewableLOD, Material material)
        {
            #region Defense

            if (renderer == null)
            {
                Debug.LogError($"Unable to create {nameof(ViewableMesh)} - given {nameof(renderer)} is null.");
                return null;
            }

            if (material == null)
            {
                Debug.LogError($"Unable to create {nameof(ViewableMesh)} - given {nameof(material)} is null.");
                return null;
            }

            #endregion

            return new ViewableMesh(renderer, targetParent, rigRootTransform, meshOcclusionData, occludingViewableLOD, material);
        }

        public void Activate(bool isActive)
        {
            renderer.gameObject.SetActive(isActive);
        }

        public void EnableRenderer(bool isEnabled)
        {
            renderer.enabled = isEnabled;
        }

        public void EnableMeshCollider(bool isEnabled)
        {
            MeshCollider.enabled = isEnabled;
        }

        public bool HasOcclusionData()
        {
            return meshOcclusionData.TryGetTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData);
        }

        public bool GetOcclusionDataForHitTriangle(int triangleIndex)
        {
            if (!meshOcclusionData.TryGetTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData))
            {
                return false;
            }

            return triangleOcclusionData[triangleIndex];
        }

        public void SetOcclusionDataForHitTriangle(int triangleIndex, bool isTriangleOccluded)
        {
            bool[] triangleOcclusionData = meshOcclusionData.GetOrCreateTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id);

            triangleOcclusionData[triangleIndex] = isTriangleOccluded;

            recalculateOccludedSubMesh();
        }

        public void SetOcclusionDataForRadius(Vector3 point, bool occlude, Vector3 cameraPosition)
        {
            bool[] triangleOcclusionData = meshOcclusionData.GetOrCreateTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id);

            float sqrRadius = OcclusionDataViewerConfiguration.PersistentValues.PaintRadius * OcclusionDataViewerConfiguration.PersistentValues.PaintRadius;

            for (int i = 0; i < sharedMeshTriangles.Length / 3; i++)
            {
                if (triangleOcclusionData[i] == occlude)
                {
                    continue;
                }

                Vector3 other = calcTriangleCenter(i);

                if (Vector3.SqrMagnitude(point - other) > sqrRadius)
                {
                    continue;
                }

                if (Physics.Raycast(cameraPosition, (other - cameraPosition).normalized, out RaycastHit hitInfo, Vector3.Distance(cameraPosition, other) * 1.1f))
                {
                    if (Vector3.SqrMagnitude(hitInfo.point - other) <= .000001f)
                    {
                        triangleOcclusionData[i] = occlude;
                    }
                }
            }

            recalculateOccludedSubMesh();
        }

        public void GenerateOcclusionData(float pokeThroughRaycastDistance, int amountOfRaycastsPerVertex)
        {
            occludingViewableLOD.EnableMeshColliders(true);
            bool[] vertexOcclusionStates = generateVertexOcclusionStates(pokeThroughRaycastDistance, amountOfRaycastsPerVertex);
            occludingViewableLOD.EnableMeshColliders(false);

            updateTriangleOcclusionData(vertexOcclusionStates);

            recalculateOccludedSubMesh();
        }

        public void ClearOcclusionData()
        {
            if (!meshOcclusionData.TryGetTriangleOcclusionData(occludingViewableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData))
            {
                return;
            }

            for (int i = 0; i < triangleOcclusionData.Length; i++)
            {
                triangleOcclusionData[i] = false;
            }

            recalculateOccludedSubMesh();
        }

        public void DrawTriangleOutline(int triangleIndex)
        {
            Handles.zTest = CompareFunction.Disabled;

            Vector3 vertex0 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3]];
            Vector3 vertex1 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3 + 1]];
            Vector3 vertex2 = sharedMeshVertices[sharedMeshTriangles[triangleIndex * 3 + 2]];

            Handles.color = Color.yellow;
            Handles.DrawAAPolyLine(3f, vertex0, vertex1, vertex2, vertex0);
        }

        public void DrawRadiusPaintCircle(Vector3 point, SceneView sceneView)
        {
            Handles.zTest = CompareFunction.Disabled;
            Handles.color = Color.yellow;
            Handles.CircleHandleCap(0, point, sceneView.camera.transform.rotation, OcclusionDataViewerConfiguration.PersistentValues.PaintRadius, EventType.Repaint);
        }

        public void DrawWireFrame(Color color)
        {
            Handles.zTest = CompareFunction.LessEqual;

            Handles.color = color;
            Handles.DrawLines(wireFrameLineSegments);
        }

        public void SelectVertexHandle(int vertexHandleIndex, float pokeThroughRaycastDistance, int amountOfRaycastsPerVertex)
        {
            raycastForSelectedVertexHandle(vertexHandleIndex, pokeThroughRaycastDistance, amountOfRaycastsPerVertex);

            SelectedVertexHandleIndex = vertexHandleIndex;
        }

        public void ClearSelectedVertexHandle()
        {
            SelectedVertexHandleIndex = null;
        }

        public void DrawVertexHandles(int baseControlId, EventType eventType)
        {
            if (!SelectedTriangleIndex.HasValue)
            {
                return;
            }

            Handles.zTest = CompareFunction.LessEqual;

            for (int i = 0; i < 3; i++)
            {
                int vertexIndex = sharedMeshTriangles[SelectedTriangleIndex.Value * 3 + i];

                if (eventType == EventType.Repaint)
                {
                    if (!SelectedVertexHandleIndex.HasValue || vertexIndex != SelectedVertexHandleIndex.Value)
                    {
                        Handles.color = Color.gray;
                    }
                    else
                    {
                        Handles.color = cachedIsSelectedVertexHandleOccluded ? Color.red : Color.green;
                    }
                }

                Vector3 vertex = sharedMeshVertices[vertexIndex];
                float size = Mathf.Clamp(HandleUtility.GetHandleSize(vertex) * 0.2f, 0.001f, 0.01f);

                Handles.SphereHandleCap(baseControlId + vertexIndex, vertex, Quaternion.identity, size, eventType);
            }
        }

        public void DrawRaycasts(RaycastVisibilityOptions raycastVisibilityOptions)
        {
            if (!SelectedVertexHandleIndex.HasValue || raycastVisibilityOptions == RaycastVisibilityOptions.None)
            {
                return;
            }

            Handles.zTest = CompareFunction.Always;

            if (vertexHandleIsOccludingResult.Item1)
            {
                if (raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexPokingThrough))
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(sharedMeshVertices[SelectedVertexHandleIndex.Value], vertexHandleIsOccludingResult.Item2);
                }
                return;
            }
            if (raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexNotPokingThrough))
            {
                Handles.color = Color.green;
                Handles.DrawLine(sharedMeshVertices[SelectedVertexHandleIndex.Value], vertexHandleIsOccludingResult.Item2);
            }

            if (!raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexOccluded) && !raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexNotOccluded))
            {
                return;
            }

            Handles.zTest = CompareFunction.LessEqual;
            var occludedLineSegments = new List<Vector3>();
            var notOccludedLineSegments = new List<Vector3>();

            for (int i = 0; i < raycastStartPositions.Length; i++)
            {
                Vector3 raycastStartPoint = raycastStartPositions[i];
                Tuple<bool, Vector3> cachedIsOccludedResult = vertexHandleIsOccludedResults[i];

                if (cachedIsOccludedResult.Item1)
                {
                    occludedLineSegments.Add(raycastStartPoint);
                    occludedLineSegments.Add(cachedIsOccludedResult.Item2);
                    continue;
                }

                notOccludedLineSegments.Add(raycastStartPoint);
                notOccludedLineSegments.Add(cachedIsOccludedResult.Item2);
            }

            if (raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexOccluded))
            {
                Handles.color = Color.red;
                Handles.DrawLines(occludedLineSegments.ToArray());
            }

            if (raycastVisibilityOptions.HasFlag(RaycastVisibilityOptions.VertexNotOccluded))
            {
                Handles.color = Color.green;
                Handles.DrawLines(notOccludedLineSegments.ToArray());
            }
        }

        #endregion

    }
}
