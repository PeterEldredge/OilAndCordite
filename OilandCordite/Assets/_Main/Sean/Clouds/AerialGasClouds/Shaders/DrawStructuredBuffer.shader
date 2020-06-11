// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DrawStructuredBuffer" 
{
	   
    Properties 
	{
        _Color ("Main Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimIntensity ("Rim Width", float) = 0.5
		_SubtractiveLight ("Subtractive Light", float) = 1.0
		_Alpha ("Alpha", float) = 1.0
    }
	SubShader 
	{
		Pass 
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        	Blend One OneMinusSrcAlpha
        	LOD 100
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			struct Vert
			{
				float4 position;
				float3 normal;
			};

			uniform StructuredBuffer<Vert> _Buffer;

			struct v2f 
			{
				float4  pos : SV_POSITION;			
				float2 uv : TEXCOORD0;
				float4 col : COLOR;
			};
			uniform float4 _RimColor;
        	uniform float4 _Color;
			uniform float _RimIntensity;
			uniform float _SubtractiveLight;
			uniform float _Alpha;
			//float4 _LightColor;

			v2f vert(uint id : SV_VertexID)
			{
				Vert vert = _Buffer[id];

				v2f OUT;
				OUT.pos = UnityObjectToClipPos(float4(vert.position.xyz, 1));
				
				//OUT.col = dot(float3(0,1,0), vert.normal) * 0.5 + 0.5;

				float3 viewDir = normalize(ObjSpaceViewDir(vert.position));
				float dotProduct = 1 - dot(vert.normal, viewDir);
				OUT.col = smoothstep(1 - _RimIntensity, 1.0, dotProduct);
				
				OUT.col *= _RimColor;
				
				return OUT;
			}

			float4 frag(v2f IN) : COLOR
			{
				float4 col = ((IN.col + _Color) + unity_AmbientSky) * _SubtractiveLight;
				col.a = _Alpha;
				return col;
			}

			ENDCG

		}
	}
}