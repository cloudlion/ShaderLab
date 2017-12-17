// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Stone"
{
	Properties{  
        _Diffuse("Diffuse", Color) = (1,1,1,1)  
        _MainTex("Base 2D", 2D) = "white"{}  
        _DissolveMap("DissolveMap", 2D) = "white"{}  
    	_DissolveThreshold("Threshold", Range(0,1)) = 0
    }  
      
    CGINCLUDE  
    #include "Lighting.cginc"  
    uniform fixed4 _Diffuse;   
    uniform sampler2D _MainTex;  
    uniform float4 _MainTex_ST;  
    uniform sampler2D _DissolveMap;  
    uniform float _DissolveThreshold;  

      
    struct v2f  
    {  
        float4 pos : SV_POSITION;  
        float3 worldNormal : TEXCOORD0;  
        float2 uv : TEXCOORD1;  
    };  
      
    v2f vert(appdata_base v)  
    {  
        v2f o;  
        o.pos = UnityObjectToClipPos(v.vertex);  
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  
        o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);  
        return o;  
    }  
      
    fixed4 frag(v2f i) : SV_Target  
    {  
        //Dissolve Map  
        fixed4 dissolveValue = tex2D(_DissolveMap, i.uv);  
        //discard  
    
        //Diffuse + Ambient light  
        fixed3 worldNormal = normalize(i.worldNormal);  
        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);  
        fixed3 lambert = saturate(dot(worldNormal, worldLightDir));  
        fixed3 albedo = lambert * _Diffuse.xyz * _LightColor0.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;  
        fixed4 colorBase = tex2D(_MainTex, i.uv);
        if (dissolveValue.r <= _DissolveThreshold)  
        {  
            colorBase.rgb = dot(colorBase.rgb, fixed3(.222,.707,.071)); 
        } 
       
        colorBase.rgb = colorBase.rgb * albedo;  
  
        return colorBase;
    }  
    ENDCG  
      
    SubShader  
    {  
        Tags{ "RenderType" = "Opaque" }  
        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag     
            ENDCG  
        }  
    }  
    FallBack "Diffuse" 
}
