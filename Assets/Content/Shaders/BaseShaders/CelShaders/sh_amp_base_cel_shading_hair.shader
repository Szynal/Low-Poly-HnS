// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CelShading/sh_amp_base_cel_shading_hair"
{
	Properties
	{
		_MainTex("BaseColor", 2D) = "white" {}
		_EmissionMap("Emissive", 2D) = "black" {}
		_SpecGlossMap("MRAO", 2D) = "white" {}
		_BumpMap("Normal", 2D) = "bump" {}
		_CelRampTexture("CelRampTexture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,0)
		_Contrast("Contrast", Range( 0.1 , 2)) = 1
		_Brightness("Brightness", Range( 0 , 2)) = 1
		_ShadowBrightness("ShadowBrightness", Range( 0 , 1)) = 0
		_NormalIntensity("NormalIntensity", Range( 0 , 2)) = 1
		_Rim("Rim", Range( 0 , 1)) = 0.125
		_MetallicMin("MetallicMin", Range( 0 , 2)) = 0
		_MetallicMax("MetallicMax", Range( 0 , 2)) = 1
		_RoughnessMin("RoughnessMin", Range( 0 , 2)) = 0
		_RoughnessMax("RoughnessMax", Range( 0 , 2)) = 1
		_HairNoise("HairNoise", 2D) = "white" {}
		_HairSpecularShift("HairSpecularShift", Range( -1 , 1)) = -0.25
		_PointLightCelRamp("PointLightCelRamp", 2D) = "white" {}
		_HairHighlightPower("HairHighlightPower", Range( 0 , 16)) = 6
		_CenterHighlightPower("CenterHighlightPower", Range( 0 , 16)) = 9
		_HairHighlightStrength("HairHighlightStrength", Range( 0 , 1)) = 0.3
		_CenterHighlightStrength("CenterHighlightStrength", Range( 0 , 1)) = 0.5
		_CullMode("CullMode", Range( 0 , 2)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 viewDir;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _CullMode;
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform float _Contrast;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Tint;
		uniform float _Brightness;
		uniform sampler2D _SpecGlossMap;
		uniform float4 _SpecGlossMap_ST;
		uniform float _MetallicMin;
		uniform float _MetallicMax;
		uniform sampler2D _CelRampTexture;
		uniform float _NormalIntensity;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _ShadowBrightness;
		uniform sampler2D _PointLightCelRamp;
		uniform float _RoughnessMin;
		uniform float _RoughnessMax;
		uniform float _HairSpecularShift;
		uniform float _HairHighlightPower;
		uniform float _HairHighlightStrength;
		uniform float _CenterHighlightPower;
		uniform float _CenterHighlightStrength;
		uniform sampler2D _HairNoise;
		uniform float4 _HairNoise_ST;
		uniform float _Rim;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 Diffuse376 = ( CalculateContrast(_Contrast,( tex2D( _MainTex, uv_MainTex ) * _Tint )) * _Brightness );
			float2 uv_SpecGlossMap = i.uv_texcoord * _SpecGlossMap_ST.xy + _SpecGlossMap_ST.zw;
			float4 tex2DNode274 = tex2D( _SpecGlossMap, uv_SpecGlossMap );
			float AmbientOcclusion354 = tex2DNode274.b;
			float4 DiffuseWithAO355 = ( Diffuse376 * AmbientOcclusion354 );
			float lerpResult496 = lerp( _MetallicMin , _MetallicMax , tex2DNode274.r);
			float Metallic352 = saturate( lerpResult496 );
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult280 = normalize( ( _WorldSpaceLightPos0.xyz - ase_worldPos ) );
			float3 lerpResult285 = lerp( _WorldSpaceLightPos0.xyz , normalizeResult280 , _WorldSpaceLightPos0.w);
			float3 LightDirection358 = lerpResult285;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 NormalTextureTS349 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), _NormalIntensity );
			float dotResult308 = dot( LightDirection358 , normalize( (WorldNormalVector( i , NormalTextureTS349 )) ) );
			float temp_output_408_0 = ( saturate( dotResult308 ) * ase_lightAtten );
			float2 temp_cast_1 = (temp_output_408_0).xx;
			float4 lerpResult506 = lerp( tex2D( _CelRampTexture, temp_cast_1 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 lerpResult315 = lerp( ase_lightColor.rgb , float3( 1,1,1 ) , Metallic352);
			float2 temp_cast_3 = (temp_output_408_0).xx;
			float4 lerpResult518 = lerp( tex2D( _PointLightCelRamp, temp_cast_3 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			float LightType514 = _WorldSpaceLightPos0.w;
			float4 lerpResult519 = lerp( ( lerpResult506 * float4( lerpResult315 , 0.0 ) ) , lerpResult518 , LightType514);
			float4 lerpResult511 = lerp( lerpResult519 , ( lerpResult519 * float4( lerpResult315 , 0.0 ) * ase_lightAtten ) , LightType514);
			float4 Lighting356 = lerpResult511;
			float3 indirectNormal322 = WorldNormalVector( i , NormalTextureTS349 );
			float lerpResult499 = lerp( _RoughnessMin , _RoughnessMax , tex2DNode274.g);
			float Roughness353 = saturate( lerpResult499 );
			Unity_GlossyEnvironmentData g322 = UnityGlossyEnvironmentSetup( ( 1.0 - Roughness353 ), data.worldViewDir, indirectNormal322, float3(0,0,0));
			float3 indirectSpecular322 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal322, g322 );
			float lerpResult326 = lerp( 0.5 , 1.0 , Metallic352);
			float4 Reflections369 = ( DiffuseWithAO355 * float4( indirectSpecular322 , 0.0 ) * lerpResult326 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentPos = mul( ase_worldToTangent, ase_normWorldNormal);
			float3 normalizeResult464 = normalize( cross( worldToTangentPos , float3(1,0,0) ) );
			float3 Tangent466 = normalizeResult464;
			float3 normalizeResult3_g2 = normalize( ( Tangent466 + ( NormalTextureTS349 * _HairSpecularShift ) ) );
			float3 temp_output_473_0 = normalizeResult3_g2;
			float3 normalizeResult7_g9 = normalize( ( LightDirection358 + i.viewDir ) );
			float dotResult8_g9 = dot( temp_output_473_0 , normalizeResult7_g9 );
			float smoothstepResult10_g9 = smoothstep( -1.0 , 0.0 , dotResult8_g9);
			float3 normalizeResult7_g10 = normalize( ( LightDirection358 + i.viewDir ) );
			float dotResult8_g10 = dot( temp_output_473_0 , normalizeResult7_g10 );
			float smoothstepResult10_g10 = smoothstep( -1.0 , 0.0 , dotResult8_g10);
			float2 uv_HairNoise = i.uv_texcoord * _HairNoise_ST.xy + _HairNoise_ST.zw;
			float4 temp_output_327_0 = ( ase_lightColor * ( ( ( smoothstepResult10_g9 * ( pow( sqrt( ( 1.0 - ( dotResult8_g9 * dotResult8_g9 ) ) ) , pow( 2.0 , _HairHighlightPower ) ) * _HairHighlightStrength ) ) + ( smoothstepResult10_g10 * ( pow( sqrt( ( 1.0 - ( dotResult8_g10 * dotResult8_g10 ) ) ) , pow( 2.0 , _CenterHighlightPower ) ) * _CenterHighlightStrength ) ) ) * tex2D( _HairNoise, uv_HairNoise ) ) * ase_lightAtten );
			float4 lerpResult334 = lerp( temp_output_327_0 , ( temp_output_327_0 * Diffuse376 ) , Metallic352);
			float4 Specular366 = lerpResult334;
			float4 SpecularLight372 = ( lerpResult326 * Specular366 );
			UnityGI gi323 = gi;
			float3 diffNorm323 = WorldNormalVector( i , NormalTextureTS349 );
			gi323 = UnityGI_Base( data, 1, diffNorm323 );
			float3 indirectDiffuse323 = gi323.indirect.diffuse + diffNorm323 * 0.0001;
			float4 AmbientLight373 = ( float4( indirectDiffuse323 , 0.0 ) * DiffuseWithAO355 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV399 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode399 = ( 0.0 + _Rim * pow( 1.0 - fresnelNdotV399, 2.0 ) );
			float lerpResult405 = lerp( 0.0 , fresnelNode399 , ase_lightAtten);
			float RimLight403 = lerpResult405;
			c.rgb = ( ( ( DiffuseWithAO355 - ( DiffuseWithAO355 * Metallic352 ) ) * Lighting356 ) + Reflections369 + SpecularLight372 + AmbientLight373 + RimLight403 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			o.Emission = tex2D( _EmissionMap, uv_EmissionMap ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16800
8;81;1788;790;1763.633;1819.875;1.266997;True;False
Node;AmplifyShaderEditor.CommentaryNode;465;-3243.102,1368.272;Float;False;1743.113;519.8682;Tangent;7;472;461;464;460;463;462;466;Tangent;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;386;-3202.844,-420.3011;Float;False;1319.308;360.7815;Light Direction (All lights);7;273;272;277;280;285;358;514;Light Direction (All lights);1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;461;-2933.309,1423.272;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;276;-3206.239,-792.8167;Float;False;1087.245;281.9016;Normal;3;349;290;279;Normal;0,0.681592,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;272;-3152.844,-221.9301;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;273;-3152.844,-366.5197;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.TransformPositionNode;472;-2693.202,1427.11;Float;False;World;Tangent;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;463;-2948.841,1651.417;Float;False;Constant;_Vector0;Vector 0;12;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;277;-2764.321,-218.9138;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-3070.045,-734.0106;Float;False;Property;_NormalIntensity;NormalIntensity;9;0;Create;True;0;0;False;0;1;0.75;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;462;-2433.115,1426.48;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;290;-2687.606,-730.4263;Float;True;Property;_BumpMap;Normal;3;0;Create;False;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;280;-2572.321,-218.9138;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;285;-2381.065,-346.0043;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;464;-2180.115,1431.48;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;349;-2367.638,-728.6326;Float;False;NormalTextureTS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;269;-1277.237,-379.8776;Float;False;2492.281;616.5393;Lighting;20;356;325;315;506;311;381;507;341;408;316;407;308;300;359;385;512;511;517;518;519;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;-2126.538,-349.0815;Float;False;LightDirection;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;288;-3629.822,-2094.782;Float;False;371;280;Base Color;1;291;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;275;-1250.96,983.2731;Float;False;2759.73;1046.864;Specular light;24;334;382;335;377;327;489;314;338;492;488;490;477;483;494;482;480;485;495;473;475;476;479;474;493;Anisotropic Specular Light;0.9961122,1,0.6367924,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;466;-1878.487,1430.341;Float;False;Tangent;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;271;-3190.651,-1546.355;Float;False;1092.328;677.0292;MRAO;12;353;499;354;352;500;501;496;498;274;497;508;509;MRAO;0.699,1,0.9582815,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;385;-1251.833,-126.9885;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;291;-3579.822,-2044.781;Float;True;Property;_MainTex;BaseColor;0;0;Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;493;-768.9745,1804.483;Float;False;Property;_CenterHighlightPower;CenterHighlightPower;19;0;Create;True;0;0;False;0;9;1000;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;476;-1192.556,1202.85;Float;False;Property;_HairSpecularShift;HairSpecularShift;16;0;Create;True;0;0;False;0;-0.25;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;390;-3579.595,-1790.897;Float;False;Property;_Tint;Tint;5;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;474;-1213.401,1113.087;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;479;-763.3806,1564.101;Float;False;Property;_HairHighlightPower;HairHighlightPower;18;0;Create;True;0;0;False;0;6;1000;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;300;-981.6669,-123.4821;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;497;-3137.356,-1496.157;Float;False;Property;_MetallicMin;MetallicMin;11;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;274;-3143.003,-1076.637;Float;True;Property;_SpecGlossMap;MRAO;2;0;Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;359;-1245.37,-256.4075;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;475;-1195.639,1288.84;Float;False;466;Tangent;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;498;-3139.628,-1399.001;Float;False;Property;_MetallicMax;MetallicMax;12;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-3197.865,-1790.781;Float;False;Property;_Contrast;Contrast;6;0;Create;True;0;0;False;0;1;0;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;480;-765.1511,1670.035;Float;False;Property;_HairHighlightStrength;HairHighlightStrength;20;0;Create;True;0;0;False;0;0.3;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;495;-384.4713,1796.102;Float;False;2;0;FLOAT;2;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;473;-773.5588,1111.337;Float;False;shf_amp_ShiftTangent;-1;;2;c80122aca3a414148bba9258421709b0;0;3;4;FLOAT3;0,0,0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;494;-770.7451,1910.417;Float;False;Property;_CenterHighlightStrength;CenterHighlightStrength;21;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;-3070.154,-2045.619;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;483;-762.3878,1405.201;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;485;-405.2266,1556.223;Float;False;2;0;FLOAT;2;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;496;-2754.319,-1497.636;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;308;-767.6132,-255.922;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;482;-764.8706,1283.737;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;407;-986.2292,125.8057;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;316;-637.1469,-258.0952;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;505;-2815.692,-2044.34;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;508;-2555.467,-1497.615;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;490;-129.4517,1673.314;Float;True;shf_amp_StrandSpec;-1;;10;1b4251b6bac30f540bccc9ac5466b8f5;0;5;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;477;-126.6543,1157.027;Float;True;shf_amp_StrandSpec;-1;;9;1b4251b6bac30f540bccc9ac5466b8f5;0;5;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-2813.052,-1789.557;Float;False;Property;_Brightness;Brightness;7;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;501;-3145.297,-1171.238;Float;False;Property;_RoughnessMax;RoughnessMax;14;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;408;-433.0691,-255.6138;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;352;-2378.315,-1481.722;Float;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;500;-3141.768,-1271.258;Float;False;Property;_RoughnessMin;RoughnessMin;13;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;488;-129.2803,1406.415;Float;True;Property;_HairNoise;HairNoise;15;0;Create;True;0;0;False;0;7a0fa0ad909f0ec42a1feee6e9d5d7a8;7a0fa0ad909f0ec42a1feee6e9d5d7a8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;492;188.6655,1158.495;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-2553.94,-2045.864;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;489;385.283,1149.405;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;-257.6552,141.7328;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;311;-254.5489,15.96183;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;314;387.6474,1030.971;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2304.748,-2055.687;Float;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;338;391.4701,1407.136;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;507;-253.7351,-66.90482;Float;False;Property;_ShadowBrightness;ShadowBrightness;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;499;-2755.808,-1273.612;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;341;-251.418,-258.121;Float;True;Property;_CelRampTexture;CelRampTexture;4;0;Create;False;0;0;False;0;b475a03964fa64642b09599531c2e016;b475a03964fa64642b09599531c2e016;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;327;706.2095,1152.188;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;377;638.6137,1410.107;Float;False;376;Diffuse;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;354;-2368.309,-1016.562;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;516;-50.1361,121.5645;Float;True;Property;_PointLightCelRamp;PointLightCelRamp;17;0;Create;True;0;0;False;0;6daf97bc07c8414469b76460d2739c18;6daf97bc07c8414469b76460d2739c18;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;315;-1.84108,1.049855;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;506;78.15466,-253.6848;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;514;-2125.65,-183.7854;Float;False;LightType;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;509;-2558.775,-1276.003;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;382;896.2582,1411.901;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;312;-1275.226,-1620.016;Float;False;1150.537;362.6572;Reflections;8;515;369;328;365;322;364;380;520;Reflections;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;252.2241,-252.16;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;518;268.7214,-120.85;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;378;-2304.012,-1918.321;Float;False;354;AmbientOcclusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;517;334.9164,34.70243;Float;False;514;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;898.9175,1283.556;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2368.671,-1271.529;Float;False;Roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;303;-1947.294,-2100.134;Float;False;219;183;Applying AO;1;310;Applying AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;380;-1221.84,-1420.657;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;519;523.3066,-255.9663;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;-1918.93,-2047.986;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;313;-1275.598,-769.5593;Float;False;1151.336;281.2077;Ambient Light;5;373;330;375;368;323;Ambient Light;0.5704813,0.3537736,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;334;1150.046,1157.514;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;512;699.6552,-90.8894;Float;False;267;292.1541;Square Light Fix;2;513;510;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;309;-1277.541,-1193.413;Float;False;1172.302;309.561;Specular;6;320;379;372;367;326;332;Specular Light;0.9076021,1,0.6556604,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;402;-1266.37,347.308;Float;False;1465.171;439.3994;RimLighting;6;405;403;404;399;401;394;RimLighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;520;-1027.507,-1418.236;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;-1219.203,-694.9748;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;379;-1208.124,-998.3226;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;366;1318.617,1157.958;Float;False;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;364;-1219.553,-1551.782;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;401;-1216.37,659.3408;Float;False;Property;_Rim;Rim;10;0;Create;True;0;0;False;0;0.125;0.125;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;510;749.6552,86.26472;Float;False;514;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;355;-1532.359,-2045.797;Float;False;DiffuseWithAO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;513;753.5067,-40.88942;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-1208.541,-1126.413;Float;False;Constant;_SpecularBrightness;SpecularBrightness;5;0;Create;True;0;0;False;0;0.5;0.04;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;394;-958.8309,404.2001;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;306;-1292.299,-2135.167;Float;False;771.9758;331.8977;Metallic Diffuse;3;324;319;363;Metallic Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;511;768.2325,-252.2103;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;326;-820.4957,-1126.122;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;-1225.917,-1934.085;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;404;-702.299,653.3048;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;375;-1222.172,-568.9113;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;323;-959.5091,-695.6359;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;-820.2244,-989.5296;Float;False;366;Specular;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectSpecularLight;322;-877.8187,-1424.492;Float;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;365;-951.1255,-1551.236;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;399;-702.1306,399.2131;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-699.8748,-697.4003;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;321;-377.8059,-2138.762;Float;False;406;334;Applying Lighting;2;362;329;Applying Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-972.9394,-1951.116;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;-556.2476,-1123.436;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;356;944.1309,-265.6577;Float;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;405;-319.2257,395.2631;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;328;-573.0942,-1555.015;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;324;-765.0978,-2046.306;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-449.0291,-694.1271;Float;False;AmbientLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;331;295.5503,-2101.319;Float;False;724.2165;700.7795;Applying Reflections, Specular, AO;5;406;374;371;370;333;Applying Reflections, Specular, AO, Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;-350.292,-1971.815;Float;False;356;Lighting;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;369;-361.2097,-1553.392;Float;False;Reflections;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;403;-125.5472,394.9881;Float;False;RimLight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;372;-374.6343,-1127.816;Float;False;SpecularLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;370;389.2529,-1919.44;Float;False;369;Reflections;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;433;-3204.964,713.8862;Float;False;1339.592;671.3103;HalfVector;8;432;426;427;425;430;428;429;431;HalfVector;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-167.6868,-2058.974;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;392;595.4392,-2612.795;Float;False;370.9999;280;Emissive;1;391;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;383.4047,-1790.195;Float;False;372;SpecularLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;411;-3204.942,410.1258;Float;False;676.5906;228.4241;0 - back face, 1 - front face, 2 - dont cull antything;1;412;Cull Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;374;387.4229,-1664.526;Float;False;373;AmbientLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;337;-3198.943,24.12969;Float;False;1016.674;301.7413;Convert to linear space;5;383;340;357;283;339;Convert roughness texture to linear space;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;381.5399,-1532.339;Float;False;403;RimLight;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;283;-2638.947,74.12969;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;432;-2266.465,772.0706;Float;False;HalfVector;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;428;-2919.278,1028.708;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;339;-2850.396,80.9836;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;431;-3154.964,1147.886;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;340;-3147.291,185.0124;Float;False;2;0;FLOAT;1;False;1;FLOAT;2.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;429;-3154.964,1019.886;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;425;-2785.172,772.0414;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;426;-2532.074,774.3383;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;333;771.2284,-2051.318;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;460;-3193.102,1420.038;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;383;-2415.698,69.02761;Float;False;RoughnessLS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;430;-3154.964,763.8861;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;427;-2794.845,1020.017;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;391;645.4392,-2562.795;Float;True;Property;_EmissionMap;Emissive;1;0;Create;False;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;515;-1233.895,-1325.326;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;357;-3145.507,81.59152;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;412;-3085.409,500.8773;Float;False;Property;_CullMode;CullMode;22;0;Create;True;0;0;True;0;2;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1414.163,-2569.095;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;CelShading/sh_amp_base_cel_shading_hair;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;412;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;412;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;472;0;461;0
WireConnection;277;0;273;1
WireConnection;277;1;272;0
WireConnection;462;0;472;0
WireConnection;462;1;463;0
WireConnection;290;5;279;0
WireConnection;280;0;277;0
WireConnection;285;0;273;1
WireConnection;285;1;280;0
WireConnection;285;2;273;2
WireConnection;464;0;462;0
WireConnection;349;0;290;0
WireConnection;358;0;285;0
WireConnection;466;0;464;0
WireConnection;300;0;385;0
WireConnection;495;1;493;0
WireConnection;473;4;475;0
WireConnection;473;5;476;0
WireConnection;473;6;474;0
WireConnection;389;0;291;0
WireConnection;389;1;390;0
WireConnection;485;1;479;0
WireConnection;496;0;497;0
WireConnection;496;1;498;0
WireConnection;496;2;274;1
WireConnection;308;0;359;0
WireConnection;308;1;300;0
WireConnection;316;0;308;0
WireConnection;505;1;389;0
WireConnection;505;0;294;0
WireConnection;508;0;496;0
WireConnection;490;1;473;0
WireConnection;490;2;482;0
WireConnection;490;3;483;0
WireConnection;490;4;495;0
WireConnection;490;5;494;0
WireConnection;477;1;473;0
WireConnection;477;2;482;0
WireConnection;477;3;483;0
WireConnection;477;4;485;0
WireConnection;477;5;480;0
WireConnection;408;0;316;0
WireConnection;408;1;407;0
WireConnection;352;0;508;0
WireConnection;492;0;477;0
WireConnection;492;1;490;0
WireConnection;296;0;505;0
WireConnection;296;1;299;0
WireConnection;489;0;492;0
WireConnection;489;1;488;0
WireConnection;376;0;296;0
WireConnection;499;0;500;0
WireConnection;499;1;501;0
WireConnection;499;2;274;2
WireConnection;341;1;408;0
WireConnection;327;0;314;0
WireConnection;327;1;489;0
WireConnection;327;2;338;0
WireConnection;354;0;274;3
WireConnection;516;1;408;0
WireConnection;315;0;311;1
WireConnection;315;2;381;0
WireConnection;506;0;341;0
WireConnection;506;2;507;0
WireConnection;514;0;273;2
WireConnection;509;0;499;0
WireConnection;325;0;506;0
WireConnection;325;1;315;0
WireConnection;518;0;516;0
WireConnection;518;2;507;0
WireConnection;335;0;327;0
WireConnection;335;1;377;0
WireConnection;353;0;509;0
WireConnection;519;0;325;0
WireConnection;519;1;518;0
WireConnection;519;2;517;0
WireConnection;310;0;376;0
WireConnection;310;1;378;0
WireConnection;334;0;327;0
WireConnection;334;1;335;0
WireConnection;334;2;382;0
WireConnection;520;0;380;0
WireConnection;366;0;334;0
WireConnection;355;0;310;0
WireConnection;513;0;519;0
WireConnection;513;1;315;0
WireConnection;513;2;407;0
WireConnection;511;0;519;0
WireConnection;511;1;513;0
WireConnection;511;2;510;0
WireConnection;326;0;320;0
WireConnection;326;2;379;0
WireConnection;323;0;368;0
WireConnection;322;0;364;0
WireConnection;322;1;520;0
WireConnection;399;0;394;0
WireConnection;399;2;401;0
WireConnection;330;0;323;0
WireConnection;330;1;375;0
WireConnection;319;0;355;0
WireConnection;319;1;363;0
WireConnection;332;0;326;0
WireConnection;332;1;367;0
WireConnection;356;0;511;0
WireConnection;405;1;399;0
WireConnection;405;2;404;0
WireConnection;328;0;365;0
WireConnection;328;1;322;0
WireConnection;328;2;326;0
WireConnection;324;0;355;0
WireConnection;324;1;319;0
WireConnection;373;0;330;0
WireConnection;369;0;328;0
WireConnection;403;0;405;0
WireConnection;372;0;332;0
WireConnection;329;0;324;0
WireConnection;329;1;362;0
WireConnection;283;0;339;0
WireConnection;432;0;426;0
WireConnection;428;0;429;0
WireConnection;428;1;431;0
WireConnection;339;0;357;0
WireConnection;339;1;340;0
WireConnection;425;0;430;0
WireConnection;426;0;425;0
WireConnection;426;1;427;0
WireConnection;333;0;329;0
WireConnection;333;1;370;0
WireConnection;333;2;371;0
WireConnection;333;3;374;0
WireConnection;333;4;406;0
WireConnection;383;0;283;0
WireConnection;427;0;428;0
WireConnection;0;2;391;0
WireConnection;0;13;333;0
ASEEND*/
//CHKSM=56012AEAF0B432A3E0A0417537EEEB8576670043