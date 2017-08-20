Shader "Custom/CanyonShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Texture ("Texture", 2D) = "bump"
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _Texture;

		float3 HUEtoRGB(in float H)
		{
			float R = abs(H * 6 - 3) - 1;
			float G = 2 - abs(H * 6 - 2);
			float B = 2 - abs(H * 6 - 4);
			return saturate(float3(R, G, B));
		}

		float3 HSLtoRGB(in float3 HSL)
		{
			float3 RGB = HUEtoRGB(HSL.x);
			float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
			return (RGB - 0.5) * C + HSL.z;
		}

		float Epsilon = 1e-10;

		float3 RGBtoHCV(in float3 RGB)
		{
			// Based on work by Sam Hocevar and Emil Persson
			float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
			float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
			float C = Q.x - min(Q.w, Q.y);
			float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
			return float3(H, C, Q.x);
		}

		float3 RGBtoHSL(in float3 RGB)
		{
			float3 HCV = RGBtoHCV(RGB);
			float L = HCV.z - HCV.y * 0.5;
			float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
			return float3(HCV.x, S, L);
		}

		

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float3 hslColor = RGBtoHSL(_Color.rgb);
				
			//if (IN.worldNormal.y < (0.9 * length(IN.worldNormal))) {
				float x = IN.worldPos.x;
				float y = IN.worldPos.y;
				float z = IN.worldPos.z;
				float spatialHeightDistortion = sin((x + 2.3) / 42.3) + sin((x + 3.1) / 50.62) + sin((z + 1.6) / 64.8) + sin((z + 2.5) / 47.3);
				y = y + spatialHeightDistortion * 2;
				float heightDistortion = sin((y + 1.2) / 1.3) + sin((y + 2.7) / 3.2) + sin((y + 3.1) / 4.9);
				heightDistortion = (heightDistortion + 0.5) * 0.15;
				float temp = saturate(hslColor.z + heightDistortion);
				//float l = round( temp * 5)/ 5;
				float l = temp - (0.031 * sin(temp * 31.4159));
				//l = temp;
				float scale = 0.02;
				float u = (scale*x + scale*y) - round(scale*x + scale*y) + 0.5;
				float v = (scale*z + scale*y) - round(scale*z + scale*y) + 0.5;
				o.Normal = UnpackNormal(tex2D(_Texture, float2(u, v))).rgb;

				// Albedo comes from a texture tinted by color
				o.Albedo = HSLtoRGB(float3(hslColor.xy, l));
			//}
			//else {
			//	o.Albedo = _Color.rgb;
			//}

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
