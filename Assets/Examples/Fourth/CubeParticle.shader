Shader "Custom/CubeParticle" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300
	
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert

#define COLORS_COUNT 7
static const float4 colors[COLORS_COUNT] = {
	float4(0, 0, 1, 1),
	float4(0, 1, 0, 1),
	float4(0, 1, 1, 1),
	float4(1, 0, 0, 1),
	float4(1, 0, 1, 1),
	float4(1, 1, 0, 1),
	float4(1, 1, 1, 1)
};

sampler2D _MainTex;
fixed4 _Color;
half _Shininess;

struct Input {
	float2 uv_MainTex;
	float4 color;
};

void vert(inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	uint id = (uint)round(v.texcoord1.x);
	o.color = colors[id % COLORS_COUNT];
}
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb * IN.color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
}
ENDCG
}

Fallback "Legacy Shaders/VertexLit"
}
