// -----------------------------------------------------------------------
// Universal RP - SimpleLitForwardPass
// -----------------------------------------------------------------------

#include "ProceduralTubeCG.cginc"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"

StructuredBuffer<Node> _NodeBuffer;
float _UVmult;

// Used in Standard (Simple Lighting) shader
Varyings LitPassVertexSimple_ProceduralTube(Attributes input, uint unity_InstanceID : SV_InstanceID)
{
    int id1, id2, id3;
	calcIDs(unity_InstanceID, id1, id2, id3);

	Node node1 = _NodeBuffer[id1];
	Node node2 = _NodeBuffer[id2];
	Node node3 = _NodeBuffer[id3];
	float mask = input.positionOS.z; // generated instanceTubeMesh has z position 0 and 1, and use this as mask.

    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs( calcVertexPositionToTube(input.positionOS, node1, node2, node3, id1, id2, id3, mask).xyz );
    VertexNormalInputs normalInput = GetVertexNormalInputs(calcNormalToTube(vertexInput.positionWS.xyz, node1.pos, node2.pos, mask), input.tangentOS);
    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    float2 uv = input.texcoord;
    uv.y = calcUV_Y(uv.y, unity_InstanceID, _UVmult);
    output.uv = TRANSFORM_TEX(uv, _BaseMap);
    output.posWS.xyz = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;

#ifdef _NORMALMAP
    output.normal = half4(normalInput.normalWS, viewDirWS.x);
    output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
    output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
#else
    output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
    output.viewDir = viewDirWS;
#endif

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normal.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
#endif

    return output;
}