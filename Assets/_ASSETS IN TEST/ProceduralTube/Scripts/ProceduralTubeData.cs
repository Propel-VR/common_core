using UnityEngine;

namespace MassiveProceduralTube {
    public class ProceduralTubeData :MonoBehaviour {

        public struct Node {
            public Vector3 position;
            public float radius;
        }

        [System.Serializable]
        public struct Handle {
            public Vector3 p0; // curve start position
            public Vector3 p1; // curve start handle
            public Vector3 p2; // curve end handle
            public Vector3 p3; // curve end position
            public float radiusStart; // radius of this node
            public float radiusEnd;   // radius of the next node
            [HideInInspector] public int taperType;     // 0: Linear, 1:Exponential, 2: InverseExponential, 3: Smooth, 4: Symmetry 
        }

    } // class
}  // namespace

