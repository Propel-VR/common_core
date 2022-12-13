using UnityEngine;

namespace MassiveProceduralTube {
    public class ProceduralTube : MonoBehaviour {

        private ProceduralTubeManager manager;

        [Header("Check below if this tube is static. It affects big to the performance")]
        public bool isStatic = false;

        private ProceduralTube tube;
        [HideInInspector] public int tubeId;  // id will be updated from Manager, when any tube is destoryed

        // ProceduralTubeManager will collect this data on Start
        public ProceduralTubeData.Handle handle;
        private ProceduralTubeData.Handle cachedHandle;
        public void Reset() {
            //handle = new ProceduralTubeData.Handle();
            cachedHandle.p0 = handle.p0 = new Vector3(1f, 0f, 0f);
            cachedHandle.p1 = handle.p1 = new Vector3(2f, 0f, 1f);
            cachedHandle.p2 = handle.p2 = new Vector3(2f, 0f, -1f);
            cachedHandle.p3 = handle.p3 = new Vector3(3f, 0f, 0f);
            cachedHandle.radiusStart = handle.radiusStart = 0.1f;
            cachedHandle.radiusEnd = handle.radiusEnd = 0.1f;
            cachedHandle.taperType = handle.taperType = 0;
        }
        public enum TaperType {
            Linear,
            Exponential,
            InverseExponential,
            Smooth,
            Symmetry,
        }
        public TaperType taperType = TaperType.Linear;
        private TaperType cachedTaperType;

        private void Awake() {
            if(manager == null)
                manager = FindObjectOfType<ProceduralTubeManager>();
            if(manager == null)
                Debug.Log("<color=red>[ProceduralTube] You need a ProceduralTubeManager in the scene.</color>");
            tube = this.GetComponent<ProceduralTube>();
            cachedTaperType = taperType;
            cachedHandle = handle;
            TaperTypeChanged(taperType);
            transform.hasChanged = false;
        }

        private void Update() {
            if(!isStatic) {
                if(cachedTaperType != taperType) {
                    TaperTypeChanged(taperType);
                    cachedTaperType = taperType;
                }
                if(transform.hasChanged) {
                    manager.UpdateHandleTransformData(tubeId);
                    transform.hasChanged = false;
                }
                CheckHandleChanges();
            }
        }

        // Check any Handle value is changed
        private void CheckHandleChanges() {
            if(cachedHandle.p0 != handle.p0) {
                manager.UpdateHandleData(tubeId, 0);
                cachedHandle.p0 = handle.p0;
            }
                
            if(cachedHandle.p1 != handle.p1) {
                manager.UpdateHandleData(tubeId, 1);
                cachedHandle.p1 = handle.p1;
            }
            if(cachedHandle.p2 != handle.p2) {
                manager.UpdateHandleData(tubeId, 2);
                cachedHandle.p2 = handle.p2;
            }
            if(cachedHandle.p3 != handle.p3) {
                manager.UpdateHandleData(tubeId, 3);
                cachedHandle.p3 = handle.p3;
            }

            if(cachedHandle.radiusStart != handle.radiusStart) {
                manager.UpdateHandleData(tubeId, 4);
                cachedHandle.radiusStart = handle.radiusStart;
            }
            if(cachedHandle.radiusEnd != handle.radiusEnd) {
                manager.UpdateHandleData(tubeId, 5);
                cachedHandle.radiusEnd = handle.radiusEnd;
            }

            if(cachedHandle.taperType != handle.taperType) {
                switch(handle.taperType) {
                    case 0:
                        taperType = TaperType.Linear;
                        break;
                    case 1:
                        taperType = TaperType.Exponential;
                        break;
                    case 2:
                        taperType = TaperType.InverseExponential;
                        break;
                    case 3:
                        taperType = TaperType.Smooth;
                        break;
                    case 4:
                        taperType = TaperType.Symmetry;
                        break;
                }
                manager.UpdateHandleData(tubeId, 6);
                cachedHandle.taperType = handle.taperType;
            }
        }

        private void TaperTypeChanged(TaperType type) {
            switch(type) {
                case TaperType.Linear:
                    handle.taperType = 0;
                    break;
                case TaperType.Exponential:
                    handle.taperType = 1;
                    break;
                case TaperType.InverseExponential:
                    handle.taperType = 2;
                    break;
                case TaperType.Smooth:
                    handle.taperType = 3;
                    break;
                case TaperType.Symmetry:
                    handle.taperType = 4;
                    break;
            }
        }

        
        // Called from ProceduralTubeInspector.cs, when the handles are changed from inspector
        public void HandleChanged(int id) {
            if(manager == null)
                manager = FindObjectOfType<ProceduralTubeManager>();
            if(manager != null)
                manager.UpdateHandleData(tubeId, id);
        }

        
        
        private void AddThisTubeToManager() {
            if(manager == null)
                manager = FindObjectOfType<ProceduralTubeManager>();
            if(manager != null)
                manager.UpdateTube(tube, out tubeId);
        }
        private void RemoveThisTubeToManager() {
            if(manager == null)
                manager = FindObjectOfType<ProceduralTubeManager>();
            if(manager != null)
                manager.RemoveTube(tubeId);
        }
        
        private void OnEnable() {
            AddThisTubeToManager();
        }
        private void OnDisable() {
            RemoveThisTubeToManager();
        }

    } // class
}  // namespace

