Shader "Custom/Ripple"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (0,0,1,0.1)
		_RippleData("RippleData", Vector) = (0.5, 0.5, 2, 0) // posX, posY, Timer , UNDEFINED
		_RippleData2("RippleData2", Vector) = (0.7, 0.5, 2, 0) // posX, posY, Timer , UNDEFINED
		_RippleData3("RippleData3", Vector) = (0.2, 0.5, 2, 0) // posX, posY, Timer , UNDEFINED
		_RippleData4("RippleData4", Vector) = (0.5, 0.9, 2, 0) // posX, posY, Timer , UNDEFINED
		_ResetTimer("Duration", float) = 1
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}
			ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

			LOD 100

			Pass
			{
				/*Stencil {
					Ref 0
					Comp Equal
					Pass IncrSat
					Fail IncrSat
				}*/

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				float4 _RippleData;
		float4 _RippleData2;
		float4 _RippleData3;
		float4 _RippleData4;
				float4 _Color;
				sampler2D  _MainTex;

				///////////////////////////  rippleOffset function by andrei512 https://www.shadertoy.com/view/4lcBzN
				static const fixed PI = 3.14159265359f;

				fixed easeInOut(fixed t) {
					return (t * t) * (3.0f - 2.0f * t);
				}

		
				fixed sinc(fixed x) {
					return sin(x) / x;
				}

				// -6 to 6 give visible diffs - more than that
				fixed sigmoid(fixed x) {
					return 1.0 / (1.0 + exp(-x));
				}

				fixed lerp(fixed a, fixed b, fixed t) {
					return a + (b - a) * t;
				}

				#define EFFECT_RANGE_PARAM 0.15
				#define EFFECT_INTENSITY_PARAM 0.07

				/**
				* p: the point for which to compute the offset
				* center: the center of the ripple
				* age: the time of the effect
				**/
				fixed2 rippleOffset(fixed2 p, fixed2 center, fixed age) {
					fixed2 offset = (center - p) * fixed2(1.0, 1.0);

					fixed centerOffset = length(offset);

					fixed distanceFactor = 1.0 - sigmoid(-4.0 + 6.0 * (centerOffset - EFFECT_RANGE_PARAM));

					fixed slope = sinc(12.0 * (easeInOut(age) - centerOffset * 0.7)  * PI * 2.0) * distanceFactor;

					return normalize(offset) * slope * EFFECT_INTENSITY_PARAM;
				}
				///////////////////////////////////////////////////////////////

				struct appdata
				{
					float4 vertex : POSITION;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 uv : TEXCOORD0;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = fixed4(v.texcoord.xy, 0, 0);
					return o;
				}

				half4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
			
					fixed2 offset = 0;
					if (_RippleData.z != 0)
					{
						fixed time = frac(_Time.y / _RippleData.z);

						_RippleData.xy = fixed2(0.5, 0.5);
						offset = rippleOffset(i.uv, _RippleData.xy, frac((_Time.y+2) / _RippleData.z));
						/*offset += rippleOffset(i.uv, _RippleData2.xy, frac((_Time.y + _RippleData2.w) / _RippleData2.z));
						offset += rippleOffset(i.uv, _RippleData3.xy, frac((_Time.y + _RippleData3.w) / _RippleData3.z));
						offset += rippleOffset(i.uv, _RippleData4.xy, frac((_Time.y + _RippleData4.w) / _RippleData4.z));*/
					}
					col.w = (offset.x + offset.y)*10;

					return col;
				}
			ENDCG
		}
	}
}