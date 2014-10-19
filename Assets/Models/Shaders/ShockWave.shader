Shader "2Pacs/ShockWave" {
	Properties {
		_Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10.0
		_AnimFreq ("Animation Frequency", Float) = 1.0
		_AnimPowerX ("Animation Power X", Float) = 0.0
		_AnimPowerY ("Animation Power Y", Float) = 0.1
		_AnimPowerZ ("Animation Power Z", Float) = 0.0
		_SelfIllu ("Illumination Power", Float) = 10.0
		_Origin ("ShockWave Origin", Vector) = (0.0, 0.0, 0.0, 1.0)
		_Radius ("Radius", Float) = 1.0
	}
	SubShader {
		Pass {
			Tags {"LightMode" = "PrepassFinal"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// user defined variables
			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _SpecColor;
			uniform half _Shininess;
			uniform half _SelfIllu;
			
			// animation 
			uniform half _AnimFreq;
			uniform half _AnimPowerX;
			uniform half _AnimPowerY;
			uniform half _AnimPowerZ;
			uniform half4 _Origin;
			uniform half _Radius;

			//unity defined variables
			uniform half4 _LightColor0;

			// base structure of vertex shader input
			struct vertexInput{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 texcoord: TEXCOORD0;	
			};

			// structure of output from the vertex shader.
			// this will be the input to the fragment shader.
			struct vertexOutput{
				half4 pos		: SV_POSITION;				
				fixed3 normalDir: TEXCOORD0;
				fixed4 lightDir	: TEXCOORD1;
				fixed3 viewDir	: TEXCOORD2;
				float4 tex		: TEXCOORD4;
			};

			// vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
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
				half3 offset = sin( (v.normal * x));

				half4 newPos = v.vertex;
				newPos.xyz = newPos.xyz + offset * animPower.xyz;
			
				// normal direction
				o.normalDir = normalize( mul( half4(v.normal, 0.0), _World2Object ).xyz );

				// unity transform position
				// transforms the input position to the position in screen space.
				o.pos = mul(UNITY_MATRIX_MVP, newPos);

				// world position
				half4 posWorld = mul(_Object2World, newPos);

				// calc viewDir
				// the view direction is the direction from the vertex (in world space) to the camera
				o.viewDir = normalize( _WorldSpaceCameraPos.xyz - posWorld.xyz);

				// light direction
				half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
				
				o.lightDir = fixed4(
					normalize( lerp(_WorldSpaceLightPos0.xyz, fragmentToLightSource, _WorldSpaceLightPos0.w)),
					lerp(1.0, 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
				);
				o.tex = v.texcoord;
				return o;
			}

			// fragment function
			fixed4 frag(vertexOutput i) : COLOR
			{
				// calc how 
				fixed nDotL = saturate(dot(i.normalDir, i.lightDir.xyz));
				//diffuse
				fixed3 diffuseReflection = i.lightDir.w * _LightColor0 * nDotL;
				fixed3 specularReflection = diffuseReflection * _SpecColor.xyz * pow( saturate( dot( reflect( -i.lightDir.xyz, i.normalDir.xyz), i.viewDir)),_Shininess);
				fixed3 lightFinal = _SelfIllu * UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT.xyz;

				// Texture Maps
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);

				return fixed4(tex.xyz * lightFinal * _Color.xyz, 1.0);
			}
			ENDCG
		}
		
		
	} 
	FallBack "Diffuse"
}
