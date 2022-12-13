using UnityEngine;

namespace MassiveProceduralTube {
    [RequireComponent(typeof(ProceduralTube))]
    public class ProceduralTube_ConstraintHandles :MonoBehaviour {

        [TextArea]
        //[Tooltip("Doesn't do anything. Just comments shown in inspector")]
        public string Note = " Using massive number of this function is not recommended." +
            "\n Use this for less than a hundred tubes.(Less than a ten for mobiles)";

        private ProceduralTube tube;
        public bool isConstraint = true;
        public Transform StartPoint;
        public Transform EndPoint;
        public Transform StartHandle;
        public Transform EndHandle;

        private bool isConstraintStart = false;
        private bool isConstraintEnd = false;
        private Transform cachedStartPoint;
        private Transform cachedEndPoint;

        private void Awake() {
            if(tube == null)
                tube = this.GetComponent<ProceduralTube>();
        }
        
        private void Update() {
            if(!isConstraint)
                return;
            if(StartPoint != null) {
                Vector3 p = tube.transform.InverseTransformPoint(StartPoint.position);
                if(StartPoint != cachedStartPoint) {
                    tube.handle.p1 = p + (tube.handle.p1 - tube.handle.p0);
                }
                tube.handle.p0 = p;
            }
                
            if(EndPoint != null) {
                Vector3 p = tube.transform.InverseTransformPoint(EndPoint.position);
                if(EndPoint != cachedEndPoint) {
                    tube.handle.p2 = p + (tube.handle.p2 - tube.handle.p3);
                }
                tube.handle.p3 = p;
            }
                
            if(StartHandle != null)
                tube.handle.p1 = tube.transform.InverseTransformPoint(StartHandle.position);
            if(EndHandle != null)
                tube.handle.p2 = tube.transform.InverseTransformPoint(EndHandle.position);
        }

    }
}

