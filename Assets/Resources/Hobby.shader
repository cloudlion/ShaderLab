// Upgrade NOTE: replaced 'defined HOBBY_ON' with 'defined (HOBBY_ON)'

Shader "Unlit/Hobby"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(HOBBY_ON)] _HOBBY("Hobby?", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
	//		#pragma multi_compile_fog
			#pragma shader_feature HOBBY_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
		//		UNITY_APPLY_FOG(i.fogCoord, col);
				#ifdef HOBBY_ON
					col = fixed4(0.0, 0.0, 0.5,1);
					//col = (0.5, 0.5, 0.5, 1.0);
				#elif HOBBY_OFF
					col = fixed4(1, 0.5, 1,1);
				#else
					col = fixed4(1, 0, 0, 0.5);
				#endif 
			//	col = fixed4(0, 0, 1, 0.5);
				return col;
			}
			ENDCG
		}
	}
//	CustomEditor "CustomShaderGUI"
}
