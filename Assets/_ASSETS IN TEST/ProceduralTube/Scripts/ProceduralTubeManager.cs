using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MassiveProceduralTube {
    [RequireComponent(typeof(ProceduralTubeRenderer))]
    public class ProceduralTubeManager :MonoBehaviour {


        #region Tube Assets
        private int curveNum; // curve count
        
        private const int BASIC_NODE_NUM_PER_CURVE = 8; // Basic node number per curve

        [Header("Tubes Properties")]
        [Tooltip("Curve Smoothness"), SerializeField, Range(1, 8)]
        private int DivisionNumber = 2; // division number per curve range 
        private int cachedDivNum;

        // Below are used by ProceduralTubeRenderer.cs
        [HideInInspector] public int nodeNumPerCurve; // per curve : BASIC_NODE_NUM * DivisionNumber
        [HideInInspector] public int nodeNum;         // Total     : curveNum * nodeNumPerCurve

        [Tooltip("Segment number for all tubes"),SerializeField, Range(3, 128)]
        public int TubeSegmentsNumber = 16;  // Tube meshes' seg number, used by Renderer when the mesh is null
        private int cachedSegNum;
        #endregion


        #region Simulation Properties
        public bool isSpringMode = true;

        public float intensity_spring = 1f;
        [Range(0, 0.2f)] public float drag_spring = 0.07f;
        public float gravity = 0.2f;
 
        private struct SpringProps {
            public Vector3 vel1;
            public Vector3 vel2;
        };
        private struct SpringHandleOffset {
            public Vector3 offset1;  // offset1 = point1 - point0, these offsets will be updated when the handles(p1,p2) are changed.
            public Vector3 offset2;
        };
        SpringHandleOffset[] springHandleOffset;
        #endregion


        #region ComputeShader Values
        [Header("Tubes Properties")]
        [Tooltip("CalcBezierCurve.compute should be attached here"), SerializeField]
        private ComputeShader computeShader;
        private int kernel_CalcBezier_Id = -1;
        private int kernel_CalcSpring_Id = -1;
        public struct CSPARAM {  // These parameters should be matched with CalcBezierCurve.compute
            public const string KERNEL_CALC = "CalcBezierPoints";
            public const string KERNEL_SPRING = "CalcHandleSpring";

            public const string CURVE_NUM = "_CurveNum";
            public const string NODE_NUM_PER_CURVE = "_NodeNumPerCurve";
            public const string NODE_NUM = "_NodeNum";

            public const string NODE_BUFFER = "_NodeBuffer";
            public const string HANDLE_BUFFER = "_HandleBuffer";                       // Read & write for simulation
            public const string HANDLE_BUFFER_FROM_TUBE = "_HandleBufferFromTube";     // Read only buffer from tube handles
            public const string SPRINGPROPS_BUFFER = "_SpringPropsBuffer";
            public const string SPRING_HANDLE_OFFSET_BUFFER = "_SpringHandleOffset";

            public const string TIME = "_mTime";

            public const string INTEN_SPRING = "_intensitySpring";
            public const string DRAG_SPRING = "_dragSpring";
            public const string GRAVITY = "_gravity";
            
            public const string DELTA_TIME = "_DT";
        }
        #endregion

        #region Buffers Values
        private bool isBufferInitialized = false;
        private bool isSpringHandleOffsetBufferInitialized = false;
        public ComputeBuffer handleBuffer_fromTubes;
        public ComputeBuffer handleBuffer;
        public ComputeBuffer nodeBuffer;
        public ComputeBuffer springPropsBuffer;
        public ComputeBuffer springHandleOffsetBuffer;

        private const int MAX_CURVE_NUM = 1024;
        private const int MAX_NODE_NUM = 1024 * 8 * 8;  // MAX_CURVE_NUM * BASIC_NODE_NUM * divNum
        #endregion


        #region Dependent Objects
        public List<ProceduralTube> tubes = new List<ProceduralTube>();
        private ProceduralTubeData.Handle[] handles;
        [SerializeField] private ProceduralTubeRenderer tubeRenderer;
        #endregion




        #region MonoBehaviour
        private void Awake() {
            cachedDivNum = DivisionNumber;
            cachedSegNum = TubeSegmentsNumber;
            if(tubeRenderer == null)
                tubeRenderer = this.GetComponent<ProceduralTubeRenderer>();
        }
        private void Update() {
            if(DivisionNumber != cachedDivNum) {
                UpdateDivisionNumber();              // Call InitializeBuffer() and tubeRenderer.UpdateInstanceBuffers()
            }
            if(TubeSegmentsNumber != cachedSegNum) {
                CreateInstanceMesh();                // Call tubeRenderer.CreateMeshProcedural(TubeSegmentsNumber)
            }
            
        }
        private void LateUpdate() {
            if(!isBufferInitialized) {  // When tube num is changed
                isBufferInitialized = true;
                InitializeBuffer();
                tubeRenderer.UpdateInstanceBuffers();
            }
            if(!isSpringHandleOffsetBufferInitialized) {
                isSpringHandleOffsetBufferInitialized = true;
                UpdateSpringHandleOffsetBuffer();
            }
            if(isSpringMode)
                DispatchCalcSpring();
            
            DispatchCalcBezier();               // Dispatch "CalcBezierPoints" in CalcBezierCurve.compute, 
            tubeRenderer.DrawMeshInstance();    // Rendering(DrawMeshInstance)
        }
        #endregion

        
        private void InitializeBuffer() {
            kernel_CalcBezier_Id = computeShader.FindKernel(CSPARAM.KERNEL_CALC);
            kernel_CalcSpring_Id = computeShader.FindKernel(CSPARAM.KERNEL_SPRING);

            // Get Tubes Data
            curveNum = tubes.Count;
            nodeNumPerCurve = BASIC_NODE_NUM_PER_CURVE * DivisionNumber;
            nodeNum = curveNum * nodeNumPerCurve;

            Debug.Log("[ProceduralTube/Manager] Buffer Initialized, There are (" + curveNum + ") curves in the scene.");

            // Get Tubes Handle Data
            handles = new ProceduralTubeData.Handle[curveNum];
            UpdateHandlesBuffer(); // Buffer will be updated in the UpdateHandles
            handleBuffer = new ComputeBuffer(curveNum, Marshal.SizeOf(typeof(ProceduralTubeData.Handle)));
            handleBuffer.SetData(handles);

            // Initial Node Data
            ProceduralTubeData.Node[] nodes = new ProceduralTubeData.Node[nodeNum];
            for(int i = 0; i < nodeNum; i++) {
                nodes[i].position = Vector3.zero;
                nodes[i].radius = 0.2f;
            }
            nodeBuffer = new ComputeBuffer(nodeNum, Marshal.SizeOf(typeof(ProceduralTubeData.Node)));
            nodeBuffer.SetData(nodes);

            // Initial Spring Properties Data
            SpringProps[] springs = new SpringProps[curveNum];
            for(int i = 0; i < springs.Length; i++) {
                Transform tr = tubes[i].transform;
                springs[i].vel1 = Vector3.zero;
                springs[i].vel2 = Vector3.zero;
            }
            springPropsBuffer = new ComputeBuffer(curveNum, Marshal.SizeOf(typeof(SpringProps)));
            springPropsBuffer.SetData(springs);

            springHandleOffset = new SpringHandleOffset[curveNum];
            springHandleOffsetBuffer = new ComputeBuffer(curveNum, Marshal.SizeOf(typeof(SpringHandleOffset)));
            UpdateSpringHandleOffsetBuffer();
        }


        // When a handle data is changed
        private void UpdateHandleBuffer() {
            handleBuffer_fromTubes.SetData(handles);
        }

        // When a handle is enabled or disabled. When the handle number(=node number) is changed.
        private void UpdateHandlesBuffer() {
            //Debug.Log("ProceduralTube/Manager] Handles Updated.");
            for(int i = 0; i < handles.Length; i++) {
            //for(int i = 0; i < curveNum; i++){
                    
                Transform tr = tubes[i].transform;
                handles[i].p0 = tr.TransformPoint(tubes[i].handle.p0);  // Local To World
                handles[i].p1 = tr.TransformPoint(tubes[i].handle.p1);
                handles[i].p2 = tr.TransformPoint(tubes[i].handle.p2);
                handles[i].p3 = tr.TransformPoint(tubes[i].handle.p3);
                handles[i].radiusStart = tubes[i].handle.radiusStart;
                handles[i].radiusEnd = tubes[i].handle.radiusEnd;
                handles[i].taperType = tubes[i].handle.taperType;
            }
            handleBuffer_fromTubes = new ComputeBuffer(curveNum, Marshal.SizeOf(typeof(ProceduralTubeData.Handle)));
            handleBuffer_fromTubes.SetData(handles);
        }
        
        private void DispatchCalcSpring() {
            computeShader.SetFloat(CSPARAM.INTEN_SPRING, intensity_spring);
            computeShader.SetFloat(CSPARAM.DRAG_SPRING, drag_spring);
            computeShader.SetFloat(CSPARAM.GRAVITY, gravity);
            computeShader.SetFloat(CSPARAM.DELTA_TIME, Time.deltaTime);

            computeShader.SetBuffer(kernel_CalcSpring_Id, CSPARAM.SPRINGPROPS_BUFFER, springPropsBuffer);
            computeShader.SetBuffer(kernel_CalcSpring_Id, CSPARAM.SPRING_HANDLE_OFFSET_BUFFER, springHandleOffsetBuffer);

            computeShader.SetBuffer(kernel_CalcSpring_Id, CSPARAM.HANDLE_BUFFER_FROM_TUBE, handleBuffer_fromTubes);  // Read&Write
            computeShader.SetBuffer(kernel_CalcSpring_Id, CSPARAM.HANDLE_BUFFER, handleBuffer);  // Read&Write

            computeShader.Dispatch(kernel_CalcSpring_Id, curveNum, 1, 1);
        }
        
        private void DispatchCalcBezier() {
            computeShader.SetInt(CSPARAM.CURVE_NUM, curveNum);
            computeShader.SetInt(CSPARAM.NODE_NUM_PER_CURVE, nodeNumPerCurve);
            computeShader.SetInt(CSPARAM.NODE_NUM, nodeNum);
            
            if(isSpringMode) {
                computeShader.SetBuffer(kernel_CalcBezier_Id, CSPARAM.HANDLE_BUFFER, handleBuffer);   // Buffer from Spring or Following
            } else {
                computeShader.SetBuffer(kernel_CalcBezier_Id, CSPARAM.HANDLE_BUFFER, handleBuffer_fromTubes);  // Buffer from Tube
            }

            computeShader.SetBuffer(kernel_CalcBezier_Id, CSPARAM.NODE_BUFFER, nodeBuffer);

            computeShader.SetFloat(CSPARAM.TIME, Time.time);

            computeShader.Dispatch(kernel_CalcBezier_Id, curveNum, 1, 1);
        }

        private void UpdateSpringHandleOffsetBuffer() {
            for(int i = 0; i < springHandleOffset.Length; i++) {
                Transform tr = tubes[i].transform;
                springHandleOffset[i].offset1 = tr.TransformPoint(tubes[i].handle.p1) - tr.TransformPoint(tubes[i].handle.p0);
                springHandleOffset[i].offset2 = tr.TransformPoint(tubes[i].handle.p2) - tr.TransformPoint(tubes[i].handle.p3);
            }
            springHandleOffsetBuffer.SetData(springHandleOffset);
        }

        // This is too heavy to run in runtime.
        private void UpdateRenderBounds(){
            float x = 0;
            float y = 0;
            float z = 0;
            for(int i = 0; i < curveNum; i++) {
                Transform tr = tubes[i].transform;
                Vector3 p0 = tr.TransformPoint(tubes[i].handle.p0);
                Vector3 p1 = tr.TransformPoint(tubes[i].handle.p1);
                Vector3 p2 = tr.TransformPoint(tubes[i].handle.p2);
                Vector3 p3 = tr.TransformPoint(tubes[i].handle.p3);
                float r = Mathf.Max(tubes[i].handle.radiusStart, tubes[i].handle.radiusEnd);
                x = GetLargestValue(x, Mathf.Abs(p0.x)+r, Mathf.Abs(p1.x)+r, Mathf.Abs(p2.x)+r, Mathf.Abs(p3.x)+r);
                y = GetLargestValue(y, Mathf.Abs(p0.y)+r, Mathf.Abs(p1.y)+r, Mathf.Abs(p2.y)+r, Mathf.Abs(p3.y)+r);
                z = GetLargestValue(z, Mathf.Abs(p0.z)+r, Mathf.Abs(p1.z)+r, Mathf.Abs(p2.z)+r, Mathf.Abs(p3.z)+r);
            }
            tubeRenderer.BoundSize = new Vector3(x,y,z);
        }

        private float GetLargestValue(params float[] v){
            float a = -10000;
            for(int i=0; i<v.Length; i++){
                a = Mathf.Max(a, v[i]);
            }
            return a;
        }

        #region Changing Events

        // When the TubeSegmentsNumber is changed
        private void CreateInstanceMesh() {
            //Debug.Log("[ProceduralTube] Tube Segment Number is changed.");
            tubeRenderer.CreateMeshProcedural(TubeSegmentsNumber);
            tubeRenderer.UpdateInstanceBuffers();
            cachedSegNum = TubeSegmentsNumber;
        }
        
        // Event from each ProceduralTube.cs, when the handle is destroyed or disabled
        // Or the DivisionNumber is Changed
        public void UpdateTube(ProceduralTube tube, out int id) {
            //Debug.Log("[ProceduralTube] Tubes Number is changed.");
            tubes.Add(tube);
            id = tubes.Count - 1;
            isBufferInitialized = false;
        }
        public void RemoveTube(int id) {
            tubes.RemoveAt(id);
            for(int i=0; i<tubes.Count; i++) {
                tubes[i].tubeId = i;
            }
            isBufferInitialized = false;
        }

        // Event from each ProceduralTube.cs, when the handle is moved
        public void UpdateHandleData(int id, int _n) {
            if(!isBufferInitialized) {
                return;  // prevent errors when the scene is just started
            }
            Transform tr = tubes[id].transform;
            switch(_n) {
                case 0:
                    handles[id].p0 = tr.TransformPoint(tubes[id].handle.p0);  // Local To World
                    break;
                case 1:
                    handles[id].p1 = tr.TransformPoint(tubes[id].handle.p1);
                    break;
                case 2:
                    handles[id].p2 = tr.TransformPoint(tubes[id].handle.p2);
                    break;
                case 3:
                    handles[id].p3 = tr.TransformPoint(tubes[id].handle.p3);
                    break;
                case 4:
                    handles[id].radiusStart = tubes[id].handle.radiusStart;
                    break;
                case 5:
                    handles[id].radiusEnd = tubes[id].handle.radiusEnd;
                    break;
                case 6:
                    handles[id].taperType = tubes[id].handle.taperType;
                    break;
            }
            UpdateHandleBuffer();
            if(_n == 1 || _n == 2) {
                isSpringHandleOffsetBufferInitialized = false;
            }
        }

        // Event from each ProceduralTube.cs, when the tube transform has changed
        public void UpdateHandleTransformData(int id) {
            Transform tr = tubes[id].transform;
            handles[id].p0 = tr.TransformPoint(tubes[id].handle.p0);  // Local To World
            handles[id].p1 = tr.TransformPoint(tubes[id].handle.p1);
            handles[id].p2 = tr.TransformPoint(tubes[id].handle.p2);
            handles[id].p3 = tr.TransformPoint(tubes[id].handle.p3);
            UpdateHandleBuffer();
        }

        private void UpdateDivisionNumber() {
            //Debug.Log("[ProceduralTube] Tubes Number is changed.");
            InitializeBuffer();
            tubeRenderer.UpdateInstanceBuffers();
            cachedDivNum = DivisionNumber;
        }
        #endregion

    } // class
} // namespace