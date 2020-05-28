Shader "Custom/EmissiveLaser"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Emission ("Emission", Range(0,1000)) = 0.0
        _Alpha ("Alpha", Range(0,1)) = 1.0
        _RimColor("Rim Color", Color) = (1.0,1.0,1.0,1.0)
        _RimIntensity ("Rim Intensity", Range(0.5,8.0)) = 1.0
        _VertexNoise("Displacement Map", 2D) = "white" {}
        _VibrateAmount("Displacement Amount", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha:premul

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _VertexNoise;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 objPos;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        float _Emission;
        float _Alpha;
        fixed4 _Color;
        float4 _RimColor;
        float4 _RimIntensity;
        float _VibrateAmount;

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float jiggleSample = tex2Dlod(_VertexNoise, v.vertex);
            v.vertex.x *= (1 - (cos(_Time.x * 100) * jiggleSample *  _VibrateAmount));
		    v.vertex.z *= (1 - (cos(_Time.y * 10) * jiggleSample *  _VibrateAmount));
            v.vertex.y *= (1 - (cos(_Time.z * 1) * jiggleSample *  _VibrateAmount));
            o.objPos = v.vertex;
        }

        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimIntensity) * _Emission;
            o.Alpha = c.a * _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
