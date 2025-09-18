Shader "Custom/NewSurfaceShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Scale ("Scale", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input
		{
			float2 uv;
			float3 worldPos;
			float3 worldNormal;
		};

		CBUFFER_START(UnityPerMaterial)
			sampler2D _MainTex;
			float _Scale;
			half _Glossiness;
			half _Metallic;
			half4 _Color;
		CBUFFER_END

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv) * _Color;

			float3 worldPos = IN.worldPos / _Scale;
			float3 worldNormal = IN.worldNormal;
			float3 blends = abs(worldNormal);
			blends /= blends.x + blends.y + blends.z + 0.0001f;

			float3 projX = tex2D(_MainTex, worldPos.yz) * blends.x;
			float3 projY = tex2D(_MainTex, worldPos.xz) * blends.y;
			float3 projZ = tex2D(_MainTex, worldPos.xy) * blends.z;

			o.Albedo = projX + projY + projZ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
