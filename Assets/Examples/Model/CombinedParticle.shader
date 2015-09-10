Shader "Custom/CombinedParticle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Angular ("Angular Speed", Vector) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert
		#pragma target 3.0
		
		static const float3 HIDDEN_POSITION = float3(10000, 0, 0);
		
		#ifdef SHADER_API_D3D11
		#include "../../Packages/ParticlePhysics/Lib/DataTypes.cginc"
		#include "../../Packages/ParticlePhysics/Lib/Quaternion.cginc"
		StructuredBuffer<float2> Positions;
		StructuredBuffer<float> Lifes;
		StructuredBuffer<Collision> Collisions;
		#endif

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _Angular;

		struct Input {
			float2 uv_MainTex;
			float4 color;
		};
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			#ifdef SHADER_API_D3D11
			int id = round(v.texcoord1.x);
			float life = Lifes[id];
			//Collision c = Collisions[id];
			//o.color = lerp(float4(0, 0, 0, 1), float4(1, 0, 0, 1), c.count / 5.0);
			float2 pos = Positions[id];
			float3 worldPos = mul(_Object2World, float4(v.vertex.xyz, 1)).xyz + float3(pos, 0);
			if (life <= 0)
				worldPos = HIDDEN_POSITION;
			v.vertex.xyz = mul(_World2Object, float4(worldPos, 1)).xyz;
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Emission = IN.color.rgb;
		}
		ENDCG
	} 
	FallBack Off
}
