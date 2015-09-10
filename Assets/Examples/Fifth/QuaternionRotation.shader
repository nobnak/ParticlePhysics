Shader "Custom/QuaternionRotation" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Angular ("Angular Speed", Vector) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0
		#include "../../Packages/ParticlePhysics/Lib/Quaternion.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		#ifdef SHADER_API_D3D11
		StructuredBuffer<float3> Positions;
		StructuredBuffer<float4> Rotations;
		#endif
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			#ifdef SHADER_API_D3D11
			int id = round(v.texcoord1.x);
			
			float3 pos = Positions[id];
			float4 q = Rotations[id];
			
			v.vertex.xyz = qrotate(q, v.vertex.xyz);
			v.normal.xyz = qrotate(q, v.normal.xyz);
			float3 posWorld = mul(_Object2World, float4(v.vertex.xyz, 1.0)).xyz + pos;
			v.vertex.xyz = mul(_World2Object, float4(posWorld, 1.0));
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack Off
}
