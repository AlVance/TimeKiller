#ifndef STCORE_URP_INCLUDED
#define STCORE_URP_INCLUDED

// Incluimos las funciones matemáticas básicas primero
#include "STFunctions.hlsl"

// Incluimos las librerías de URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// --- CBUFFER para SRP Batcher ---
CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
float4 _Color;
float4 _DarkColor;
float4 _ShnColor;
float4 _OtlColor;
float _AmbientCol;
float _ColIntense;
float _ColBright;
float _Steps;
float _StpSmooth;
float _Offset;
float _MinLight;
float _MaxLight;
float _Lumin;
float _MaxAtten;
float _ShnIntense;
float _ShnRange;
float _ShnSmooth;
float _Segmented;
float _Clipped;
float _ShnOverlap;
float _OtlWidth;
CBUFFER_END

// --- Función Toon Principal ---
float Toon(float ndotl, float atten)
{
    float offset = clamp(_Offset, -1.0, 1.0);
    float delta = max(0.001, _MaxLight - _MinLight);

    // intense
    float ints_pls = ndotl + offset;
    float ints_max = 1.0 + offset;
    float intense = saturate(ints_pls / ints_max);

    // lit (escalones)
    float steps = _Segmented > 0.5 ? floor(_Steps) : 1.0;
    steps = max(1.0, steps);
    float step_val = 1.0 / steps;
    float lit_num = ceil(intense / step_val);
    float lit = lit_num * step_val;

    // smooth (suavizado)
    float reduce_v = _Offset - 1.0;
    float reduce_res = 1.0 - saturate(reduce_v / 0.1);
    float reduce = (lit_num == 1.0) ? reduce_res : 1.0;

    float smth_start = lit - step_val;
    float smth_end = smth_start + step_val * _StpSmooth;

    float smth_lrp = invLerp01(smth_end, smth_start, intense);
    float smth_stp = custom_smoothstep(smth_end, smth_start, intense, 0.0);

    float smooth_v = lerp(smth_lrp, smth_stp, _StpSmooth);
    float smooth_res = saturate(lit - smooth_v * reduce * step_val);

    // --- SOMBRAS (Recepción de sombras) ---
    // atten viene de mainLight.shadowAttenuation en el frag
    float atten_clmp = clamp(atten, 1.0 - _MaxAtten, 1.0);
    float dimLit = smooth_res * atten_clmp;
    
    // Luminosidad y Recorte
    float lumLight = _MaxLight + _Lumin;
    float lum_dlt = lumLight - _MinLight;
    
    float clip_cf = saturate(dimLit - _MinLight) / delta;
    float clip_v = clamp(_MinLight + clip_cf * lum_dlt, _MinLight, lumLight);

    float lerp_v = lum_dlt * dimLit;
    float relate_v = _MinLight + lerp_v;

    return (_Clipped > 0.5) ? clip_v : relate_v;
}

// --- Post Effects (Shine) ---
void PostShine(inout float4 col, float ndotl, float atten)
{
    float pos_v = abs(ndotl - 1.0);
    float len = _ShnRange * 2.0;

    float smth_inv = 1.0 - _ShnSmooth;
    float smth_end = len * smth_inv;

    float shine = posz(len - pos_v);
    float smooth_val = custom_smoothstep(len, smth_end, pos_v, 1.0);
    
    // La atenuación afecta al brillo también
    float dim = 1.0 - _MaxAtten * rev(atten) * rev(_ShnOverlap);

    float blend_factor = _ShnIntense * shine * smooth_val * dim;
    col = ColorBlend(col, _ShnColor, blend_factor);
}

float4 PostEffects(float4 col, float toon, float atten, float NdotL, float NdotH, float VdotN, float FdotV)
{
    PostShine(col, NdotL, atten);
    return col;
}

#endif