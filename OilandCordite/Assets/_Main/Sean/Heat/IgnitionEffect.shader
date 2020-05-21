Shader "Hidden/IgnitionEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VignetteRadius("Vignette Radius", Range(0.0, 1.0)) = 1.0
        _VignetteSoftness("Vignette Softness", Range(0.0, 1.0)) = 0.5
        _VignetteColor("Vignette Color", Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _VignetteRadius;
            float _VignetteSoftness;
            float4 _VignetteColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float centerDistance = distance(i.uv.xy, float2(0.5, 0.5));
                float vignette =  1 - smoothstep(_VignetteRadius, _VignetteRadius - _VignetteSoftness, centerDistance);
                fixed4 newCol = saturate(col + (vignette * _VignetteColor));
                return newCol;
            }
            ENDCG
        }
    }
}
