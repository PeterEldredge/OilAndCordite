Shader "Unlit/UnlitLava"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Grit Noise", 2D) = "white" {}
        _CooledNoise ("Cooled Grit Noise", 2D) = "white" {}
        _NoiseSpeed ("Noise Speed", float) = 1.0
        _EdgeCutoff ("Edge Cutoff", float) = 1.0
        _EdgeDistance ("Edge Distance", float) = 1.0
        _CutoffMultiplier ("Cutoff Multiplier", float) = 1.0
        _VertexMovementSpeed ("Vertex Movement Speed", float) = 1.0
        _VertexAmplitude ("Vertex Amplitude", float) = 1.0
        _VertexWavelength ("Vertex Wavelength", float) = 1.0

        _LavaEdgeColor ("Lava Edge Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaInnerColor ("Lava Inner Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaCooledColor ("Lava Cooled Color", Color) = (1.0,1.0,1.0,1.0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _NoiseTex;
            sampler2D _CooledNoise;
            float4 _MainTex_ST;

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float _DepthMaxDistance;
            float _CutoffMultiplier;
            float _VertexMovementSpeed;
            float _VertexAmplitude;
            float _VertexWavelength;

            float4 _LavaInnerColor;
            float4 _LavaEdgeColor;
            float4 _LavaCooledColor;
            float4 _LavaVeryHotColor;
            float _EdgeCutoff;
            float _EdgeDistance;
            
            float _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float k = 2 * UNITY_PI / _VertexWavelength;
                float f = k * (worldPos.x - _VertexMovementSpeed * _Time.y);
                o.vertex.x += _VertexAmplitude * cos(f);
                o.vertex.y += _VertexAmplitude * sin(f);

                o.screenPosition = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 random2(float2 st)
            {
                st = float2(dot(st, float2(127.1,311.7)), dot(st, float2(269.5,183.3)));
                return -1.0 + 2.0*frac(sin(st)*43758.5453123);
            }

            // Gradient Noise by Inigo Quilez - iq/2013
            // https://www.shadertoy.com/view/XdXGW8
            float noise(float2 st) 
            {
                float2 i = floor(st);
                float2 f = frac(st);

                float2 u = f*f*(3.0-2.0*f);

                return lerp( lerp( dot( random2(i + float2(0.0,0.0) ), f - float2(0.0,0.0) ),
                        dot( random2(i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                        lerp( dot( random2(i + float2(0.0,1.0) ), f - float2(0.0,1.0) ),
                        dot( random2(i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float gritNoise = tex2D(_NoiseTex, i.uv);
                float cooledNoise = tex2D(_CooledNoise, i.uv);
                float2 st = i.uv.xy;
                float t = abs(1.0-sin(_Time.x * _NoiseSpeed)) * _NoiseSpeed * 5;

                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);
                float depthDifference = existingDepthLinear - i.screenPosition.w;
                depthDifference = 1 - depthDifference;

                float foamDepthDifference01 = saturate(depthDifference / _EdgeDistance);
                float surfaceNoiseCutoff = foamDepthDifference01 * _EdgeCutoff;
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                float3 col = _LavaInnerColor;

                st += noise(st * 2.0) * t;
                col = float3(1.0, 1.0, 1.0) * smoothstep(0.1, 1.0, noise(st)); // Big black drops
                //col += smoothstep(.15,.2,noise(st*10.0)); // Black splatter
                col -= smoothstep(.35,.4,noise(st * 10.0 * gritNoise)); // Holes on splatter
                
                col += (surfaceNoiseCutoff);

                float4 surfaceNoise = col.r > 0 ? lerp(_LavaInnerColor, _LavaEdgeColor, gritNoise) : _LavaCooledColor * cooledNoise;

                return float4(col, 0);
            }
            ENDCG
        }
    }
}
