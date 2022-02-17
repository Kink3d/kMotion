﻿Shader "Hidden/kMotion/CameraMotionVectors"
{
    SubShader
    {
        Pass
        {
            Cull Off 
            ZWrite Off 
            ZTest Always

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
			#pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // -------------------------------------
            // Inputs
            TEXTURE2D(_CameraDepthTexture);       SAMPLER(sampler_CameraDepthTexture);

            // -------------------------------------
            // Structs
            struct Attributes
            {
                uint vertexID   : SV_VertexID;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
            };

            // -------------------------------------
            // Vertex
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = GetFullScreenTriangleVertexPosition(input.vertexID);
                return output;
            }

            // -------------------------------------
            // Fragment
            half4 frag(Varyings input) : SV_Target
            {
                // Calculate PositionInputs
                half depth = LOAD_TEXTURE2D(_CameraDepthTexture, input.position.xy).x;
                half2 screenSize = half2(1/_ScreenParams.x, 1/_ScreenParams.y);
                PositionInputs positionInputs = GetPositionInput(input.position.xy, screenSize, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

                // Calculate positions
                float4 previousPositionVP = mul(_PrevViewProjMatrix, float4(positionInputs.positionWS, 1.0));
                float4 positionVP = mul(UNITY_MATRIX_VP, float4(positionInputs.positionWS, 1.0));
                previousPositionVP.xy = previousPositionVP.xy / previousPositionVP.w;
                positionVP.xy = positionVP.xy / positionVP.w;

                // Calculate velocity
                float2 velocity = (positionVP.xy - previousPositionVP.xy);
                #if UNITY_UV_STARTS_AT_TOP
                    velocity.y = -velocity.y;
                #endif

                // Convert velocity from Clip space (-1..1) to NDC 0..1 space
                // Note it doesn't mean we don't have negative value, we store negative or positive offset in NDC space.
                return half4(velocity.xy * 0.5 + 0.5, 0, 0);
            }

            ENDHLSL
        }
    }
}
