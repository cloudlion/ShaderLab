// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Shrink"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Texture", 2D) = "white" {}
		_DistortFactor ("Factor", float) = 1
		_DistortStrength ("Strength", float) = 1
		_DistortStrength ("Center", Vector) = (0.5, 0.5, 1, 1)
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float _DistortFactor; //扭曲强度
			float4 _DistortCenter; //扭曲中心点xy值(0-1)屏幕空间
			float _DistortStrength;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 dir = i.uv - _DistortCenter.xy;

//最终偏移的值：方向 * (1-长度)，越靠外偏移越小

				float2 scaleOffset = _DistortFactor * normalize(dir) * (1 - length(dir));

//采样Noise贴图

				fixed4 noise = tex2D(_NoiseTex, i.uv);

//noise的权重 = 参数 * 距离，越靠近外边的部分，扰动越严重

				float2 noiseOffset = noise.xy * _DistortStrength * dir;

//计算最终offset = 两种扭曲offset的差(取和也行，总之效果好是第一位的)

				float2 offset = scaleOffset - noiseOffset;

//计算采样uv值：正常uv值+从中间向边缘逐渐增加的采样距离

				float2 uv = i.uv + offset;

				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
}
