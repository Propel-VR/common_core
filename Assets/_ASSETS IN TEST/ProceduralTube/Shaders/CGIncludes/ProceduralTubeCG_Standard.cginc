
#include "ProceduralTubeCG.cginc"

#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
#include "UnityInstancing.cginc"

// -----------------------------------------------------------------------
// Legacy RP - Standard Shader
// -----------------------------------------------------------------------

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	StructuredBuffer<Node> _NodeBuffer;
	float _UVmult;
#endif

struct appdata_tan_color {
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float3 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

void vert_tube(inout appdata_tan_color v) {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

	int id1, id2, id3;
	calcIDs(unity_InstanceID, id1, id2, id3);

	Node node1 = _NodeBuffer[id1];
	Node node2 = _NodeBuffer[id2];
	Node node3 = _NodeBuffer[id3];
	float mask = v.vertex.z; // generated instanceTubeMesh has z position 0 and 1, and use this as mask.

	v.vertex = calcVertexPositionToTube(v.vertex, node1, node2, node3, id1, id2, id3, mask);

	v.normal = calcNormalToTube(v.vertex.xyz, node1.pos, node2.pos, mask);

	v.texcoord.y = calcUV_Y(v.texcoord.y, unity_InstanceID, _UVmult);
#endif
}

// instancing_options procedural:setup
void setup() {

}