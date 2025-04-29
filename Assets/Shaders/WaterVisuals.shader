Shader "Custom/WaterHeightColor_SeaLevelFoam_IntersectionFixed"
{
    Properties
    {
        _ColorShallow ("Shallow Color", Color) = (0, 0.7, 1, 1)
        _ColorDeep ("Deep Color", Color) = (0, 0.3, 0.8, 1)
        _SeaLevel ("Sea Level", Float) = 0
        _ShallowDepth ("Shallow Depth Above Sea Level", Float) = 3
        _DeepDepth ("Deep Depth Below Sea Level", Float) = -3

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Float) = 1.0
        _NormalSpeed ("Normal Speed", Vector) = (0.05, 0.03, 0, 0)

        _RefractionStrength ("Refraction Strength", Range(0,0.1)) = 0.02

        _FoamTex ("Foam Texture", 2D) = "white" {}
        _FoamThreshold ("Foam Threshold", Float) = 0.0
        _FoamRange ("Foam Range", Float) = 1.0
        _FoamIntensity ("Foam Intensity", Range(0, 1)) = 1.0

        _IntersectionFoamSharpness ("Intersection Foam Sharpness", Float) = 50.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
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
            #pragma require _CameraDepthTexture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 tangentWS : TEXCOORD4;
                float3 bitangentWS : TEXCOORD5;
            };

            float4 _ColorShallow;
            float4 _ColorDeep;
            float _SeaLevel;
            float _ShallowDepth;
            float _DeepDepth;

            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            float _NormalStrength;
            float4 _NormalSpeed;
            float _RefractionStrength;

            sampler2D _FoamTex;
            float _FoamThreshold;
            float _FoamRange;
            float _FoamIntensity;

            float _IntersectionFoamSharpness;

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            // Correct screen UV function
            float2 GetScreenUV(float4 positionCS)
            {
                float2 uv = positionCS.xy / positionCS.w;
                uv = uv * 0.5 + 0.5;
#if UNITY_UV_STARTS_AT_TOP
                uv.y = 1.0 - uv.y;
#endif
                return uv;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _NormalMap);
                OUT.height = IN.positionOS.y;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = normalize(GetCameraPositionWS() - worldPos);

                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.tangentWS = normalize(TransformObjectToWorldDir(IN.tangentOS.xyz));
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Animate normal map
                float2 animatedUV = IN.uv + _Time.y * _NormalSpeed.xy;
                float3 normalTS = UnpackNormal(tex2D(_NormalMap, animatedUV)) * _NormalStrength;
                float3x3 TBN = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 normalWS = normalize(mul(normalTS, TBN));

                float2 refractedUV = IN.uv + normalTS.xy * _RefractionStrength;

                float fresnel = pow(1.0 - saturate(dot(IN.viewDirWS, normalWS)), 5.0);

                // Calculate water height color
                float shallowBorder = _SeaLevel + _ShallowDepth;
                float deepBorder = _SeaLevel + _DeepDepth;
                float t = saturate((IN.height - deepBorder) / (shallowBorder - deepBorder));
                float4 waterColor = lerp(_ColorDeep, _ColorShallow, t);

                // Calculate basic foam
                float foamMask = saturate((IN.height - (_SeaLevel + _FoamThreshold)) / _FoamRange);
                foamMask = pow(foamMask, 2.0); // Make foam softer
                float foamAmount = foamMask * _FoamIntensity;
                float4 foamColor = tex2D(_FoamTex, refractedUV);

                // Depth intersection foam
                float2 screenUV = GetScreenUV(IN.positionCS);
                float sceneDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;
                float linearSceneDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
                float linearWaterDepth = LinearEyeDepth(IN.positionCS.z / IN.positionCS.w, _ZBufferParams);
                float depthDiff = saturate((linearWaterDepth - linearSceneDepth) * _IntersectionFoamSharpness);

                // Animate intersection foam
                float intersectionFoam = depthDiff;
                intersectionFoam *= 0.5 + 0.5 * sin(_Time.y * 4.0 + IN.positionCS.x * 0.1 + IN.positionCS.y * 0.1);

                // Combine foam effects
                float finalFoam = saturate(foamAmount + intersectionFoam * _FoamIntensity);
                waterColor.rgb = lerp(waterColor.rgb, 1.0, finalFoam * foamColor.r);

                // Final Fresnel effect
                half4 finalColor = waterColor;
                finalColor.rgb = lerp(finalColor.rgb, 1.0, fresnel * 0.5);
                finalColor.a = lerp(0.3, 0.7, fresnel);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
