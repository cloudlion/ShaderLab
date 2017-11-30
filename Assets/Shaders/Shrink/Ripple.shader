Shader "Hidden/Ripple"
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/Shaders/cginc/CustomLib.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 grabPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex); 
				return o;
			}

//			sampler2D _GrabTempTex;  
//            float4 _GrabTempTex_ST; 

			sampler2D _MainTex;
			float _LengthFactor;
			float _HeightFactor;

			fixed4 frag (v2f i) : SV_Target
			{
				i.grabPos.xy = i.grabPos.xy + RippleOffset(i.grabPos.xy, 0, _LengthFactor, _HeightFactor);
				fixed4 col = tex2Dproj(_MainTex, i.grabPos);
				return col;
			}
			ENDCG
		}
	}
}
