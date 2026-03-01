#ifndef STFUNCTIONS_URP_INCLUDED
#define STFUNCTIONS_URP_INCLUDED

// Reemplazo de clamp01 por saturate (más eficiente en GPU)
float clamp01(float value)
{
    return saturate(value);
}

float rev(float value)
{
    return 1.0 - value;
}

float rev01(float value)
{
    return saturate(1.0 - value);
}

float pos(float value)
{
    return value > 0 ? 1.0 : 0.0;
}

float posz(float value)
{
    return value >= 0 ? 1.0 : 0.0;
}

float neg(float value)
{
    return value < 0 ? 1.0 : 0.0;
}

float negz(float value)
{
    return value <= 0 ? 1.0 : 0.0;
}

float lerp01(float from, float to, float value)
{
    return saturate(lerp(from, to, value));
}

float invLerp(float from, float to, float value, float equal = 0.0)
{
    if (from == to)
        return equal;
    return (value - from) / (to - from);
}

float invLerp01(float from, float to, float value, float equal = 0.0)
{
    if (from == to)
        return equal;
    return saturate((value - from) / (to - from));
}

float wght_invLerp(float from, float to, float value, bool invert = false)
{
    float val = (value - from) / (to - from);

    float wgtMin = !invert ? 0.0 : 1.0;
    float wgtMax = !invert ? 1.0 : 0.0;
    float wgt = value < from ? wgtMin : wgtMax;

    float res = (value == from) ? 0.5 : wgt;
    return (from == to) ? res : val;
}

// Renombrado para evitar conflicto con la función intrínseca de HLSL en URP
float custom_smoothstep(float from, float to, float value, float equal)
{
    if (from == to)
        return equal;
    return smoothstep(from, to, value);
}

float wght_smoothstep(float from, float to, float value, bool invert = false)
{
    float val = smoothstep(from, to, value);

    float wgtMin = !invert ? 0.0 : 1.0;
    float wgtMax = !invert ? 1.0 : 0.0;
    float wgt = value < from ? wgtMin : wgtMax;

    float res = (value == from) ? 0.5 : wgt;
    return (from == to) ? res : val;
}

float smoothlerp(float from, float to, float value)
{
    float val = -(2.0 / ((value + 0.34) * 4.7)) + 1.3;
    return saturate(lerp(from, to, val));
}

float colmagnmin(float3 color)
{
    return min(color.r, min(color.g, color.b));
}

float colmagnmax(float3 color)
{
    return max(color.r, max(color.g, color.b));
}

float colspacemax(float3 color)
{
    return 1.0 - colmagnmin(color);
}

float colspacemin(float3 color)
{
    return 1.0 - colmagnmax(color);
}

float4 ColorBlend(float4 tcol, float4 dcol, float blendf)
{
    return lerp(tcol, dcol, saturate(blendf));
}

#endif