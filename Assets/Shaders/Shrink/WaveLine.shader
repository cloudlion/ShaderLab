Shader "Unlit/WaveLine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HeightFactor("Factor", float) = 0.005
		_LengthFactor ("LengthFactor", float) = 32
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/Shaders/cginc/CustomLib.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;  
			float _LengthFactor;
			float _HeightFactor;


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv =TRANSFORM_TEX(v.texcoord, _MainTex); 
				return o;
			}



			fixed4 frag (v2f i) : SV_Target
			{
				float2 dv = float2(0.0, 0.0) - i.uv.xy;
		//		float dist = sqrt(dv.x * dv.x + dv.y * dv.y);
				float dist = i.uv.x;
				float PI = 3.1415;
				float sinFactor =  (sin((dist- _Time.y )*_LengthFactor*PI ));
				i.uv.y = i.uv.y + sinFactor*0.1;
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
