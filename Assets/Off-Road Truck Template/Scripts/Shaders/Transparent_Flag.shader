// Mobile Flag - From ALIyerEdon : aliyeredon@gmail.com
Shader "EasyFlag/Legacy/Style Two/Transparent" 
{
	Properties
	 {
		_DiffusePower("Diffuse Power",Range(0,3)) = 1
		_MainTex ("Texture", 2D) = "white" {}

		_WaveHeight("Wave Height",Float) = 1
		_WaveSpeed("Wave Speed",Float) = 140
		_WaveCount("Wave Count",Float) = 0.3
	}

SubShader {

	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 150

	CGPROGRAM

	#pragma surface surf Lambert alpha:fade   vertex:vert
	#pragma target 2.0

	sampler2D _MainTex;
	float _DiffusePower;

	float _WaveHeight;
	float _WaveSpeed;
	float _WaveCount;

	struct Input {
		float2 uv_MainTex;
		half3 vDir : TEXCOORD2;
	};

	void vert (inout appdata_full v, out Input o)
	{   

		UNITY_INITIALIZE_OUTPUT(Input,o);

	    float phase = _Time * _WaveSpeed;
	    float offset = (v.vertex.x * (v.vertex.z * 0.2)) * _WaveCount/2;
	    v.vertex.y = sin(phase + offset) * _WaveHeight;

	    o.vDir = normalize( ObjSpaceViewDir(v.vertex) ).xzy;


		    
	}

	void surf (Input IN, inout SurfaceOutput o) {
		
		half4 tex = tex2D(_MainTex, IN.uv_MainTex);

		o.Albedo = tex.rgb *_DiffusePower;

		o.Alpha = tex.a;

	}

		ENDCG   
	}

	Fallback "Diffuse"
}
