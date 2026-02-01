#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

// Safeguard the include so the preview window doesn't crash on light arrays
#if defined(SHADERGRAPH_PREVIEW)
    // No include needed for preview
#else
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Attenuation, out float Shadow)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(1.0f, 1.0f, 0.0f));
    Color = 1.0f;
    Attenuation = 1.0f;
    Shadow = 1.0f;
#else
    // To get actual shadows, we must calculate the shadow coordinates first
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
    
    Direction = mainLight.direction;
    Color = mainLight.color;
    Attenuation = mainLight.distanceAttenuation;
    Shadow = mainLight.shadowAttenuation;
#endif
}

void MainLight_half(half3 WorldPos, out half3 Direction, out half3 Color, out half Attenuation, out half Shadow)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(half3(1.0f, 1.0f, 0.0f));
    Color = 1.0f;
    Attenuation = 1.0f;
    Shadow = 1.0f;
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
    
    Direction = mainLight.direction;
    Color = mainLight.color;
    Attenuation = mainLight.distanceAttenuation;
    Shadow = mainLight.shadowAttenuation;
#endif
}

void AdditionalLight_float(float3 WorldPos, int lightID, out float3 Direction, out float3 Color, out float Attenuation)
{
    Direction = normalize(float3(1.0f, 1.0f, 0.0f));
    Color = 0.0f;
    Attenuation = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();
    if(lightID < lightCount)
    {
        Light light = GetAdditionalLight(lightID, WorldPos);
        Direction = light.direction;
        Color = light.color;
        Attenuation = light.distanceAttenuation;
    }
#endif
}

void AdditionalLight_half(half3 WorldPos, int lightID, out half3 Direction, out half3 Color, out half Attenuation)
{
    Direction = normalize(half3(1.0f, 1.0f, 0.0f));
    Color = 0.0f;
    Attenuation = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();
    if(lightID < lightCount)
    {
        Light light = GetAdditionalLight(lightID, WorldPos);
        Direction = light.direction;
        Color = light.color;
        Attenuation = light.distanceAttenuation;
    }
#endif
}

void AllAdditionalLights_float(float3 WorldPos, float3 WorldNormal, float2 CutoffThresholds, out float3 LightColor)
{
    LightColor = 0.0f;

#ifndef SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();

    for(int i = 0; i < lightCount; ++i)
    {
        // Get light data including shadow information
        Light light = GetAdditionalLight(i, WorldPos);

        // Calculate diffuse lighting
        float3 diffuse = dot(light.direction, WorldNormal);
        diffuse = smoothstep(CutoffThresholds.x, CutoffThresholds.y, diffuse);

        // Combine with color and distance attenuation (falloff)
        float3 color = diffuse * light.color * light.distanceAttenuation;

        // Multiply by shadow attenuation (0 in shadow, 1 in light)
        // This handles point/spot light shadows specifically
        color *= light.shadowAttenuation;

        LightColor += color;
    } 
#endif
}

void AllAdditionalLights_half(half3 WorldPos, half3 WorldNormal, half2 CutoffThresholds, out half3 LightColor)
{
    // Simply call the float version for consistency
    float3 outColor;
    AllAdditionalLights_float((float3)WorldPos, (float3)WorldNormal, (float2)CutoffThresholds, outColor);
    LightColor = (half3)outColor;
}

#endif // ADDITIONAL_LIGHT_INCLUDED