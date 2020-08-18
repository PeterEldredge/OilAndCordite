Shader "Custom/ForceField"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LowColor ("Low Health Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _CrackTex ("Crack Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _VertexNoiseMap ("Vertex Noise Map", 2D) = "white" {}
        _Amount ("Vertex Noise Amount", float) = 1.
        _Alpha ("Alpha", Range(0,1)) = 1.
        _Ramp ("Ramp Distance", float) = 1.
        _EmissionScale ("Excessive Emission Sclae", float) = 10.
        _ScrollSpeed ("Scroll Speed", float) = 1.
        _ShieldHealth ("Shield Damage", Range(0,1)) = 1.
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.5

        sampler2D _MainTex;
        sampler2D _VertexNoiseMap;
        sampler2D _CrackTex;

        float _Amount;
        float _Alpha;
        float _ShieldHealth;
        float _Ramp;
        float _EmissionScale;
        float _ScrollSpeed;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 st_MainTex2 : TEXCOORD1;
            float3 normal : TEXCOORD2;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _LowColor;

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            fixed3 jiggleSample = tex2Dlod(_VertexNoiseMap, v.vertex);
            float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));

            v.vertex.x *= (1 - (cos(_Time.x * 100) * jiggleSample * _Amount));
		    v.vertex.z *= (1 - (sin(_Time.x * 100) * jiggleSample * _Amount));
            o.st_MainTex2 = float2( abs (dot (viewDir, v.normal)), 0.5);
            o.normal = v.normal;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            float edgeRamp = lerp(0.0, 1.0, 1.0 - clamp(IN.st_MainTex2.x - (_Ramp - 0.5) * 2.0, 0.0, 1.0) );
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color, _LowColor, _ShieldHealth);
            fixed4 cracks = tex2D (_CrackTex, IN.uv_MainTex + (_Time.x * _ScrollSpeed)) * IN.uv_MainTex.y * _ShieldHealth;
            o.Albedo = (c.rgb + (1. * cracks.rgb * 5.)) * edgeRamp;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = (_Alpha * edgeRamp) + (1. * cracks * 5.);
            o.Emission = cracks * (edgeRamp * c) * _EmissionScale;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
