Shader "Particles/Blood Effect URP"
{
    Properties
    {
        [Header(Color Controls)]
        [HDR] _BaseColor("Base Color Mult", Color) = (1,1,1,1)
        _LightStr("Lighting Strength", float) = 0.85
        _AlphaMin("Alpha Clip Min", Range(-0.01, 1.01)) = 0.1
        _AlphaSoft("Alpha Clip Softness", Range(0,1)) = 0.022
        _EdgeDarken("Edge Darkening", float) = 1.0
        _ProcMask("Procedural Mask Strength", float) = 1.0

        [Header(Mask Controls)]
        _MainTex("Mask Texture", 2D) = "white" {}
        _MaskStr("Mask Strength", float) = 0.7
        _Columns("Flipbook Columns", Int) = 1
        _Rows("Flipbook Rows", Int) = 1
        _ChannelMask("Channel Mask", Vector) = (1,0,0,0)
        [Toggle] _FlipU("Flip U Randomly", float) = 0
        [Toggle] _FlipV("Flip V Randomly", float) = 0

        [Header(Noise Controls)]
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseAlphaStr("Noise Strength", float) = 0.8
        _ChannelMask2("Channel Mask", Vector) = (1,0,0,0)
        _Randomize("Randomize Noise", float) = 1.0

        [Header(Vertex Physics)]
        _FallOffset("Gravity Offset", range(-1,0)) = -1.0
        _FallRandomness("Gravity Randomness", float) = 0.25

        [HideInInspector] _WarpTex("Warp Texture", 2D) = "grey" {}
        [HideInInspector] _WarpStr("Warp Strength", float) = 1.0
        [HideInInspector] _NoiseColorStr("Noise Color Strength", float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 color        : COLOR;
                float4 texcoord0    : TEXCOORD0;
                float3 texcoord1    : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float4 vertLight    : TEXCOORD1;
                float3 customData   : TEXCOORD3;
                float fogCoord      : TEXCOORD4;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_WarpTex); SAMPLER(sampler_WarpTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _LightStr;
                float _AlphaMin;
                float _AlphaSoft;
                float _EdgeDarken;
                float _ProcMask;

                float4 _MainTex_ST;
                float _MaskStr;
                float _Columns;
                float _Rows;
                half4 _ChannelMask;
                float _FlipU;
                float _FlipV;

                float4 _NoiseTex_ST;
                float _NoiseAlphaStr;
                float _NoiseColorStr;
                half4 _ChannelMask2;
                float _Randomize;

                float4 _WarpTex_ST;
                float _WarpStr;

                float _FallOffset;
                float _FallRandomness;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;

                float lifetime = v.texcoord0.w;
                lifetime = lifetime * lifetime + (_FallOffset + ((v.texcoord0.z - 0.5) * _FallRandomness)) * lifetime;
                float3 fallPos = lifetime * float3(0, v.texcoord1.z, 0);

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz) + fallPos;
                o.positionHCS = TransformWorldToHClip(worldPos);

                float2 UVflip = round(frac(float2(v.texcoord0.z * 13, v.texcoord0.z * 8)));
                UVflip = UVflip * 2 - 1;
                UVflip = lerp(float2(1,1), UVflip, float2(_FlipU, _FlipV));

                o.color = v.color;
                o.color.a *= o.color.a;
                o.color.a += _AlphaMin;

                o.customData = float3(v.texcoord1.xy, v.texcoord0.z);

                o.uv.xy = v.texcoord0.xy * _MainTex_ST.xy * UVflip + _MainTex_ST.zw;
                o.uv.zw = o.uv.xy * float2(_Columns, _Rows) + v.texcoord0.z * float2(3,8) * _Randomize;

                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 ambient = SampleSH(normalWS);
                o.vertLight.xyz = lerp(float3(1,1,1), ambient, _LightStr);

                o.fogCoord = ComputeFogFactor(o.positionHCS.z);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 uvWarp = SAMPLE_TEXTURE2D(_WarpTex, sampler_WarpTex, i.uv.zw * _WarpTex_ST.xy + _WarpTex_ST.zw * (i.customData.x + 1) + (float2(5,8) * i.customData.z));
                float2 warp = (uvWarp.xy * 2) - 1;
                warp *= _WarpStr * i.customData.y;

                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy + warp);
                mask = saturate(lerp(half4(1,1,1,1), mask, _MaskStr));

                float2 tempUV = frac(i.uv.xy * float2(_Columns, _Rows)) - 0.5;
                tempUV *= tempUV * 4;
                float edgeMask = saturate(tempUV.x + tempUV.y);
                edgeMask = 1 - (edgeMask * edgeMask);
                edgeMask = lerp(1.0, edgeMask, _ProcMask);

                mask *= edgeMask;
                half4 col = max(0.001, i.color);
                col.a = saturate(dot(mask, _ChannelMask));

                half4 noise4 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv.zw * _NoiseTex_ST.xy + _NoiseTex_ST.zw * i.customData.x + warp);
                half noise = dot(noise4, _ChannelMask2);
                noise = saturate(lerp(1.0, noise, _NoiseAlphaStr));

                col.a *= noise;
                half preClipAlpha = col.a;
                half clippedAlpha = saturate((preClipAlpha * i.color.a - _AlphaMin) / max(_AlphaSoft, 0.0001));
                col.a = clippedAlpha;

                float3 baseLighting = max(float3(0.01, 0.01, 0.01), i.vertLight.xyz);

                half edge = 1 - saturate(preClipAlpha * clippedAlpha);
                edge = 1 - (edge * edge);
                edge = edge + lerp(0.0, noise - 0.5, _NoiseColorStr);
                edge = saturate(lerp(0.71, edge * edge, _EdgeDarken));

                col.a *= saturate(lerp(1.25, _BaseColor.a, edge));
                col.rgb *= lerp(min(col.rgb * col.rgb * col.rgb * 0.3, 1.0), 0.71, edge);

                col.rgb *= max(float3(0,0,0), baseLighting * _BaseColor.rgb);

                col.rgb = MixFog(col.rgb, i.fogCoord);

                return col;
            }
            ENDHLSL
        }
    }
}