// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PrimitiveClouds"
{
    Properties
    {
        _Color ("Debug Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex ("Texture", 3D) = "white" {}
        _WeatherMap ("Weather", 2D) = "white" {}
        _Alpha ("Alpha", float) = 0.02
        _StepSize ("Step Size", float) = 0.01
        _Radius ("Sphere Radius", float) = 0.5
        _Center ("Sphere Center", float) = (0.0,0.0,0.0)
        _IntersectionPosition ("Intersection Position (DO NOT EDIT)", float) = (0.0,0.0,0.0)
        _IntersectionOrientation ("Intersection Orientation (DO NOT EDIT)", float) = (0.0,0.0,0.0)
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

            sampler2D _WeatherMap;
            sampler3D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;
            float _Alpha;
            float _StepSize;
            float3 _Center = float3(0.0,0.0,0.0);
            float3 _IntersectionPosition;
            float3 _IntersectionOrientation;
            float _Radius = 0.5;
            float4 _Color;

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

            float fTorus(float3 p, float smallRadius, float largeRadius) 
            {
	            return length(float2(length(p.xz) - largeRadius, p.y)) - smallRadius;
            }

            float sdf_blend(float d1, float d2, float a)
            {
                return a * d1 + (1 - a) * d2;
            }

            float map(float3 pos) 
            {
                return distance(pos, _Center) - _Radius;
            }
	
            float vmax(float3 v)
            {
                return max(max(v.x, v.y), v.z);
            }
            
            float sdf_boxcheap(float3 p, float3 c, float3 s)
            {
                return vmax(abs(p-c) - s);
            }

            float opSub( float d1, float d2, float k = 32) 
            {
                return max(-d1,d2);
            }

            float opSmoothSub( float d1, float d2, float k ) 
            {
                float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
                return lerp( d2, -d1, h ) + k*h*(1.0-h); 
            }


            float sdCappedCylinder( float3 p, float h, float r )
            {
                float2 d = abs(float2(length(p.xz),p.y)) - float2(h,r);
                return min(max(d.x,d.y),0.0) + length(max(d,0.0));
            }

            float sdf_CappedCylinder(float3 p, float3 a, float3 b, float r)
            {
                float3  ba = b - a;
                float3  pa = p - a;
                float baba = dot(ba,ba);
                float paba = dot(pa,ba);
                float x = length(pa*baba-ba*paba) - r*baba;
                float y = abs(paba-baba*0.5)-baba*0.5;
                float x2 = x*x;
                float y2 = y*y*baba;
                float d = (max(x,y)<0.0)?-min(x2,y2):(((x>0.0)?x2:0.0)+((y>0.0)?y2:0.0));
                return sign(d)*sqrt(abs(d))/baba;
            }

            #include "Lighting.cginc"
            fixed4 simpleLambert (fixed3 normal) 
            {
                fixed3 lightDir = _WorldSpaceLightPos0.xyz; // Light direction
                fixed3 lightCol = _LightColor0.rgb; // Light color
                
                fixed NdotL = max(dot(normal, lightDir),0);
                fixed4 c;
                c.rgb = _Color * lightCol * NdotL;
                c.a = 1;
                return c;
            }
            float3 normal (float3 p)
            {
                const float eps = 0.01;
                return normalize(float3(map(p + float3(eps, 0, 0) ) - map(p - float3(eps, 0, 0)),
                                map(p + float3(0, eps, 0) ) - map(p - float3(0, eps, 0)),
                                map(p + float3(0, 0, eps) ) - map(p - float3(0, 0, eps))));
            }

            float opUnion( float d1, float d2 ) 
            {  
               return min(d1,d2); 
            }

            fixed4 renderSurface(float3 p)
            {
                float3 n = normal(p);
                return simpleLambert(n);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float depthTextureSample = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.pos);
                float linearDepth= LinearEyeDepth(depthTextureSample) * i.wPos;

                float3 worldPos = i.wPos;
                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);

                float4 color = float4(0, 0, 0, 0);
                float3 samplePosition = worldPos;

                float d = 0.0;
                float distanceTraveled = 0.0f;

                float2 ringThickness =  (5.00, 2.0);
                // Raymarch through object space
                for (int i = 0; i < MAX_STEP_COUNT; i++)
                {
                    float dis = distance(_Center, _IntersectionPosition);
                    float boundX = max(abs(samplePosition.x), max(abs(samplePosition.y), abs(samplePosition.z))) + EPSILON;
                    if(dis > -30 && dis < 30) {
                        //d = sdf_blend(sdf_sphere(samplePosition, _Center, _Radius), sdf_boxcheap(samplePosition, _Center, _Radius), _Radius);
                        //d = sdf_blend(sdf_sphere( samplePosition, _Center, _Radius), sdf_torus( samplePosition, ringThickness), dis / boundX);
                        float initialShape = sdf_sphere(samplePosition, _Center, _Radius);
                        //float cutoutShape = sdf_sphere(samplePosition, _IntersectionPosition, _Radius / 5);
                        float centerAngle = acos(dot(normalize(_IntersectionPosition), _Center));
                        float cutoutShape = sdf_CappedCylinder(samplePosition, _IntersectionPosition, _IntersectionOrientation, 6.0);
                        d = opSmoothSub(cutoutShape, initialShape, 0.5);
                    }
                    else {
                        d = sdf_sphere(samplePosition, _Center, _Radius);
                    }
                    if(d < 0.0) {
                        // calculate cloud color and density
                        return renderSurface(samplePosition);
                    }
                    if(distanceTraveled > linearDepth) {
                        return fixed4(0.0,0.0,0.0,0.0);
                    }
                    samplePosition += viewDir * _StepSize;
                    distanceTraveled += _StepSize;
                }
                 return fixed4(0.0,0.0,0.0,0.0);
            }
            ENDCG
        }
    }
}