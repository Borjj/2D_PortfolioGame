Shader "Custom/Vignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _Radius ("Radius", Range(0.1, 3)) = 1
        _VignetteColor ("Vignette Color", Color) = (0, 0, 0, 1)
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

            sampler2D _MainTex;
            float _Intensity;
            float _Radius;
            float4 _VignetteColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Calculate distance from center (0,0) to current UV coordinate
                float2 uv = i.uv * 2.0 - 1.0;
                float dist = length(uv);
                
                // Create smooth vignette mask
                float vignette = dist * _Radius;
                vignette = smoothstep(0.4, 1.4, vignette);
                vignette *= _Intensity;
                
                // Blend original color with vignette
                return lerp(col, _VignetteColor, vignette);
            }
            ENDCG
        }
    }
}