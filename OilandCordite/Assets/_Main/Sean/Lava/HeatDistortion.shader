Shader "GrabPassInvert"
{
    Properties 
    {
        _DistortionNoise("Distortion Noise", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", float) = 1.0
        _DistortionSpeed("Distortion Speed", float) = 1.0
        _xScale("Billboard X Scale", float) = 1.0
        _yScale("Billboard Y Scale", float) = 1.0
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        ZTest always

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BackgroundTexture;
            sampler2D _DistortionNoise;

            float _DistortionStrength;
            float _DistortionSpeed;
            float _xScale;
            float _yScale;

            struct appdata 
            {
                float4 vertex : POSITION;
                float4 texCoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 grabPos: TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;

                float4 pos = v.vertex;
                float4 originInViewSpace = mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1));
                float4 vertInViewSpace = originInViewSpace + float4(pos.x, pos.z, 0, 0);
                pos = mul(UNITY_MATRIX_P, vertInViewSpace) * float4(_xScale, _yScale, 1.0, 1.0);
                o.pos = pos;
                o.grabPos = ComputeGrabScreenPos(o.pos);

                float noise = tex2Dlod(_DistortionNoise, v.texCoord).rgb;
                o.grabPos.x += cos(noise * _Time.x * _DistortionSpeed) * _DistortionStrength;
                o.grabPos.y += sin(noise * _Time.x * _DistortionSpeed) * _DistortionStrength;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
                return bgcolor;
            }
            ENDCG
        }

    }
}