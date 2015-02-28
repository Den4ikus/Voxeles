// -----------------------------------------------------------------------------
// 
// 
// -----------------------------------------------------------------------------

//StructuredBuffer<float3> RefVectorsBuffer : register(b0);
TextureCube skybox : register(t0);

SamplerState SkyboxSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

static float3 CUBE_MIN = float3(-1.0f, -1.0f, -1.0f);
static float3 CUBE_MAX = float3(1.0f, 1.0f, 1.0f);

float4 VS(float2 pos: POSITION, float3 start: TEXCOORD0, float3 end: TEXCOORD1, out float3 outStart : TEXCOORD0, out float3 outDir : TEXCOORD1): SV_POSITION
{//
	//coords = float2(pos.x * .5f  + .5f, -pos.y * .5f + .5f);
	outStart = start;
	outDir = end - start;
	return float4(pos, 0.0f, 1.0f);
}

float4 PS(float4 pos : SV_POSITION, float3 start : TEXCOORD0, float3 dir : TEXCOORD1): SV_Target
{
	//float3 camPos = RefVectorsBuffer[0];
	//float3 v1 = lerp(RefVectorsBuffer[1], RefVectorsBuffer[2], float3(coords.y,coords.y,coords.y));//float3(coords.x,coords.x,coords.x));
	//float3 v2 = lerp(RefVectorsBuffer[3], RefVectorsBuffer[4], float3(coords.y,coords.y,coords.y));//float3(coords.x,coords.x,coords.x));
	//float3 ray = lerp(v2, v1, float3(coords.x,coords.x,coords.x));//float3(coords.y,coords.y,coords.y));

	//float3 dir = (ray - camPos);
	float3 tMin = (CUBE_MIN - start) / dir;
    float3 tMax = (CUBE_MAX - start) / dir;
    float3 t1 = min(tMin, tMax);
    float3 t2 = max(tMin, tMax);
    float tNear = max(max(t1.x, t1.y), t1.z);
    float tFar = min(min(t2.x, t2.y), t2.z);

	if(tFar - tNear < 0 || tNear < 0) return skybox.Sample(SkyboxSampler, dir);
	//clip(tFar - tNear);
	
	float3 p = tNear * dir + start;
    float3 l = normalize (float3(8.0f, 10.0f, 10.0f) - p); // vector to light source
    float3 n = normalize(float3(step(t1.y, t1.x)*step(t1.z, t1.x), step(t1.x, t1.y)*step(t1.z, t1.y), step(t1.x, t1.z)*step(t1.y, t1.z)));
	return float4(1.0f, 1.0f, 1.0f, 1.0f) * max(dot(n, l), 0.0f);
}

float4 PS_Skybox(float4 pos : SV_POSITION, float3 start : TEXCOORD0, float3 dir : TEXCOORD1) : SV_Target
{
	return skybox.Sample(SkyboxSampler, dir);
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}