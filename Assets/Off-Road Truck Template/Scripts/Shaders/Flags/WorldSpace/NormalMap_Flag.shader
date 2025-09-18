// Mobile Flag - From ALIyerEdon : aliyeredon@gmail.com
Shader "Mobile Flag/World Space/Normal Map" 
{
	Properties
	 {
		_DiffusePower("Diffuse Power",Range(0,3)) = 1
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}


		_WaveHeight("Wave Height",Float) = .07
		_WaveSpeed("Wave Speed",Float) = 14
		_WaveCount("Wave Count",Float) = 30
	}

SubShader {

	Tags { "RenderType"="Opaque" }
	LOD 150

	CGPROGRAM

	#pragma surface surf Lambert vertex:vert

	sampler2D _MainTex;
	float _DiffusePower;
	sampler2D _BumpMap;

	float _WaveHeight;
	float _WaveSpeed;
	float _WaveCount;

	struct Input {
		float2 uv_MainTex;
		half3 vDir : TEXCOORD2;
		float2 uv_BumpMap;
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

		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}

		ENDCG   
	}

	Fallback "Diffuse"
}
