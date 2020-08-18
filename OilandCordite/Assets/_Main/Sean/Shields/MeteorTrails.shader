Shader "Unlit/MeteorTrail"
{
    Properties 
    {
        _InnerColor ("Center Color", Color) = (1.,1.,1.,1.)
        _OuterColor ("Outer Color", Color) = (1.,1.,1.,1.)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _AnimationSpeed ("UV Animation Speed", float) = 1.

        _VertexMovementSpeed ("Vertex Movement Speed", float) = 1.
        _VertexAmplitude ("Vertex Amplitude", float) = 1.
        _VertexWavelength ("Vertex Wavelength", float) = 1.

    }
 
    SubShader 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass 
        {  
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                
                #include "UnityCG.cginc"
    
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 fragPos : TEXCOORD2;
                };
    
                struct v2f {
                    float4 vertex : SV_POSITION;
                    half2 uv : TEXCOORD0;
                    float4 fragPos : TEXCOORD2;
                };
    
                sampler2D _MainTex;
                sampler2D _NoiseTex;
                float4 _MainTex_ST;
                fixed4 _InnerColor;
                fixed4 _OuterColor;

                float _AnimationSpeed;

                float _VertexMovementSpeed;
                float _VertexAmplitude;
                float _VertexWavelength;
                
                float _Speed;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                    float k = 2 * UNITY_PI / _VertexWavelength;
                    float f = k * (worldPos.x - _VertexMovementSpeed * _Time.y);
                    o.vertex.x += _VertexAmplitude * cos(f) * tex2Dlod(_NoiseTex, v.vertex);
                    o.vertex.y += _VertexAmplitude * sin(f);

                    o.fragPos = UnityObjectToClipPos(v.vertex);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }


                fixed4 frag (v2f i) : SV_Target
                {
                    float2 uvT = float2(i.uv.x + (_Time.x * _Speed), i.uv.y);
                    fixed4 col = tex2D(_MainTex, i.uv) * lerp(_InnerColor, _OuterColor, i.uv.y);
                    return col;
                }
            ENDCG
        }
    }
}