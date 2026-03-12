Shader "Custom/URP/WorldSpaceWobble"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Wobble
        _WobbleAmplitude ("Wobble Amplitude", Range(0.0, 0.05)) = 0.008
        _WobbleFrequency ("Wobble Frequency", Range(0.1, 20.0)) = 4.0
        _WobbleSpeed    ("Wobble Speed",     Range(0.0, 10.0)) = 1.5

        // Second wave layer (adds organic feel)
        _WobbleAmplitude2 ("Wobble Amplitude 2", Range(0.0, 0.05)) = 0.004
        _WobbleFrequency2 ("Wobble Frequency 2", Range(0.1, 20.0)) = 7.3
        _WobbleSpeed2     ("Wobble Speed 2",     Range(0.0, 10.0)) = 2.8

        // Vertical mask — wobble is stronger toward the top of each tile
        _WobbleMaskStrength ("Vertical Mask Strength", Range(0.0, 1.0)) = 0.6

        // Required for URP 2D sprite rendering
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "RenderType"        = "Transparent"
            "RenderPipeline"    = "UniversalPipeline"
            "IgnoreProjector"   = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "WorldSpaceWobble"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ---------------------------------------------------------------------------
            // Structs
            // ---------------------------------------------------------------------------
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float2 worldPos     : TEXCOORD1;   // XY world position for wobble
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ---------------------------------------------------------------------------
            // Uniforms
            // ---------------------------------------------------------------------------
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_AlphaTex);
            SAMPLER(sampler_AlphaTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4  _Color;
                half4  _RendererColor;

                float  _WobbleAmplitude;
                float  _WobbleFrequency;
                float  _WobbleSpeed;

                float  _WobbleAmplitude2;
                float  _WobbleFrequency2;
                float  _WobbleSpeed2;

                float  _WobbleMaskStrength;
                float  _EnableExternalAlpha;
            CBUFFER_END

            // ---------------------------------------------------------------------------
            // Vertex shader — pass world position through, no displacement here
            // (flat quads don't have enough verts; displacement is done in frag via UV)
            // ---------------------------------------------------------------------------
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color * _Color * _RendererColor;

                // World XY for wobble math
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldPos    = worldPos.xy;

                return OUT;
            }

            // ---------------------------------------------------------------------------
            // Fragment shader — world-space UV distortion
            // ---------------------------------------------------------------------------
            half4 frag(Varyings IN) : SV_Target
            {
                float2 wp = IN.worldPos;
                float  t  = _Time.y;

                // --- Vertical mask -------------------------------------------------------
                // IN.uv.y == 0 at the bottom of a tile, 1 at the top.
                // Lerp between 1 (full wobble at top) and (1 - mask) (reduced at bottom).
                float mask = lerp(1.0 - _WobbleMaskStrength, 1.0, IN.uv.y);

                // --- Wave layer 1 --------------------------------------------------------
                float2 wave1 = float2(
                    sin(wp.y * _WobbleFrequency  + t * _WobbleSpeed)  * _WobbleAmplitude,
                    cos(wp.x * _WobbleFrequency  + t * _WobbleSpeed)  * _WobbleAmplitude
                );

                // --- Wave layer 2 (offset direction so they don't cancel) ----------------
                float2 wave2 = float2(
                    sin(wp.x * _WobbleFrequency2 + t * _WobbleSpeed2 + 1.7) * _WobbleAmplitude2,
                    cos(wp.y * _WobbleFrequency2 + t * _WobbleSpeed2 + 0.9) * _WobbleAmplitude2
                );

                // --- Apply mask and accumulate -------------------------------------------
                float2 uvOffset = (wave1 + wave2) * mask;
                float2 distortedUV = IN.uv + uvOffset;

                // --- Sample --------------------------------------------------------------
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);

                #ifdef ETC1_EXTERNAL_ALPHA
                    half4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, distortedUV);
                    texColor.a = lerp(texColor.a, alpha.r, _EnableExternalAlpha);
                #endif

                return texColor * IN.color;
            }

            ENDHLSL
        }

        // Shadow / depth pass — unlit tilemaps typically don't need this,
        // but included so the material doesn't break shadow casters.
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct DepthAttribs  { float4 pos : POSITION; };
            struct DepthVaryings { float4 pos : SV_POSITION; };

            DepthVaryings DepthVert(DepthAttribs IN)
            {
                DepthVaryings OUT;
                OUT.pos = TransformObjectToHClip(IN.pos.xyz);
                return OUT;
            }
            half4 DepthFrag(DepthVaryings IN) : SV_Target { return 0; }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/2D/Sprite-Unlit-Default"
}
