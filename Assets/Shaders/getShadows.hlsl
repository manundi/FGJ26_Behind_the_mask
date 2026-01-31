#ifndef CUSTOM_SHADOWS_INCLUDED
#define CUSTOM_SHADOWS_INCLUDED

// Internal URP check to prevent the "implicit array" error in Preview
#if defined(SHADERGRAPH_PREVIEW)
    // In the preview window, we just return 1.0 (no shadow)
#else
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void CalculateShadows_float(float3 WorldPos, out float ShadowAmount)
{
    ShadowAmount = 1.0;

// Only execute shadow logic if we are NOT in the preview window 
// AND the URP lighting library is actually loaded
#if !defined(SHADERGRAPH_PREVIEW)
    #if defined(UNIVERSAL_LIGHTING_HLSL)
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
            float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPos));
            shadowCoord = screenPos;
        #endif

        ShadowAmount = MainLightRealtimeShadow(shadowCoord);
    #endif
#endif
}

void CalculateShadows_half(half3 WorldPos, out half ShadowAmount)
{
    float outShadow;
    CalculateShadows_float((float3)WorldPos, outShadow);
    ShadowAmount = (half)outShadow;
}

#endif