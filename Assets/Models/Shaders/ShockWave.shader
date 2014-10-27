Shader "2Pacs/ShockWave" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
		_AnimFreq ("Animation Frequency", Float) = 1.0
		_AnimPowerX ("Animation Power X", Float) = 0.0
		_AnimPowerY ("Animation Power Y", Float) = 0.1
		_AnimPowerZ ("Animation Power Z", Float) = 0.0
		_SelfIllum ("Illumination Power", Float) = 10.0
		_Origin ("ShockWave Origin", Vector) = (0.0, 0.0, 0.0, 1.0)
		_Radius ("Radius", Float) = 100.0
		_PowerOffset ("Power Offset", Float) = 10.0 // This is a reset timer so that the color will come back gradually
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Illum;
		fixed4 _Color;
		half _Shininess;
		half _SelfIllum;
		half _PowerOffset;
		// animation 
		half _AnimFreq;
		half _AnimPowerX;
		half _AnimPowerY;
		half _AnimPowerZ;
		half4 _Origin;
		half _Radius;
		

		struct Input {
			float2 uv_MainTex;
			float illumReflection;
			fixed4 vColor;
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			half4 worldPos = mul(_Object2World, v.vertex);
			// distance to origin
			half d = length(worldPos-_Origin);
			// diff between shockwave radial position and vertex position
			half diff = _Radius - d;
			half pi = 3.14159;
			half x = diff*_AnimFreq;
			x = clamp(x,0, pi);
				
			//animation
			half3 animPower = half3(_AnimPowerX, _AnimPowerY, _AnimPowerZ);
			half3 offset = v.normal.xyz *sin(x);
			o.illumReflection = _SelfIllum * sin(x);
			half cScale = clamp(abs(diff/_PowerOffset),0.0, 1.0);
			o.vColor = _Color * pow(cScale,4) + length(offset);
			v.vertex.xyz += offset * animPower.xyz;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * IN.vColor;
			o.Albedo = c.rgb;
			fixed3 illumReflection = fixed3(IN.illumReflection,IN.illumReflection,IN.illumReflection);
			o.Emission = illumReflection +  c.rgb;
			o.Gloss = tex.a;
			o.Alpha = c.a;
			o.Specular = _Shininess;
		}
		ENDCG
	}
	FallBack "Self-Illumin/Specular"
}
