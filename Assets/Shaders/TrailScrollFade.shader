Shader "Custom/TrailScrollFade"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _FadeStrength ("Fade Strength", Range(0, 1)) = 0.98
        _UVOffsetX ("UV Offset X", Float) = 0
        _UVOffsetY ("UV Offset Y", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _FadeStrength;
            float _UVOffsetX;
            float _UVOffsetY;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float2 offsetUV = i.uv + float2(_UVOffsetX, _UVOffsetY);
                float4 color = tex2D(_MainTex, offsetUV);
                return color * _FadeStrength;
            }
            
            ENDCG
        }
    }
}
