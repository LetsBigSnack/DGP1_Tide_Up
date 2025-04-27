Shader "Custom/WaterHeightColor_Transparent"
{
    Properties
    {
        _ColorLow ("Low Color", Color) = (0, 0.5, 1, 1)
        _ColorHigh ("High Color", Color) = (0, 1, 1, 1)
        _HeightMin ("Min Height", Float) = 0
        _HeightMax ("Max Height", Float) = 1
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Float) = 1.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "WaterPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            float4 _ColorLow;
            float4 _ColorHigh;
            float _HeightMin;
            float _HeightMax;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            float _NormalStrength;
            float _Smoothness;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _NormalMap);
                OUT.height = IN.positionOS.y;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = normalize(GetCameraPositionWS() - worldPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float t = saturate((IN.height - _HeightMin) / (_HeightMax - _HeightMin));
                float4 baseColor = lerp(_ColorLow, _ColorHigh, t);

                
                float3 normalTS = UnpackNormal(tex2D(_NormalMap, IN.uv)) * _NormalStrength;
                float3 normalWS = normalize(mul(normalTS, (float3x3)UNITY_MATRIX_IT_MV));

                
                float fresnel = pow(1.0 - saturate(dot(IN.viewDirWS, normalWS)), 3.0);

                
                half4 finalColor = baseColor;
                finalColor.a = lerp(0.3, 0.7, fresnel);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
