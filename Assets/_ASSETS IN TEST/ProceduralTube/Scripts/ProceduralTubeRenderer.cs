using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MassiveProceduralTube {
    [RequireComponent(typeof(ProceduralTubeManager))]
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralTubeRenderer : MonoBehaviour {

        /// <summary>
        /// All the functions will be called by ProceduralTubeManager.cs
        /// UpdateInstanceBuffers(), will be called when the node number is changed.
        /// DrawMeshInstance(), will be called when manager Update(), LateUpdate()
        /// CreateMeshProcedural() will be called when the manager Start(), Awake()
        /// </summary>
        
        [SerializeField, Tooltip("The Manager should be attached in the same GameObject.")]
        private ProceduralTubeManager manager;

        #region Material
        private Shader instanceShader;
        [Space(15)]
        public Material instanceMaterial;
        public Vector2 TextureTiling = Vector2.one;
        public Vector2 TextureOffset = Vector2.zero;
        private Vector2 cachedTiling = Vector2.one;
        private Vector2 cachedOffset = Vector2.zero;

        private int prop_TexAlbedo;
        private int prop_TexNormal;
        private int prop_TexMetallicGloss;
        private int prop_TexAmbientOcclusion;
        private int prop_TexEmission;
        #endregion

        #region Shadow
        [Space(15)]
        public ShadowCastingMode CastShadows = ShadowCastingMode.On;
        public bool ReceiveShadows = true;
        #endregion
        
        #region Mesh
        private Mesh instanceMesh;
        private MeshFilter meshFilter;
        #endregion

        #region Properties_Cylinder
        private float cyl_height = 1;     // This should be 1! This value will be used in vertex shader
        private float cyl_radius = 1;     // This should be 1. This value will be a scale of each node
        private const int cyl_seg_height = 2;  // This value should be 2
        private int cyl_seg_radius = 16;       // This value will be changed by the manager
        private const float TWO_PI = Mathf.PI * 2f;
        #endregion


        #region Properties_DrawMeshInstanced
        private ComputeBuffer argsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        private int instanceCount = 100000; // This value depends on the nodeNum from the manager
        private int subMeshIndex = 0;

        public Vector3 BoundSize {
            set => renderBounds.size = value;
        }
        private Bounds renderBounds = new Bounds();
        #endregion


        private void Awake() {
            meshFilter = this.GetComponent<MeshFilter>();
            if(manager == null)
                manager = this.GetComponent<ProceduralTubeManager>();
            if(instanceShader == null)
                instanceShader = Shader.Find("Procedural Tube/Standard(Basic)/Opaque");
            if(instanceMaterial == null)
                instanceMaterial = new Material(instanceShader);
            GetShaderPropertyID();
        }
        private void Update() {
            ApplyTextureTilingOffset();
        }


        #region Public Functions
        // Called by Manager when the node number is changed
        public void UpdateInstanceBuffers() {
            //Debug.Log("[ProceduralTube/Renderer] InstanceBuffer is updated.");
            if(argsBuffer == null) {
                argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint),
                ComputeBufferType.IndirectArguments);
                args[0] = args[1] = args[2] = args[3] = 0;
            }
            if(instanceMesh == null) {
                CreateMeshProcedural(manager.TubeSegmentsNumber);
            }
            args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
            args[1] = (uint)manager.nodeNum;  // Instance Number
            args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
            args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
            argsBuffer.SetData(args);
        }

        // Called by Manager when Update(), LateUpdate()
        public void DrawMeshInstance() {
            //Debug.Log("[ProceduralTube/Renderer] Draw Mesh Instance. Number of instance: " + args[1]);
            instanceMaterial.SetBuffer(ProceduralTubeManager.CSPARAM.NODE_BUFFER, manager.nodeBuffer);
            float uvMult = 1.0f / (manager.nodeNumPerCurve-1);
            instanceMaterial.SetFloat("_UVmult", uvMult);
            instanceMaterial.SetFloat("_NodeNumPerCurve", manager.nodeNumPerCurve);

            Graphics.DrawMeshInstancedIndirect(
                instanceMesh,     // Mesh that will be used for instancing
                subMeshIndex,     // SubMesh Index
                instanceMaterial, // Material that will be used for rendering
                new Bounds(Vector3.zero, Vector3.one*10000),  // Infinity value causes flickering
                argsBuffer,        // Buffers for GPU Instancing
                0,     // argsOffset
                null,  // material property block
                castShadows:CastShadows,
                receiveShadows:ReceiveShadows,  // ReceiveShadows
                layer:0      // layer
                );
        }

        // Called by Manager when Start(), Awake()
        public void CreateMeshProcedural(int _segments_radius) {
            //Debug.Log("[ProceduralTube/Renderer] Instance Cylinder is Created with (" + _segments_radius + ") segments.");
            cyl_seg_radius = _segments_radius;
            instanceMesh = new Mesh();
            instanceMesh.name = "ProceduralInstanceMesh";
            instanceMesh = BuildCylinderMesh(cyl_height, cyl_radius, cyl_seg_height, cyl_seg_radius); // height, radius, seg_height, seg_radius
            if(meshFilter == null)
                meshFilter = this.GetComponent<MeshFilter>();
            meshFilter.mesh = instanceMesh;
        }
        #endregion


        #region Procedural Cylinder
        // This will be called by the manager.Awake(), and segments num is changed

        private Mesh BuildCylinderMesh(float _height, float _radius, int _seg_height, int _seg_radius) {
            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();
            
            int verNumRow = _seg_radius + 1; // +1 for welding

            float heightPerSegment = _height / (_seg_height - 1); // height per segments_height

            // vertices, uvs, normals
            for(int i = 0; i < _seg_height; i++) {
                float top = heightPerSegment * (i);
                //float bottom = top - heightPerSegment;
                GenerateRingZ(verNumRow, top, _radius, _height, vertices, uvs, normals, colors, true, i == (_seg_height-1));  // Generate one ring
            }
            
            // triangles
            for(int i = 0; i < _seg_height - 1; i++) { // Face number is (nodeNumPerCurve-1), so calculate triangles only for the faces
                for(int j = 0; j < _seg_radius; j++) {

                    int idx = j + i * verNumRow;// not segments_radius, because of welding problem
                    
                    int a = idx + 0;
                    int b = idx + 1;
                    int c = a + verNumRow; // not segments_radius
                    int d = b + verNumRow; // not segments_radius
                    
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);

                    triangles.Add(d);
                    triangles.Add(c);
                    triangles.Add(b);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private void GenerateRingZ(int _verNumRow, float _top, float _radius, float _height,
            List<Vector3> _vertices, List<Vector2> _uvs, List<Vector3> _normals, List<Color> _colors, bool _isSide, bool _isLast) {
            for(int i = 0; i < _verNumRow; i++) {
                float ratio = (float)i / (_verNumRow - 1);  // 0~2PI, 0-2PI will weld vertices
                float rad = ratio * TWO_PI;

                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                float x = cos * _radius;
                float y = sin * _radius;

                _vertices.Add(new Vector3(x, y, _top));     // top: 0~height
                _uvs.Add(new Vector2(ratio, _top/_height)); // align by UV.y
                _normals.Add(new Vector3(cos, sin, 0));
                if(!_isLast) {
                    _colors.Add(new Color(0, 0, 0));  // Use Colors as a vertex id
                } else {
                    _colors.Add(new Color(1, 0, 0));
                }
            }
        }
        private void GenerateRingY(int _verNumRow, float _top, float _radius, float _height,
            List<Vector3> _vertices, List<Vector2> _uvs, List<Vector3> _normals, List<Color> _colors, bool _isSide, bool _isLast) {
            for(int i = 0; i < _verNumRow; i++) {
                float ratio = (float)i / (_verNumRow - 1);  // 0~2PI, 0-2PI will weld vertices
                float rad = ratio * TWO_PI;

                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                float x = cos * _radius;
                float z = sin * _radius;

                _vertices.Add(new Vector3(x, _top, z));     // top: 0~height
                _uvs.Add(new Vector2(ratio, _top / _height)); // align by UV.y
                _normals.Add(new Vector3(cos, 0f, sin));
                if(!_isLast) {
                    _colors.Add(new Color(0, 0, 0));  // Use Colors as a vertex id
                } else {
                    _colors.Add(new Color(1, 0, 0));
                }
            }
        }
        #endregion


        #region Set Textures UV
        private void GetShaderPropertyID() {  // Awake
            if(GraphicsSettings.currentRenderPipeline == null){
                prop_TexAlbedo = Shader.PropertyToID("_MainTex");
            } else {
                prop_TexAlbedo = Shader.PropertyToID("_BaseMap");
            }
            prop_TexNormal = Shader.PropertyToID("_BumpMap");
            prop_TexMetallicGloss = Shader.PropertyToID("_MetallicGlossMap");
            prop_TexAmbientOcclusion = Shader.PropertyToID("_OcclusionMap");
            prop_TexEmission = Shader.PropertyToID("_EmissionMap");
        }
        private void ApplyTextureTilingOffset() {  // Update
            if(cachedTiling != TextureTiling) {
                instanceMaterial.SetTextureScale(prop_TexAlbedo, TextureTiling);
                instanceMaterial.SetTextureScale(prop_TexNormal, TextureTiling);
                instanceMaterial.SetTextureScale(prop_TexMetallicGloss, TextureTiling);
                instanceMaterial.SetTextureScale(prop_TexAmbientOcclusion, TextureTiling);
                instanceMaterial.SetTextureScale(prop_TexEmission, TextureTiling);
                cachedTiling = TextureTiling;
            }
            if(cachedOffset != TextureOffset) {
                instanceMaterial.SetTextureOffset(prop_TexAlbedo, TextureOffset);
                instanceMaterial.SetTextureOffset(prop_TexNormal, TextureOffset);
                instanceMaterial.SetTextureOffset(prop_TexMetallicGloss, TextureOffset);
                instanceMaterial.SetTextureOffset(prop_TexAmbientOcclusion, TextureOffset);
                instanceMaterial.SetTextureOffset(prop_TexEmission, TextureOffset);
                cachedOffset = TextureOffset;
            }
        }
        #endregion


        #region OnEnable&Disable
        private void OnDestroy() {
            if(argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
        private void OnDisable() {
            if(argsBuffer != null)
                argsBuffer.Release();
            argsBuffer = null;
        }
        #endregion

    } // class
} // namespace
