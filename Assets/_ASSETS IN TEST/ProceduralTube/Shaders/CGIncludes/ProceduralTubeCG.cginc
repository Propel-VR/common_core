/////////////////////////////////////////////////////
/// Below values will be used for
/// CalcBezierCurve.compute and
/// ProceduralTubeShader.shader
//////////////////////////////////////////////////////


struct Node {
	float3 pos;
	float radius;
};

struct Handle {
	float3 p0;  // curve start position
	float3 p1;  // curve start handle
	float3 p2;  // curve end handle
	float3 p3;  // curve end position
	float radiusStart;  // radius of this node
	float radiusEnd;    // radius of the next node
	int taperType;      // 0: Linear, 1:Exponential, 2: InverseExponential, 3: Smooth, 4: Symmetry 
};

uint _CurveNum;        // Total Curve Number gathered from ProceduralTubeManager.cs
uint _NodeNumPerCurve; // Node number per curve
uint _NodeNum;         // Total Node Number calculated from ProceduralTubeManager.cs

float _mTime;          // _Time is being used by Unity's Surface Shader



// -----------------------------------------------------------------------
/// Below functions are to calculate Tube's vertex position / normal / UV
// -----------------------------------------------------------------------

float4x4 eulerAnglesToRotationMatrix(float3 angles) {
	float ch = cos(angles.y); float sh = sin(angles.y); // heading
	float ca = cos(angles.z); float sa = sin(angles.z); // attitude
	float cb = cos(angles.x); float sb = sin(angles.x); // bank

	// Ry-Rx-Rz (Yaw Pitch Roll)
	return float4x4(
		ch * ca + sh * sb * sa, -ch * sa + sh * sb * ca, sh * cb, 0,
		cb * sa, cb * ca, -sb, 0,
		-sh * ca + ch * sb * sa, sh * sa + ch * sb * ca, ch * cb, 0,
		0, 0, 0, 1
		);
}

float4x4 directionToRotationMatrix(float3 dir) {
	float rotY = atan2(dir.x, dir.z);
	float rotX = -asin(dir.y / (length(dir.xyz) + 1e-8));
	float3 angles = float3(rotX, rotY, 0);

	float ch = cos(angles.y); float sh = sin(angles.y); // heading
	float ca = cos(angles.z); float sa = sin(angles.z); // attitude
	float cb = cos(angles.x); float sb = sin(angles.x); // bank

	// Ry-Rx-Rz (Yaw Pitch Roll)
	return float4x4(
		ch*ca + sh * sb*sa, -ch * sa + sh * sb*ca, sh*cb, 0,
		cb*sa, cb*ca, -sb, 0,
		-sh * ca + ch * sb*sa, sh*sa + ch * sb*ca, ch*cb, 0,
		0, 0, 0, 1
		);
}

float4x4 directionToRotationMatrix2(float3 dir1, float3 dir2) {
	float rotY1 = atan2(dir1.x, dir1.z);
	float rotX1 = -asin(dir1.y / (length(dir1.xyz) + 1e-8));

	float rotY2 = atan2(dir2.x, dir2.z);
	float rotX2 = -asin(dir2.y / (length(dir2.xyz) + 1e-8));

	float diff = rotY2 - rotY1;
	float offsetZ = -diff;

	float3 angles = float3(rotX2, rotY2, offsetZ);

	float ch = cos(angles.y); float sh = sin(angles.y); // heading
	float ca = cos(angles.z); float sa = sin(angles.z); // attitude
	float cb = cos(angles.x); float sb = sin(angles.x); // bank

	// Ry-Rx-Rz (Yaw Pitch Roll)
	return float4x4(
		ch*ca + sh * sb*sa, -ch * sa + sh * sb*ca, sh*cb, 0,
		cb*sa, cb*ca, -sb, 0,
		-sh * ca + ch * sb*sa, sh*sa + ch * sb*ca, ch*cb, 0,
		0, 0, 0, 1
		);
}


void calcIDs(int id, out int id1, out int id2, out int id3){
	int curveIdx = floor(id / _NodeNumPerCurve);   // 0(0~7), 1(8~15), 2(16~23), 3, 4...
	int minID = curveIdx * _NodeNumPerCurve;       // 0, 8, 16, 24..
	int maxID = minID + (_NodeNumPerCurve - 1);    // 7, 15, 23, 31..
	id1 = clamp(id + 0, minID, maxID);
	id2 = clamp(id + 1, minID, maxID);
	id3 = clamp(id + 2, minID, maxID);
}

float4 calcVertexPositionToTube(float4 vertex, Node node1, Node node2, Node node3, int id1, int id2, int id3, float mask){
	
	float radius1 = node1.radius;
	float radius2 = node2.radius;
	float3 p1 = node1.pos;  // 0 1.. 5 6 7
	float3 p2 = node2.pos;  // 1 2.. 6 7 7
	float3 p3 = node3.pos;  // 2 3.. 7 7 7
	float3 dir1 = normalize(p2 - p1);
	float3 dir2 = normalize(p3 - p2);
	if (id3 - id2 == 0) {
		dir2 = dir1;
	}

	float4 vPos = vertex;

	float3 diffp2p1 = (p2 - p1); // difference between p1 and p2
	// Move difference of p1 and p2 first
	vPos.xyz += ((diffp2p1 - float3(0, 0, 1)) * mask); // -float3(0,0,1) is to remove the z(1) value,


	float4x4 rotMatrix;

	// seperate the rotation value by z value 0 and 1
	if (mask != 0) { // Rotate of p2->p3
		// Move to the center(0,0,0) first
		vPos.xyz -= diffp2p1;
		vPos.xyz *= radius2;   // Scale
		rotMatrix = directionToRotationMatrix2(dir1, dir2);
		vPos = mul(rotMatrix, vPos);
		// Back to the its position
		vPos.xyz += diffp2p1;
	} else {           // Rotate of p1->p2
	    // vertices that has z=0 already have their center (0,0,0).
		vPos.xyz *= radius1;   // Scale
		rotMatrix = directionToRotationMatrix(dir1);
		vPos = mul(rotMatrix, vPos);
	}

	// Move all the vertices to p1
	vPos.xyz += p1;

	return vPos;
}

float3 calcNormalToTube(float3 vertex, float3 p1, float3 p2, float mask){
	return (vertex - (mask==0 ? p1 : p2));
}

float calcUV_Y(float y, int id, float UVmult){
	y *= UVmult;
	y += (UVmult * (id % _NodeNumPerCurve));
	return y;
}