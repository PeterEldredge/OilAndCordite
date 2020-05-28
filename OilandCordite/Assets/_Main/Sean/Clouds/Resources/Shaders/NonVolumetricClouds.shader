Shader "Custom/NonVolumetricClouds"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _VertexNoise ("Vertex Noise Map", 2D) = "white" {}
        _Amount("Vert Jiggle amount", float) = 0.0
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimIntensity ("Rim Intensity", Range(0.5,8.0)) = 1.0
        _fadeOffIntensity ("Fadeoff Intensity", Range(0.0,3.0)) = 1.0
        _CloudBaseColor ("Cloud Base Color", Color) = (1,1,1,1)
        _CloudPeakColor ("Cloud Peak Color", Color) = (1,1,1,1)
        _ShadowColor ("Cloud Shadow Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        ZWrite on
        LOD 200


        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha 

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
        fixed4 _Color;
        float _Amount;
        float4 _RimColor;
        float _RimIntensity;
        float _fadeOffIntensity;

        float4 _CloudPeakColor;
        float4 _CloudBaseColor;
        float4 _ShadowColor;

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float jiggleSample = tex2Dlod(_VertexNoise, v.vertex);
            v.vertex.x *= (1 - (cos(_Time.x * 100) * jiggleSample * _Amount));
		    v.vertex.z *= (1 - (cos(_Time.y * 10) * jiggleSample * _Amount));
            v.vertex.y *= (1 - (cos(_Time.z * 1) * jiggleSample * _Amount));
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

        half4 LightingCSLambert (SurfaceOutput s, half3 lightDir, half atten) 
        {
            fixed diff = max (0, dot (s.Normal, lightDir));

            fixed4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
            
            //shadow colorization
            c.rgb += _ShadowColor.xyz * max(0.0,(1.0-(diff*atten*2))) * _Color;
            c.a = s.Alpha;
            return c;
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
            fixed3 col = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float jiggleSample = tex2D(_VertexNoise, IN.uv_MainTex);

            float4 newCol = lerp(_CloudBaseColor, _CloudPeakColor, 1.0);
            float3 localPos = IN.objPos;
            half4 atten = unity_LightAtten[3];
            newCol *= _ShadowColor * (1-atten);
           // col += lerp(col, _HotColor, heatAmout);
            o.Albedo = newCol.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimIntensity);
            o.Alpha = fixed4(newCol.rgb * pow (rim, _fadeOffIntensity) * jiggleSample, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
