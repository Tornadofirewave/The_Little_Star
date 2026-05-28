Shader "Custom/NightGlowTilemap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _RendererColor ("Renderer Color", Color) = (1, 1, 1, 1)
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _GlowTiling ("Glow Tiling", Vector) = (0.25, 0.25, 0, 0)
        _ColorA ("Glow Color A", Color) = (0.05, 0.45, 0.85, 1)
        _ColorB ("Glow Color B", Color) = (0.8, 0.08, 0.35, 1)
        _ColorC ("Glow Color C", Color) = (0.45, 0.12, 0.8, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 1)) = 0.75
        _ColorSpeed ("Color Speed", Range(0, 5)) = 1
        _WhiteThreshold ("White Threshold", Range(0, 1)) = 0.55
        _OverlayStrength ("Overlay Strength", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                half2 lightingUV : TEXCOORD1;
                float2 worldXY : TEXCOORD2;
                #if defined(DEBUG_DISPLAY)
                float3 positionWS : TEXCOORD3;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_GlowTex);
            SAMPLER(sampler_GlowTex);

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MaskTex_ST;
                float4 _GlowTex_ST;
                float4 _Color;
                float4 _RendererColor;
                float4 _GlowTiling;
                float4 _ColorA;
                float4 _ColorB;
                float4 _ColorC;
                float _GlowIntensity;
                float _ColorSpeed;
                float _WhiteThreshold;
                float _OverlayStrength;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #ifdef UNITY_INSTANCING_ENABLED
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteFlip);
                #endif

                float3 worldPos = TransformObjectToWorld(input.positionOS);
                output.positionCS = TransformWorldToHClip(worldPos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.lightingUV = half2(ComputeScreenPos(output.positionCS / output.positionCS.w).xy);
                output.color = input.color * _Color * _RendererColor;
                #ifdef UNITY_INSTANCING_ENABLED
                output.color *= unity_SpriteColor;
                #endif
                output.worldXY = worldPos.xy;
                #if defined(DEBUG_DISPLAY)
                output.positionWS = worldPos;
                #endif
                return output;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 Frag(Varyings input) : SV_Target
            {
                half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 baseColor = sprite * input.color;

                float2 glowUV = input.worldXY * _GlowTiling.xy + _Time.y * float2(0.08, 0.05);
                half4 glowSample = SAMPLE_TEXTURE2D(_GlowTex, sampler_GlowTex, TRANSFORM_TEX(glowUV, _GlowTex));
                half glowMask = saturate(dot(glowSample.rgb, half3(0.3333, 0.3333, 0.3333)));

                half pulse = sin(_Time.y * _ColorSpeed) * 0.5h + 0.5h;
                half3 shiftedColor = lerp(lerp(_ColorA.rgb, _ColorB.rgb, pulse), _ColorC.rgb, glowMask);

                half spriteBrightness = max(max(sprite.r, sprite.g), sprite.b);
                half spriteWhiteness = min(min(sprite.r, sprite.g), sprite.b);
                half whiteMask = smoothstep(_WhiteThreshold, 1.0h, spriteWhiteness) * spriteBrightness * baseColor.a;

                half3 texturedGlow = shiftedColor * lerp(half3(1.0h, 1.0h, 1.0h), glowSample.rgb, 0.65h) * _GlowIntensity;
                half overlayAmount = saturate(whiteMask * _OverlayStrength);
                half3 recolored = lerp(baseColor.rgb, texturedGlow, overlayAmount);

                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(saturate(recolored), baseColor.a, mask, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);

                #if defined(DEBUG_DISPLAY)
                half4 debugColor = 0;
                SETUP_DEBUG_DATA_2D(inputData, input.positionWS);
                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }
    }
}
