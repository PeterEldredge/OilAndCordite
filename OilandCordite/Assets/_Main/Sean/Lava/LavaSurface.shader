Shader "Custom/LavaSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _NoiseTex ("Grit Noise", 2D) = "white" {}
        _CrackTexture ("Crack Texture", 2D) = "white" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseSpeed ("Noise Speed", float) = 1.0
        _LavaEdgeColor ("Lava Edge Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaInnerColor ("Lava Inner Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaCooledColor ("Lava Cooled Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaCrackColor ("Lava Cracks Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaVeryHotColor ("Lava Very Hot Color", Color) = (1.0,1.0,1.0,1.0)
        _LavaCrackCoolSpeed ("Lava Crack Cool Speed", float) = 10.0
        _LavaCoolSpeed ("Lava Cool Speed", float) = 10.0
        _LavaCrackCooledColor ("Lava Crack Cooled Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _LavaCrackScale ("Lava Crack Scale", float) = 1.0
        _LavaDepthFade ("Depth Fade", float) = 1.0

        _VertexMovementSpeed ("Vertex Movement Speed", float) = 1.0
        _VertexAmplitude ("Vertex Amplitude", float) = 1.0
        _VertexWavelength ("Vertex Wavelength", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _CrackTexture;
        sampler2D _CameraDepthTexture;
        float4 _CameraDepthTexture_TexelSize;

        float _VertexMovementSpeed;
        float _VertexAmplitude;
        float _VertexWavelength;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float eyeDepth;
        };

        void vert(inout appdata_full vertexData) {
			float3 p = vertexData.vertex.xyz;

			float k = 2 * UNITY_PI / _VertexWavelength;
			p.y = _VertexAmplitude * sin(k * (p.x - _VertexMovementSpeed * _Time.y));

			vertexData.vertex.xyz = p;
		}

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _NoiseSpeed;

        float _LavaCrackCoolSpeed;
        float _LavaCoolSpeed;

        float4 _LavaInnerColor;
        float4 _LavaEdgeColor;
        float4 _LavaVeryHotColor;
        float4 _LavaCooledColor;
        float4 _LavaCrackColor;
        float4 _LavaCrackCooledColor;
        float _LavaCrackScale;
        float _LavaDepthFade;

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

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 gritNoise = tex2D(_NoiseTex, IN.uv_MainTex);
            float2 st = IN.uv_MainTex.xy;
            float t = abs(1.0-sin(_Time.x * _NoiseSpeed)) * _NoiseSpeed;
            float3 rocks = _LavaCooledColor;
            float3 lava = _LavaInnerColor;

            float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
            float sceneZ = LinearEyeDepth(rawZ);
            float partZ = IN.eyeDepth;
 
            float fade = 1.0;
            if ( rawZ > 0.0 )
                fade = saturate(_LavaDepthFade * (sceneZ - partZ));

            st += noise(st * 2.0) * t;
            rocks = float3(1.0, 1.0, 1.0) * smoothstep(0.1, 1.0, noise(st * 0.8) * 0.5); // build base lava pits
            rocks += smoothstep(.15,.5,noise(st * 0.5)); // add additional pits / decomp

            float4 cracks = tex2D(_CrackTexture, IN.uv_MainTex * (1 - rocks * noise(st)) * _LavaCrackScale + (t * 0.1)) 
                            * lerp(_LavaCrackColor, _LavaCrackCooledColor, sin(_Time.x * _LavaCrackCoolSpeed));

            rocks += _LavaCooledColor * fade;

            lava = (lerp( lerp(_LavaInnerColor, _LavaVeryHotColor, sin(_Time.x * _LavaCoolSpeed)),
                     _LavaEdgeColor, (0.5 - rocks.r)/ 0.5) * (1 - gritNoise)) * fade;

            o.Albedo = (rocks * lava) + cracks;
            // Metallic and smoothness come from slider variables
            o.Metallic = 0.0; 
            o.Smoothness = 0.0;
            o.Alpha = 1.0;
            o.Emission = (rocks * lava) + cracks;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
