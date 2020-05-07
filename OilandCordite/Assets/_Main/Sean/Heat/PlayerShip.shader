Shader "Custom/PlayerShip"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HotColor ("Heat Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HeatPoint ("Heat Origin Location", float) = (0,0,0)
        _HeatRadius("Heat Point Radius", float) = 0.0
        _FalloffAmount("Fall off Amount", float) = 1.0
        _Temperature("Temperature", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float3 _HeatPoint;
        float _HeatRadius;
        float _FalloffAmount;
        float _Temperature;
        float _HotColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed3 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float heatAmout = (_HeatPoint * (1 / _HeatRadius) * _FalloffAmount * _Temperature);
            col += lerp(col, _HotColor, heatAmout);
            o.Albedo = col.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Emission = (_HeatPoint * (1 / _HeatRadius) * _FalloffAmount * _Temperature);
            o.Smoothness = _Glossiness;
            o.Alpha = fixed4(col, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
