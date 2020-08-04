// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CloudsRaymarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTexture ("NoiseGen", 3D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"
        }
        Pass
        {
            Tags {"Queue"="Transparent" "RenderType"="Transparent" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 lm : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 lm : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = mul(v.uv, unity_ObjectToWorld);
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewDir = mul(unity_CameraToWorld, float4(viewVector,0));
                o.lm = v.lm;
                return o;
            }

            Texture3D<float> _NoiseTexture;
            Texture3D<float> _DecompositionTexture;
            Texture2D<float> _WeatherMap;
            Texture2D<float> _OffsetNoise;

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            SamplerState sampler_NoiseTexture;
            SamplerState sampler_WeatherMap;
            SamplerState sampler_OffsetNoise;
            SamplerState sampler_DecompositionTexture;
            float3 _VolumeBoundaryMin;
            float3 _VolumeBoundaryMax;
            float _DarknessThreshold;
            float4 noiseWeight;
            float4 decompositionWeight;
            float detailNoiseWeight;
            float _InternalNoiseSpeed;
            float _WindSpeed;
            int _RaymarchSteps;
            int _lightMarchSteps;
            float4 _LightColor0;
            float _DensityOffset;
            float _CloudLightAbsorptionFactor;
            float _CloudScale;
            float _DensityMultiplier;

            float _AmbientSky;
            float _EquatorIntensity;
            float _SkyIntensity;

            float4 _CloudPeakColor;
            float4 _CloudBaseColor;

            //Src: https://medium.com/@bromanz/another-view-on-the-classic-ray-aabb-intersection-algorithm-for-bvh-traversal-41125138b525
             float2 rayBoxDest (float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDirection) 
             {
                float3 t0 = (boundsMin - rayOrigin) / rayDirection;
                float3 t1 = (boundsMax - rayOrigin) / rayDirection;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);
                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));
                float distanceToVolume = max(0, dstA);
                float distanceOfInnerVolume = max(0, dstB - distanceToVolume);
                return float2(distanceToVolume, distanceOfInnerVolume);
            }

            float getNoise (float3 pos) 
            {
                float3 volumeSize = _VolumeBoundaryMax - _VolumeBoundaryMin;
                float noiseSamplePosition = mul(pos, unity_ObjectToWorld) + float3(_Time.x  * _WindSpeed, _Time.x * _WindSpeed, _Time.x * _WindSpeed);

                float3 weatherUVLoc = mul(pos, unity_ObjectToWorld);
                float2 weatherUV = (volumeSize.xz + (weatherUVLoc.xz)) / max(volumeSize.x,volumeSize.z) + float3(_Time.x  * _WindSpeed, _Time.x * _WindSpeed, _Time.x * _WindSpeed);
                weatherUV = weatherUV * _CloudScale;
                float weatherMap = _WeatherMap.SampleLevel(sampler_WeatherMap, weatherUV, 0);
                float heightRatio = (_VolumeBoundaryMax.y - pos.y) / volumeSize.y;
                float heightGradient = (weatherMap * heightRatio);

                float4 samp = _NoiseTexture.SampleLevel(sampler_NoiseTexture, noiseSamplePosition , 0);
                //TODO: Noise decomposition to give clouds better shapes
                float4 normalizedShapeWeights = noiseWeight / dot(noiseWeight, 1);
                float shapeFBM = samp * heightGradient;

                float baseShapeDensity = (shapeFBM + _DensityOffset) * 0.1;

                if (baseShapeDensity > 0.01)
                {
                    float4 detailNoise = _DecompositionTexture.SampleLevel(sampler_DecompositionTexture, noiseSamplePosition, 0);
                    float4 normDecomWeight = decompositionWeight / dot(decompositionWeight, 1);
                    float detailFBM = dot(detailNoise, normDecomWeight);

                    float oneMinusShape = 1 - shapeFBM;
                    float detailErodeWeight = oneMinusShape * oneMinusShape * oneMinusShape;

                    float cloudDensity = baseShapeDensity - (1-detailFBM) * detailErodeWeight * detailNoiseWeight;
    
                    return cloudDensity * _DensityMultiplier * 0.08;
                }
                return 0;  
            }

            float marchLighting (float3 position) {
                float3 dirToLight =  -1 * _WorldSpaceLightPos0.xyz;
                float dstInsideBox = rayBoxDest(_VolumeBoundaryMin, _VolumeBoundaryMax, position, 1/dirToLight).y;
                
                float stepSize = dstInsideBox/ _lightMarchSteps;
                float totalDensity = 0;

                for (int step = 0; step < _lightMarchSteps; step ++) {
                    position += dirToLight * stepSize;
                    totalDensity += max(0, getNoise(position) * stepSize);
                }

                float transmittance = -exp(totalDensity * _CloudLightAbsorptionFactor);
                float lightTrans = _DarknessThreshold - transmittance * (1-_DarknessThreshold); 
                if (lightTrans > 0 )
                {
                    return lightTrans;
                }
                return 0.1f;
            }

            // simple volumetric raymarcher to get noise coordinates
            float4 getNoiseSimple (float3 pos) 
            {
                float4 samp = _NoiseTexture.SampleLevel(sampler_NoiseTexture, pos, 0);
                return samp;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayPos = _WorldSpaceCameraPos;
                float rayLength = length(i.viewDir);
                float3 rayDirection = i.viewDir / rayLength;

                float2 rayboxHit = rayBoxDest(_VolumeBoundaryMin, _VolumeBoundaryMax, rayPos, rayDirection);
                float distanceToVolume = rayboxHit.x;
                float distanceToVolumeInner = rayboxHit.y;

                float depthTextureSample = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float linearDepth= LinearEyeDepth(depthTextureSample) * rayLength;

                float step = 0.0;

                float3 volumeEntryPoint = rayPos + rayDirection * distanceToVolume;

                float totalLight = 0;
                float lightTransmittance = 1.0;

                // determine ray steps depending how far camera is away from render volume
                // TODO: Refactor into fade away 
                if (distanceToVolume > 100.0f) 
                {
                    step = distanceToVolumeInner / _RaymarchSteps;
                    _CloudLightAbsorptionFactor = _CloudLightAbsorptionFactor;
                }
                else
                {
                    step = distanceToVolumeInner / _RaymarchSteps;
                }

                float randomOffset = _OffsetNoise.SampleLevel(sampler_OffsetNoise, i.uv, 0);
                float distanceTraveled = randomOffset;
                float maxDistance = min(linearDepth- distanceToVolume, distanceToVolumeInner);
                float3 ambientLight = float3(0.0, 0.0, 0.0);
                float lightTrans = 0.0;

                while (distanceTraveled < maxDistance) 
                {
                    rayPos = volumeEntryPoint + rayDirection * distanceTraveled;

                    float cloudDensity = getNoise(rayPos);

                    lightTrans = marchLighting(rayPos);
                    totalLight += cloudDensity * step * lightTrans * lightTransmittance;
                    lightTransmittance *= exp(-cloudDensity * step * _CloudLightAbsorptionFactor);
                    distanceTraveled += step;

                    if (lightTransmittance < 0.1)
                    {
                        break;
                    }
                }

                float3 bgCol = tex2D(_MainTex,i.uv);
                float3 cloudCol = (0.0,0.0,0.0);

                if (totalLight > 0)
                {
                    cloudCol = totalLight * _LightColor0 * lerp((unity_AmbientSky * _AmbientSky * _SkyIntensity), (unity_AmbientEquator * _AmbientSky * _EquatorIntensity), totalLight);
                }

                float3 col = bgCol + cloudCol * lightTrans;
                return float4(col,0);
            }
            ENDCG
        }
    }
}