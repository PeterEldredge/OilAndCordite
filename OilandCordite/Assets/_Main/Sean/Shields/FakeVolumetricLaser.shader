Shader "Custom/FakeVolumetricLaser" {
	Properties {
        _InnerColor("Inner Color", Color) = (1.0,1.0,1.0,1.0)
        _OuterColor("Outer Color", Color) = (1.0,1.0,1.0,1.0)
		_Fresnel("Fresnel", Range (0., 10.)) = 1.
		_AlphaOffset("Alpha Offset", Range(0., 20.)) = 1.
		_Fade("Fade", Range(0., 10.)) = 1.
        _NoiseTex("Noise Texture", 2D) = "White"

        _VertexMovementSpeed ("Vertex Movement Speed", float) = 1.
        _VertexAmplitude ("Vertex Amplitude", float) = 1.
        _VertexWavelength ("Vertex Wavelength", float) = 1.
	}
 
	SubShader 
    {
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"} 
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha One
     
        Pass 
        {  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                half2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _NoiseTex;

            float _Fresnel;
            float _AlphaOffset;
            float _Fade;
            float4 _InnerColor;
            float4 _OuterColor;

            float _VertexMovementSpeed;
            float _VertexAmplitude;
            float _VertexWavelength;

            v2f vert (appdata_t v)
            {
                v2f o;
                float noise = tex2Dlod(_NoiseTex, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                // get vertex's world position 
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float k = 2 * UNITY_PI / _VertexWavelength;
                float f = k * (o.worldPos.x - _VertexMovementSpeed * _Time.y);

                o.vertex.x += _VertexAmplitude * cos(f) * tex2Dlod(_NoiseTex, v.vertex);
                o.vertex.y += _VertexAmplitude * sin(f);

                // get world mormal
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float nu = (i.uv.x < .5)? i.uv.x : (1. - i.uv.x);
                nu = pow(nu, 2.);
                float2 n_uv = float2(nu, i.uv.y);
                float4 col = float4(1.0, 1.0, 1.0, 0.1);

                half3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float raycast = saturate(dot(viewDir, i.normal));
                float fresnel = pow(raycast, _Fresnel);

                // fade out
                float fade = saturate(pow(1. - i.uv.y, _Fade));

                col = lerp(_OuterColor, _InnerColor, fresnel);

                col.a *= fresnel * _AlphaOffset * fade;

                return col;
            }
            ENDCG
        }
    }
}