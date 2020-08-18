Shader "Custom/PlayerShip"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _AdditiveColor ("Additive Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _HotColor ("Heat Color", Color) = (1,1,1,1)
        _VeryHotColor ("Very Hot Color", Color) = (1,1,1,1)
        _NuclearColor ("Nucelar Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _VertexNoise ("Vertex Noise Map", 2D) = "white" {}
        _HeatNoise("Heat Noise Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HeatPoint ("Heat Origin Location", float) = (0,0,0)
        _HeatRadius("Heat Point Radius", float) = 0.0
        _FalloffAmount("Fall off Amount", float) = 1.0
        _Temperature("Temperature", float) = 0.0
        _Amount("Vert Jiggle amount", float) = 0.0
        _GradientFade("Heat Gradient Fade", float) = 0.5
        _LowerGradientFade("Lower Gradient Fade", float) = 0.9

        // individual wave settings 
        _Amplitude ("Wave Size", Range(0,1)) = 0.4
        _Frequency ("Wave Freqency", Range(1, 200)) = 2
        _Offset("what", float) = 0.5
    }
    SubShader
    {
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _VertexNoise;
        sampler2D _HeatNoise;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 objPos;
        };
        
        const float PI_OVR_2 = 1.57079632679489661923;
        

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _AdditiveColor;

        float _Amount;
        float3 _HeatPoint;
        float _HeatRadius;
        float _FalloffAmount;
        float _Temperature;
        fixed4 _HotColor;
        fixed4 _VeryHotColor;
        fixed4 _NuclearColor;
        float _GradientFade;
        float _LowerGradientFade;

        float _Amplitude;
        float _Frequency;
        float _Offset;

        static const float _TwoPi = 6.28318530718;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            fixed3 jiggleSample = tex2Dlod(_VertexNoise, v.vertex);
            v.vertex.x *= (1 - (_Temperature * cos(_Time.x * 100) * jiggleSample * _Amount));
		    v.vertex.z *= (1 - (_Temperature * sin(_Time.x * 100) * jiggleSample * _Amount));
            o.objPos = v.vertex;

        //    //float k = 2 * UNITY_PI / _Wavelength;
		// 	//p.y = _Amplitude * sin(k * (p.x - _Speed * _Time.y));
        //     float period = 0.1 * _TwoPi / 5;
        //     //v.vertex.y *= 1 - sin(period * (10 * _Time.y));
        //     // if(v.vertex.z < _Offset) 
        //     // {
        //     //     v.vertex.y *= (1 - sin(period * (v.vertex.z * _Frequency + _SinTime.w * 5)) * _Amplitude * v.normal.y);
        //     // }
            
        }

        // code to swap from a 2 color gradient to a 3 color gradient depending on temperature
        float3 buildHeatGradient(float3 localCoords) {
            float4 nuclearColor = float4(0.0, 0.0, 0.0, 0.0);
            float baseColor = 0.0;
            float4 heatColor = float4(0.0, 0.0, 0.0, 0.0);
            baseColor = lerp(heatColor, _HotColor, localCoords.z / _GradientFade * 0.5) * step(localCoords.z, _GradientFade * 0.5 * _Time.x);
            nuclearColor = lerp(_HotColor, _VeryHotColor, localCoords.z / _GradientFade * _Temperature) * step(localCoords.z, _GradientFade * _Temperature);
            nuclearColor += lerp(_VeryHotColor, _NuclearColor, (localCoords.z - _GradientFade * _Temperature) / (1 - _GradientFade * _Temperature)) * step(_GradientFade * _Temperature, localCoords.z);
            return _Temperature * (baseColor + nuclearColor);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed3 col = (tex2D (_MainTex, IN.uv_MainTex) * _Color) + _AdditiveColor;
            float3 localPos = IN.objPos;
            float heatAmout = (_HeatPoint * (1 / _HeatRadius) * _FalloffAmount * _Temperature);
           // col += lerp(col, _HotColor, heatAmout);
            o.Albedo = col.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            // o.Emission = _Temperature * lerp(_VeryHotColor.rgb, _HotColor.rgb, localPos.z) * tex2D(_VertexNoise, IN.uv_MainTex);
            o.Emission = buildHeatGradient(localPos) + _AdditiveColor;
            o.Smoothness = _Glossiness;
            o.Alpha = fixed4(col, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
