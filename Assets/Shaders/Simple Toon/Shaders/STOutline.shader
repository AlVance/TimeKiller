Shader "Universal Render Pipeline/Simple Toon/SToon Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Colorize)][Space(5)]
        _Color ("Color", COLOR) = (1,1,1,1)
        _DarkColor ("Dark Color", COLOR) = (0,0,0,1)
        _AmbientCol ("Ambient", Range(0,1)) = 0
        _ColIntense ("Intensity", Range(0,3)) = 1
        _ColBright ("Brightness", Range(-1,1)) = 0

        [Header(Detail)][Space(5)]
        [Toggle] _Segmented ("Segmented", Float) = 1
        _Steps ("Steps", Range(1,25)) = 3
        _StpSmooth ("Smoothness", Range(0,1)) = 0
        _Offset ("Lit Offset", Range(-1,1.1)) = 0

        [Header(Light)][Space(5)]
        [Toggle] _Clipped ("Clipped", Float) = 0
        _MinLight ("Min Light", Range(0,1)) = 0
        _MaxLight ("Max Light", Range(0,1)) = 1
        _Lumin ("Luminosity", Range(0,2)) = 0
        _MaxAtten ("Max Attenuation", Range(0,1)) = 1

        [Header(Outline)][Space(5)]
        _OtlColor ("Outline Color", COLOR) = (0,0,0,1)
        _OtlWidth ("Outline Width", Range(0,5)) = 1

        [Header(Shine)][Space(5)]
        [HDR] _ShnColor ("Shine Color", COLOR) = (1,1,0,1)
        [Toggle] _ShnOverlap ("Overlap Shadows", Float) = 0
        _ShnIntense ("Shine Intensity", Range(0,1)) = 0
        _ShnRange ("Shine Range", Range(0,1)) = 0.15
        _ShnSmooth ("Shine Smoothness", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        // --- PASS 1: FORWARD LIT (Lógica principal de Toon) ---
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Requerido para sombras y luces adicionales en URP
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // Asegúrate de que STCore.hlsl esté en la misma carpeta
            #include "STCore.hlsl" 

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            sampler2D _MainTex;

            Varyings vert (Attributes v)
            {
                Varyings o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
                
                o.positionCS = vertexInput.positionCS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                // --- Cálculo de coordenadas de sombra ---
                o.shadowCoord = TransformWorldToShadowCoord(vertexInput.positionWS);
                
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // Preparar vectores básicos
                float3 normal = normalize(i.normalWS);
                
                // --- Obtener Luz Principal (Directional Light) ---
                Light mainLight = GetMainLight(i.shadowCoord);
                float3 lightDir = normalize(mainLight.direction);
                
                // Atenuación de sombra (proyectada por otros) + atenuación de distancia
                float atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

                // Cálculos de Iluminación Toon
                float NdotL = dot(normal, lightDir);
                float toon = Toon(NdotL, atten);

                // Mezcla de Colores
                float4 lightColor = float4(mainLight.color, 1.0);
                float4 litcol = ColorBlend(_Color, lightColor, _AmbientCol);
                float4 texcol = tex2D(_MainTex, i.uv) * litcol * _ColIntense + _ColBright;

                // Aplicar el efecto toon
                float4 finalCol = ColorBlend(_DarkColor, texcol, toon);

                // Efectos de Post-procesado (Shine)
                finalCol = PostEffects(finalCol, toon, atten, NdotL, 0, 0, 0);

                return float4(finalCol.rgb, 1.0);
            }
            ENDHLSL
        }

        // --- PASS 2: OUTLINE (Inverted Hull) ---
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front // Dibuja las caras internas expandidas

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Usamos el mismo CBUFFER definido en STCore.hlsl
            #include "STCore.hlsl" 

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                
                // Extrusión de vértices en espacio de mundo para un outline constante
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                
                // El valor 0.01 ajusta la escala del outline
                positionWS += normalWS * _OtlWidth * 0.01; 
                
                o.positionCS = TransformWorldToHClip(positionWS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Si el ancho es 0, no dibujamos nada
                if(_OtlWidth <= 0) discard;
                return _OtlColor;
            }
            ENDHLSL
        }

        // --- PASS 3: SHADOW CASTER ---
        // Necesario para que el objeto proyecte sombras sobre otros
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
}