Shader "Custom/StarField"
{
    Properties
    {
        _MainTex      ("Sprite Texture", 2D)    = "white" {}
        _StarDensity  ("Star Density",   Float) = 20.0
        _StarSize     ("Star Size",      Float) = 0.05
        _TwinkleSpeed ("Twinkle Speed",  Float) = 2.0
        _StarColor    ("Star Color",     Color) = (1, 0.95, 0.8, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "RenderType"      = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float  _StarDensity;
                float  _StarSize;
                float  _TwinkleSpeed;
                float4 _StarColor;
            CBUFFER_END

            // --------------------------------------------------------
            // Returns two pseudo-random floats in [0,1] for a given
            // 2D cell coordinate. Each cell always gets the same values
            // so star positions are stable across frames.
            // --------------------------------------------------------
            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            // --------------------------------------------------------
            // Divides UV space into a grid. Each cell contains one
            // candidate star at a random sub-cell position. Returns
            // a brightness value in [0,1] for the current pixel.
            // Neighbouring cells are sampled so stars near cell edges
            // are not clipped.
            // --------------------------------------------------------
            float StarField(float2 uv)
            {
                float2 grid   = uv * _StarDensity;
                float2 cellID = floor(grid);
                float2 cellUV = frac(grid);

                float result = 0.0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset  = float2(x, y);
                        float2 h       = hash2(cellID + offset);

                        // h.xy is the star's random position inside its cell
                        float2 diff = cellUV - offset - h;
                        float  dist = length(diff);

                        // Hard circular dot — smoothstep fades the edge
                        float brightness = smoothstep(_StarSize, 0.0, dist);

                        // Each star gets a unique speed and phase so they
                        // twinkle independently
                        float speed = h.x * _TwinkleSpeed * 2.0 + _TwinkleSpeed * 0.5;
                        float phase = h.y * 6.28318; // 2*PI
                        float pulse = sin(_Time.y * speed + phase) * 0.5 + 0.5;

                        // Clamp minimum brightness so stars never fully vanish
                        pulse = lerp(0.2, 1.0, pulse);

                        result += brightness * pulse;
                    }
                }

                return saturate(result);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float stars = StarField(IN.uv);

                // Black base, star colour layered on top.
                // Fully opaque — the sprite mesh defines the shape.
                half4 col  = half4(0.0, 0.0, 0.0, IN.color.a);
                col.rgb   += _StarColor.rgb * stars;

                return col;
            }
            ENDHLSL
        }
    }
}
