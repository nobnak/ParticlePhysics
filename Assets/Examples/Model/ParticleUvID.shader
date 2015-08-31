Shader "Custom/ParticleUvID" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma only_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			
			static const float3 HIDDEN_POSITION = float3(10000, 0, 0);
			
			StructuredBuffer<float2> Positions;
			StructuredBuffer<float> Lifes;

			struct vsin {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};
			struct vs2ps {
				float4 vertex : POSITION;
			};

			fixed4 _Color;
			
			vs2ps vert(vsin IN) {
				int id = round(IN.uv2.x);
				float life = Lifes[id];
				float3 vWorld = mul(_Object2World, IN.vertex).xyz;
				if (life > 0)
					vWorld.xy += Positions[id];
				else
					vWorld.xyz = HIDDEN_POSITION;
				
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_VP, float4(vWorld, 1));
				return OUT;
			}
			float4 frag(vs2ps IN) : COLOR {
				return _Color;
			}
			ENDCG
		}
	} 
	FallBack Off
}
