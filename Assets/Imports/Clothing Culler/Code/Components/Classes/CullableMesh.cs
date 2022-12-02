using System;
using System.Collections;
using System.Collections.Generic;
using Salvage.ClothingCuller.Configuration;
using Salvage.ClothingCuller.ExtensionMethods;
using UnityEngine;

namespace Salvage.ClothingCuller.Components
{
    public class CullableMesh
    {
        private readonly Renderer renderer;
        private readonly Mesh originalSharedMesh;
        private readonly MeshOcclusionData meshOcclusionData;
        private readonly List<bool[]> appliedTriangleOcclusionData;
        private bool initialized;
        private Mesh culledMesh;
        private int[][] originalTrianglesBySubmesh;
        private int[][] visibleTrianglesBySubmesh;
        private int originalTriangleCount;
        private bool[] mergedTriangleOcclusionData;

        #region Constructor

        private CullableMesh(Renderer renderer, Mesh originalSharedMesh, MeshOcclusionData meshOcclusionData)
        {
            this.renderer = renderer;
            this.originalSharedMesh = originalSharedMesh;
            this.meshOcclusionData = meshOcclusionData;

            appliedTriangleOcclusionData = new List<bool[]>();
        }

        #endregion

        #region Private Methods

        private void initialize()
        {
            culledMesh = createCulledMesh(originalSharedMesh);
            originalTrianglesBySubmesh = createOriginalTrianglesBySubmesh(originalSharedMesh);
            visibleTrianglesBySubmesh = createVisibleTrianglesBySubmesh(originalTrianglesBySubmesh);
            originalTriangleCount = createOriginalTriangleCount(originalTrianglesBySubmesh);
            mergedTriangleOcclusionData = createMergedTriangleOcclusionData(originalTriangleCount);

            initialized = true;
        }

        private Mesh createCulledMesh(Mesh originalSharedMesh)
        {
            Mesh culledMesh = UnityEngine.Object.Instantiate(originalSharedMesh);
            culledMesh.name = culledMesh.name.Replace("(Clone)", "_culled");

            return culledMesh;
        }

        private int[][] createOriginalTrianglesBySubmesh(Mesh originalSharedMesh)
        {
            var originalTrianglesBySubmesh = new int[originalSharedMesh.subMeshCount][];

            for (int i = 0; i < originalSharedMesh.subMeshCount; i++)
            {
                originalTrianglesBySubmesh[i] = originalSharedMesh.GetTriangles(i);
            }

            return originalTrianglesBySubmesh;
        }

        private int[][] createVisibleTrianglesBySubmesh(int[][] originalTrianglesBySubmesh)
        {
            var visibleTrianglesBySubmesh = new int[originalTrianglesBySubmesh.Length][];

            for (int i = 0; i < originalTrianglesBySubmesh.Length; i++)
            {
                visibleTrianglesBySubmesh[i] = new int[originalTrianglesBySubmesh[i].Length];
            }

            return visibleTrianglesBySubmesh;
        }

        private int createOriginalTriangleCount(int[][] originalTrianglesBySubmesh)
        {
            int originalTriangleCount = 0;

            for (int i = 0; i < originalTrianglesBySubmesh.Length; i++)
            {
                originalTriangleCount += originalTrianglesBySubmesh[i].Length;
            }

            return originalTriangleCount;
        }

        private bool[] createMergedTriangleOcclusionData(int triangleCount)
        {
            return new bool[triangleCount];
        }

        private void recalculateTriangles()
        {
            if (appliedTriangleOcclusionData.Count == 0)
            {
                renderer.SetSharedMesh(originalSharedMesh);
                return;
            }

            mergeTriangleOcclusionData();

            for (int i = 0; i < originalSharedMesh.subMeshCount; i++)
            {
                int visibleTriangleCount = setVisibleTriangles(originalTrianglesBySubmesh[i], originalSharedMesh.GetIndexStart(i), mergedTriangleOcclusionData, visibleTrianglesBySubmesh[i]);
                culledMesh.SetTriangles(visibleTrianglesBySubmesh[i], 0, visibleTriangleCount, i);
            }

            renderer.SetSharedMesh(culledMesh);
        }

        private void mergeTriangleOcclusionData()
        {
            for (int i = 0; i < mergedTriangleOcclusionData.Length; i++)
            {
                mergedTriangleOcclusionData[i] = false;
            }

            foreach (bool[] triangleOcclusionData in appliedTriangleOcclusionData)
            {
                for (int i = 0; i < mergedTriangleOcclusionData.Length; i++)
                {
                    if (triangleOcclusionData[i])
                    {
                        mergedTriangleOcclusionData[i] = true;
                    }
                }
            }
        }

        private int setVisibleTriangles(int[] submeshTriangles, uint startIndex, bool[] mergedTriangleOcclusionData, int[] visibleSubmeshTriangles)
        {
            int visibleTriangleCount = 0;
            for (int i = 0; i < submeshTriangles.Length; i += 3)
            {
                if (mergedTriangleOcclusionData[(i + startIndex) / 3])
                {
                    continue;
                }

                visibleSubmeshTriangles[visibleTriangleCount] = submeshTriangles[i];
                visibleSubmeshTriangles[visibleTriangleCount + 1] = submeshTriangles[i + 1];
                visibleSubmeshTriangles[visibleTriangleCount + 2] = submeshTriangles[i + 2];
                visibleTriangleCount += 3;
            }

            return visibleTriangleCount;
        }

        #endregion

        #region Public Methods

        public static CullableMesh Create(Renderer renderer, MeshOcclusionData meshOcclusionData)
        {
            #region Defense

            if (renderer == null)
            {
                Debug.LogError($"Unable to create {nameof(CullableMesh)} - given {nameof(renderer)} is null.");
                return null;
            }

            if (meshOcclusionData == null)
            {
                Debug.LogError($"Unable to create {nameof(CullableMesh)} - given {nameof(meshOcclusionData)} is empty.");
                return null;
            }

            #endregion

            Mesh originalSharedMesh = renderer.GetSharedMesh();
            if (originalSharedMesh == null)
            {
                return null;
            }

            return new CullableMesh(renderer, originalSharedMesh, meshOcclusionData);
        }

        public bool TryCull(CullableLOD occluderCullableLOD)
        {
            if (!initialized)
            {
                initialize();
            }

            if (!meshOcclusionData.TryGetTriangleOcclusionData(occluderCullableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData))
            {
                Debug.LogError($"Unable to cull '{originalSharedMesh.name}' - no occlusion data found for given occluding LOD.");
                return false;
            }

            if (triangleOcclusionData.Length != originalTriangleCount)
            {
                Debug.LogError($"Unable to cull '{originalSharedMesh.name}' - amount of triangles doesn't match the amount of triangles in serialized data.");
                return false;
            }

            appliedTriangleOcclusionData.Add(triangleOcclusionData);

            recalculateTriangles();

            return true;
        }

        public bool TryUncull(CullableLOD occluderCullableLOD)
        {
            if (!meshOcclusionData.TryGetTriangleOcclusionData(occluderCullableLOD.LodOcclusionData.Id, out bool[] triangleOcclusionData))
            {
                Debug.LogError($"Unable to cull '{originalSharedMesh.name}' - no occlusion data found for given occluding LOD.");
                return false;
            }

            appliedTriangleOcclusionData.Remove(triangleOcclusionData);

            recalculateTriangles();

            return true;
        }

        #endregion

    }
}
