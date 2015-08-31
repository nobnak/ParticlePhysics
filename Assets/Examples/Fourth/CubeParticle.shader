Shader "Custom/CubeParticle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
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

		struct Input {
			float2 uv_MainTex;
			float4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			int id = round(v.texcoord1.x);
			o.color = colors[id % COLORS_COUNT];
		}
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
