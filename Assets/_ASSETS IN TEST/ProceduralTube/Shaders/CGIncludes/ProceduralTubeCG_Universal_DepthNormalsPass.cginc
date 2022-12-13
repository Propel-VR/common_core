// -----------------------------------------------------------------------
// Universal RP - DepthNormalsPass
// -----------------------------------------------------------------------

#include "ProceduralTubeCG.cginc"
#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"

StructuredBuffer<Node> _NodeBuffer;
float _UVmult;

Varyings DepthNormalsVertex_ProceduralTube(Attributes input, uint unity_InstanceID : SV_InstanceID)
{
    int id1, id2, id3;
    calcIDs(unity_InstanceID, id1, id2, id3);

    Node node1 = _NodeBuffer[id1];
    Node node2 = _NodeBuffer[id2];
    Node node3 = _NodeBuffer[id3];
    float mask = input.positionOS.z; // generated instanceTubeMesh has z position 0 and 1, and use this as mask.

    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.uv         = TRANSFORM_TEX(input.texcoord, _BaseMap);
    input.positionOS.xyz = calcVertexPositionToTube(input.positionOS, node1, node2, node3, id1, id2, id3, mask).xyz;
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

    return output;
}