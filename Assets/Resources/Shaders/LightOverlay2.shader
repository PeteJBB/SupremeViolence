Shader "Custom/LightOverlay2" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Brightness ("Brightness", Range(0,10.0)) = 1.0
	_Intensity ("Intensity", Range(0,4.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	//Blend DstColor SrcColor
	//Blend SrcAlpha OneMinusSrcAlpha
	Blend One OneMinusSrcColor

	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {

		// pass for Brightness
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _Brightness;
			float _Intensity;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				col.rgb = tex.rgb * i.color.rgb * _Brightness;
				col.a = i.color.a * tex.a;
				col = lerp(fixed4(0,0,0,0), col, col.a);
				return col;
			}
			ENDCG 
		}

		// pass for Intensity
		Pass {
		
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _Brightness;
			float _Intensity;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				col.rgb = tex.rgb * i.color.rgb * _Brightness;
				col.a = i.color.a * tex.a;
				col = lerp(fixed4(0,0,0,0), col, col.a * _Intensity);
				return col;
			}
			ENDCG 
		}
	}
}
}