Shader "Custom/Particle" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Id ("ID", int) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma only_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			
			StructuredBuffer<float2> Positions;

			struct vsin {
				float4 vertex : POSITION;
			};
			struct vs2ps {
				float4 vertex : POSITION;
			};

			fixed4 _Color;
			int _Id;
			
			vs2ps vert(vsin IN) {
				float3 vWorld = mul(_Object2World, IN.vertex).xyz;
				vWorld.xy += Positions[_Id];
				
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
