// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PrimitiveClouds"
{
    Properties
    {
        _Color ("Debug Color", Color) = (1.0,1.0,1.0,1.0)
        _NoiseTexture("Cloud Noise Texture", 3D) = "white" {}
        _WeatherMap ("Weather", 2D) = "white" {}
        _Alpha ("Alpha", float) = 0.02
        _StepSize ("Step Size", float) = 0.01
        _Radius ("Sphere Radius", float) = 0.5
        _Center ("Sphere Center", float) = (0.0,0.0,0.0)
        _BoundaryNoiseMap ("Boundary Noise Map", 2D) = "white" {}
        _NoiseMultiplier ("Noise Multiplier", float) = 1.0
        _DensityOffset ("Cloud Density Offset", float) = 1.0

        _CloudBaseColor ("Cloud Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _CloudPeakColor ("Cloud Peak Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _CloudLightAbsorptionFactor("Cloud Light Absorption Factor", float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Maximum amount of raymarching samples
            #define MAX_STEP_COUNT 128

            // Allowed floating point inaccuracy
            #define EPSILON 0.00001f

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // Clip space
                float3 wPos : TEXCOORD1; // World position
            };

            Texture3D<float> _NoiseTexture;
            Texture2D<float> _WeatherMap;

            SamplerState sampler_NoiseTexture;
            SamplerState sampler_WeatherMap;

            sampler2D _BoundaryNoiseMap;
            sampler3D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;
            float _Alpha;
            float4 _LightColor0;
            float _StepSize;
            float3 _Center = float3(0.0,0.0,0.0);
            float _Radius = 0.5;
            float4 _Color;
            float _NoiseMultiplier;
            float _DensityOffset;

            float _CloudLightAbsorptionFactor;

            float4 _CloudPeakColor;
            float4 _CloudBaseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz; 
                return o;
            }

            float sdf_sphere(float3 pos, float3 c, float r) 
            {
                return distance(pos, c) - r;
            }

            float sdf_torus(float3 p, float2 t)
            {
                float2 q = float2(length(p.xz)-t.x,p.y);
                return length(q)-t.y;
            }

            float getNoise(float3 pos) 
            {
                float noiseSamplePosition = pos;
                float4 samp = _NoiseTexture.SampleLevel(sampler_NoiseTexture, noiseSamplePosition, 0);

                float3 weatherUVLoc = mul(pos, unity_ObjectToWorld);
                float2 weatherUV = weatherUVLoc;
                weatherUV = weatherUV * 1.0;
                float weatherMap = _WeatherMap.SampleLevel(sampler_WeatherMap, weatherUV, 0);
                float heightGradient = (weatherMap);

                //TODO: Noise decomposition to give clouds better shapes
                
                float baseShapeDensity = (samp + _DensityOffset) * heightGradient;

                // dense clouds at heigher altitudes to unify shape
                if(heightGradient > 0.5)
                    baseShapeDensity += 10.0;
                return baseShapeDensity;  
            }


            fixed4 frag(v2f i) : SV_Target
            {

                float3 worldPos = i.wPos;
                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);

                float lightTransmittance = 1.0;
                float totalLight = 0.0;
                float3 ambientLight = float3(0.0, 0.0, 0.0);

                float4 color = float4(0, 0, 0, 0);
                float3 samplePosition = worldPos;

                float d = 0.0;
                float noiseMult = (tex2D( _BoundaryNoiseMap, samplePosition.xz) * _NoiseMultiplier);

                // Raymarch through object space
                for (int it = 0; it < MAX_STEP_COUNT; it++)
                {
                    float cloudDensity = getNoise(samplePosition);
                    lightTransmittance *= exp(-cloudDensity * (it * _StepSize) * _CloudLightAbsorptionFactor);
                    totalLight += cloudDensity; 
                    ambientLight = lerp(_CloudPeakColor, _CloudBaseColor, sqrt(abs(saturate(cloudDensity))));
                    samplePosition += viewDir * _StepSize;
                }
                float3 bgCol = _Color;
                float3 cloudCol = totalLight * _LightColor0 * ambientLight;
                float3 col = bgCol * lightTransmittance + cloudCol;
                return float4(col,0);
            }
            ENDCG
        }
    }
}