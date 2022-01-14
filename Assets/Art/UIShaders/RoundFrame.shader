Shader "UI/Generated/RoundFrame"
{
    Properties
    {
        Vector1_84cea09b1175404dbaac0930d53f5724("BorderLength", Float) = 0.6
        _Color("BorderColor", Color) = (1, 1, 1, 1)
        Vector2_320170b7fa4341eca4207ff5a62159c3("Offset", Vector) = (0, 0, 0, 0)
        Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7("Rotation", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Pass
        {
            Name "Default"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
            ZWrite Off
            ZTest [unity_GUIZTestMode]     
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_28c1e552c95e4966a39baecc148ab3ff_Out_0 = _Color;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.BaseColor = (_Property_28c1e552c95e4966a39baecc148ab3ff_Out_0.xyz);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_28c1e552c95e4966a39baecc148ab3ff_Out_0 = _Color;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.BaseColor = (_Property_28c1e552c95e4966a39baecc148ab3ff_Out_0.xyz);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float Vector1_84cea09b1175404dbaac0930d53f5724;
        float4 _Color;
        float2 Vector2_320170b7fa4341eca4207ff5a62159c3;
        float Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }

        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Floor_float(float In, out float Out)
        {
            Out = floor(In);
        }

        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0 = _Color;
            float _Split_9da95c00fb8746faaa9df1048e80b84d_R_1 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[0];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_G_2 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[1];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_B_3 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[2];
            float _Split_9da95c00fb8746faaa9df1048e80b84d_A_4 = _Property_71296ae6f8c84c05b6029046aacc6ef4_Out_0[3];
            float4 _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0 = IN.uv0;
            float _Split_d48fd73966fd4db79c7bf548a553feff_R_1 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[0];
            float _Split_d48fd73966fd4db79c7bf548a553feff_G_2 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[1];
            float _Split_d48fd73966fd4db79c7bf548a553feff_B_3 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[2];
            float _Split_d48fd73966fd4db79c7bf548a553feff_A_4 = _UV_f999dc8571af4d3f8ba5093eeefdfca8_Out_0[3];
            float2 _Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0 = float2(_Split_d48fd73966fd4db79c7bf548a553feff_R_1, _Split_d48fd73966fd4db79c7bf548a553feff_G_2);
            float2 _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0 = Vector2_320170b7fa4341eca4207ff5a62159c3;
            float2 _Add_08b8f666919848d0a3cc97dd24556038_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_08b8f666919848d0a3cc97dd24556038_Out_2);
            float2 _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2;
            Unity_Add_float2(_Add_08b8f666919848d0a3cc97dd24556038_Out_2, float2(0, 1), _Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2);
            float _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3;
            Unity_Rotate_Degrees_float(_Add_6818045ef7ca48d0aff8f90023d4d2d7_Out_2, float2 (0.5, 0.5), _Property_e8a85dc55c514f7f9f1af912babf67e6_Out_0, _Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3);
            float _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1;
            Unity_Length_float2(_Rotate_960b372e2cdd43f987e2cc74e9fc7abe_Out_3, _Length_64cf594a7c09451ab3533c0dbf83073f_Out_1);
            float _Property_914c6f609ea54e84a73a70d1eff88610_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2;
            Unity_Subtract_float(1, _Property_914c6f609ea54e84a73a70d1eff88610_Out_0, _Subtract_d8cae495c46d4498bba2e358127f4848_Out_2);
            float _Divide_e10ed775af5245119c42b6d26e39d928_Out_2;
            Unity_Divide_float(_Subtract_d8cae495c46d4498bba2e358127f4848_Out_2, 2, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2);
            float _Divide_248158d11ea34e1cad1b17984467c777_Out_2;
            Unity_Divide_float(1, _Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _Divide_248158d11ea34e1cad1b17984467c777_Out_2);
            float _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_248158d11ea34e1cad1b17984467c777_Out_2, _Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2);
            float _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1;
            Unity_Floor_float(_Multiply_a400bc5e1959471684fe79863ae26d6a_Out_2, _Floor_2e280a16e10744fdadec1e9f884dc565_Out_1);
            float _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3;
            Unity_Clamp_float(_Floor_2e280a16e10744fdadec1e9f884dc565_Out_1, 0, 1, _Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3);
            float _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1;
            Unity_OneMinus_float(_Divide_e10ed775af5245119c42b6d26e39d928_Out_2, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1);
            float _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2;
            Unity_Divide_float(1, _OneMinus_99f7b52134e749b29e5d8e7de5d10d53_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2);
            float _Multiply_c3317062e5134e6386cc2831b313127f_Out_2;
            Unity_Multiply_float(_Length_64cf594a7c09451ab3533c0dbf83073f_Out_1, _Divide_c17af1126b234abba0949d4f07e65fcc_Out_2, _Multiply_c3317062e5134e6386cc2831b313127f_Out_2);
            float _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1;
            Unity_Floor_float(_Multiply_c3317062e5134e6386cc2831b313127f_Out_2, _Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1);
            float _Clamp_57b30334d35544daa64806bae63c603d_Out_3;
            Unity_Clamp_float(_Floor_821b6ba27b78467aa5caadf49c96cc40_Out_1, 0, 1, _Clamp_57b30334d35544daa64806bae63c603d_Out_3);
            float _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2;
            Unity_Subtract_float(_Clamp_0bcea53a115848b98bcc9e6569bf24c4_Out_3, _Clamp_57b30334d35544daa64806bae63c603d_Out_3, _Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2);
            float2 _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2;
            Unity_Add_float2(_Vector2_d0ab6cec28c745dc9042d076b7bc974c_Out_0, _Property_f2fb01f6c7e74aa59c12e769027a26df_Out_0, _Add_1602317b8d8f45b38b991e5c33e1313e_Out_2);
            float2 _Add_6fd457188b554452916504631dbdd171_Out_2;
            Unity_Add_float2(_Add_1602317b8d8f45b38b991e5c33e1313e_Out_2, float2(0, 0), _Add_6fd457188b554452916504631dbdd171_Out_2);
            float _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0 = Vector1_24e97fc9b5354d6bbfbf374a14e4c9e7;
            float2 _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3;
            Unity_Rotate_Degrees_float(_Add_6fd457188b554452916504631dbdd171_Out_2, float2 (0.5, 0.5), _Property_6ee6619b441e435d8f842c2fff8fedd6_Out_0, _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3);
            float _Split_949ee835299a4379948d2bfe3cc79815_R_1 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[0];
            float _Split_949ee835299a4379948d2bfe3cc79815_G_2 = _Rotate_58ea1e76bd814380ade9988544b035d6_Out_3[1];
            float _Split_949ee835299a4379948d2bfe3cc79815_B_3 = 0;
            float _Split_949ee835299a4379948d2bfe3cc79815_A_4 = 0;
            float _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2;
            Unity_Subtract_float(_Split_949ee835299a4379948d2bfe3cc79815_G_2, 0.5, _Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2);
            float _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1;
            Unity_Absolute_float(_Subtract_534f022e784e4cafa5f5dd8415c9ff39_Out_2, _Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1);
            float _Property_21cb5cedbf744c4c84560ec7ea904668_Out_0 = Vector1_84cea09b1175404dbaac0930d53f5724;
            float _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2;
            Unity_Divide_float(_Property_21cb5cedbf744c4c84560ec7ea904668_Out_0, 2, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2);
            float _Divide_d3fd88b687b54e5387065727dea084d2_Out_2;
            Unity_Divide_float(1, _Divide_7007c2484ecc40ce83ed6c8e7f5642cd_Out_2, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2);
            float _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2;
            Unity_Multiply_float(_Absolute_0db08cd2084d41b1997a4810a0f94fba_Out_1, _Divide_d3fd88b687b54e5387065727dea084d2_Out_2, _Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2);
            float _Floor_065b0dcc955543f1a8b251d122b29005_Out_1;
            Unity_Floor_float(_Multiply_232ab8b0b90e4d49b279d899f4db304a_Out_2, _Floor_065b0dcc955543f1a8b251d122b29005_Out_1);
            float _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3;
            Unity_Clamp_float(_Floor_065b0dcc955543f1a8b251d122b29005_Out_1, 0, 1, _Clamp_119d136424404bfd9d224ad145ba50d2_Out_3);
            float _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1;
            Unity_OneMinus_float(_Clamp_119d136424404bfd9d224ad145ba50d2_Out_3, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1);
            float _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2;
            Unity_Add_float(_Subtract_32e1b1321d4e4f77b1f717943a88953c_Out_2, _OneMinus_6716db38b14140b8b9dafd91cc0b853c_Out_1, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2);
            float _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            Unity_Multiply_float(_Split_9da95c00fb8746faaa9df1048e80b84d_A_4, _Add_12571f1f935243b9bbc0ba009067e7f5_Out_2, _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2);
            surface.Alpha = _Multiply_6d2976eeae774a3aa71c35a7822fe985_Out_2;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}