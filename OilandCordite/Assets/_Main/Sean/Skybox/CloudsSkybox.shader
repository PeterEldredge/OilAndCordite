// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/CloudsSkybox"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 1, 0)
        _Color3 ("Color 3", Color) = (1,1,1,0)
        _UpVector ("Up Vector", Vector) = (0, 1, 0, 0)
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0
        _BrushNoise("Brush Texture", 2D) = "white" {}
        _StarMap("Star Map", 2D) = "white" {}
        _Intersections("Intersections", int) = 1
        _IntersectionHeight("Intersection Width", Float) = 1.0
        _IntersectionOffset("Intersection Offset", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    
        CGINCLUDE


        #include "UnityCG.cginc"

        struct appdata
        {
            float4 position : POSITION;
            float3 texcoord : TEXCOORD0;
        };
        
        struct v2f
        {
            float4 position : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            float3 normal : NORMAL;
        };
        
        int _Intersections;
        float _IntersectionHeight;
        float _IntersectionOffset;
        half4 _Color1;
        half4 _Color2;
        half4 _UpVector;
        half _Intensity;
        half _Exponent;

        sampler2D _BrushNoise;
        sampler2D _StarMap;
        SamplerState sampler_BrushNoise;
        
        v2f vert (appdata_base v)
        {
            v2f o;
            o.position = UnityObjectToClipPos (v.vertex);
            o.normal = v.normal * -1;
            o.texcoord = v.texcoord;
            return o;
        }
        
        fixed4 frag (v2f i) : COLOR
        {
            half4 brushNoise =  tex2D(_BrushNoise, i.texcoord);
            float stars = tex2D(_StarMap, i.texcoord);
            
            // Start of procedural implementation (on hold for now) 

            // half4 splatchOne = lerp(_Color1, _Color2, 1);
            // half4 splatchTwo = lerp(_Color1, _Color2, 2);
            // half4 splatchThree = lerp(_Color1, _Color2, 3);
            // if(i.texcoord.y > 0.4 + _IntersectionOffset) {
            //     return _Color2;
            // }
            // if(i.texcoord.y > 0.3 + _IntersectionOffset) {
            //     return lerp(_Color1, _Color2, 0.8);
            // }
            // if(i.texcoord.y > 0.2 + _IntersectionOffset) {
            //     half4 col = lerp(_Color1, _Color2,0.5);
            //     if(i.texcoord.y > 0.28 + _IntersectionOffset) {
            //         // sample noise
            //         // if noise value is above 0.5  round up
            //         // if noise is below 0.5 round down
            //         if(brushNoise > 0.5) {
            //             return col * brushNoise;
            //         }
            //         else {
            //             return col;
            //         }
            //     }
            //     return col;
            // }
            // if(i.texcoord.y > 0.1 + _IntersectionOffset) {
            //     return lerp(_Color1, _Color2,0.3* brushNoise);
            // }
            // return col;
            if(i.texcoord.y > 0.01) {
                float timeOffset = abs(sin(_Time.x * 5.0));
                if(stars > timeOffset || stars > 1.0) {
                    return brushNoise + (stars * timeOffset);
                }
                return brushNoise;
            }
            return _Color1;

           // return lerp (_Color1, _Color2, pow (d, _Exponent)) * _Intensity;
        }

        ENDCG
    }
}