// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CelShading/sh_amp_base_cel_shading_opaque"
{
	Properties
	{
		_MainTex("BaseColor", 2D) = "white" {}
		_EmissionMap("Emissive", 2D) = "black" {}
		_SpecGlossMap("MRAO", 2D) = "white" {}
		_BumpMap("Normal", 2D) = "bump" {}
		_CelRampTexture("CelRampTexture", 2D) = "white" {}
		_SpecularBrightness("SpecularBrightness", Range( 0 , 2)) = 1
		_Tint("Tint", Color) = (1,1,1,0)
		_Contrast("Contrast", Range( 0.1 , 2)) = 1
		_Brightness("Brightness", Range( 0 , 2)) = 1
		_NormalIntensity("NormalIntensity", Range( 0 , 2)) = 1
		_ShadowBrightness("ShadowBrightness", Range( 0 , 1)) = 0
		_Rim("Rim", Range( 0 , 1)) = 0.125
		_MetallicMin("MetallicMin", Range( 0 , 2)) = 0
		_MetallicMax("MetallicMax", Range( 0 , 2)) = 1
		_RoughnessMin("RoughnessMin", Range( 0 , 2)) = 0
		_RoughnessMax("RoughnessMax", Range( 0 , 2)) = 1
		_CullMode("CullMode", Range( 0 , 2)) = 2
		_PointLightCelRamp("PointLightCelRamp", 2D) = "white" {}
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
		uniform float _SpecularBrightness;
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
			float lerpResult415 = lerp( _MetallicMin , _MetallicMax , tex2DNode274.r);
			float Metallic352 = saturate( lerpResult415 );
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult280 = normalize( ( _WorldSpaceLightPos0.xyz - ase_worldPos ) );
			float3 lerpResult285 = lerp( _WorldSpaceLightPos0.xyz , normalizeResult280 , _WorldSpaceLightPos0.w);
			float3 LightDirection358 = lerpResult285;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 NormalTextureTS349 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), _NormalIntensity );
			float dotResult308 = dot( LightDirection358 , normalize( (WorldNormalVector( i , NormalTextureTS349 )) ) );
			float temp_output_408_0 = ( saturate( dotResult308 ) * ase_lightAtten );
			float2 temp_cast_1 = (temp_output_408_0).xx;
			float4 lerpResult420 = lerp( tex2D( _CelRampTexture, temp_cast_1 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			float2 temp_cast_2 = (temp_output_408_0).xx;
			float4 lerpResult433 = lerp( tex2D( _PointLightCelRamp, temp_cast_2 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			float LightType425 = _WorldSpaceLightPos0.w;
			float4 lerpResult430 = lerp( lerpResult420 , lerpResult433 , LightType425);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 lerpResult315 = lerp( ase_lightColor.rgb , float3( 1,1,1 ) , Metallic352);
			float4 lerpResult424 = lerp( ( lerpResult430 * float4( lerpResult315 , 0.0 ) ) , ( lerpResult430 * ase_lightAtten * float4( lerpResult315 , 0.0 ) ) , LightType425);
			float4 Lighting356 = lerpResult424;
			float3 indirectNormal322 = WorldNormalVector( i , NormalTextureTS349 );
			float lerpResult418 = lerp( _RoughnessMin , _RoughnessMax , tex2DNode274.g);
			float Roughness353 = saturate( lerpResult418 );
			Unity_GlossyEnvironmentData g322 = UnityGlossyEnvironmentSetup( ( 1.0 - Roughness353 ), data.worldViewDir, indirectNormal322, float3(0,0,0));
			float3 indirectSpecular322 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal322, g322 );
			float lerpResult326 = lerp( _SpecularBrightness , 1.0 , Metallic352);
			float4 Reflections369 = ( DiffuseWithAO355 * float4( indirectSpecular322 , 0.0 ) * lerpResult326 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult292 = normalize( ( LightDirection358 + ase_worldViewDir ) );
			float dotResult298 = dot( (WorldNormalVector( i , NormalTextureTS349 )) , normalizeResult292 );
			float RoughnessLS383 = ( 1.0 - pow( Roughness353 , ( 1.0 / 2.2 ) ) );
			float temp_output_297_0 = pow( 2.0 , ( ( 7.0 * RoughnessLS383 ) + 1.0 ) );
			float temp_output_317_0 = ( pow( saturate( dotResult298 ) , temp_output_297_0 ) * ( ( 0.0397808 * temp_output_297_0 ) + 0.0823107 ) );
			float3 temp_output_327_0 = ( ase_lightColor.rgb * temp_output_317_0 * ase_lightAtten );
			float4 lerpResult334 = lerp( float4( temp_output_327_0 , 0.0 ) , ( float4( temp_output_327_0 , 0.0 ) * Diffuse376 ) , Metallic352);
			float4 Specular366 = lerpResult334;
			float4 SpecularLight372 = ( lerpResult326 * Specular366 );
			UnityGI gi323 = gi;
			float3 diffNorm323 = WorldNormalVector( i , NormalTextureTS349 );
			gi323 = UnityGI_Base( data, 1, diffNorm323 );
			float3 indirectDiffuse323 = gi323.indirect.diffuse + diffNorm323 * 0.0001;
			float4 AmbientLight373 = ( float4( indirectDiffuse323 , 0.0 ) * DiffuseWithAO355 );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
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
-1915;7;1896;504;5251.362;2133.616;3.926803;False;True
Node;AmplifyShaderEditor.CommentaryNode;271;-3419.47,-1423.869;Float;False;979.6082;655.3135;MRAO;12;354;352;353;274;413;414;415;416;418;417;422;423;MRAO;0.699,1,0.9582815,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;417;-3392.805,-1056.433;Float;False;Property;_RoughnessMax;RoughnessMax;15;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;274;-3392.472,-969.0118;Float;True;Property;_SpecGlossMap;MRAO;2;0;Create;False;0;0;False;0;None;d251482e542e80347b5007c45bac2eff;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;416;-3389.276,-1156.453;Float;False;Property;_RoughnessMin;RoughnessMin;14;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;418;-3007.316,-1150.807;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;423;-2852.823,-1154.196;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;386;-3399.671,-321.8924;Float;False;1319.308;360.7815;Light Direction (All lights);7;273;272;277;280;285;358;425;Light Direction (All lights);1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;273;-3349.671,-268.111;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;272;-3349.671,-123.5215;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;337;-3392.84,119.6097;Float;False;1016.674;301.7413;Convert to linear space;5;383;340;357;283;339;Convert roughness texture to linear space;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2715.297,-1154.458;Float;False;Roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;357;-3339.404,177.0715;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;340;-3341.188,280.4924;Float;False;2;0;FLOAT;1;False;1;FLOAT;2.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;277;-2961.147,-120.5052;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;339;-3044.294,176.4636;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;276;-3411.852,-706.1227;Float;False;1087.245;281.9016;Normal;3;349;290;279;Normal;0,0.681592,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;280;-2769.147,-120.5052;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;285;-2577.891,-247.5956;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;283;-2832.844,169.6097;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-3374.658,-643.3167;Float;False;Property;_NormalIntensity;NormalIntensity;9;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;383;-2609.595,164.5076;Float;False;RoughnessLS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;-2323.364,-250.6729;Float;False;LightDirection;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;275;-1276.678,337.2174;Float;False;3153.768;758.9798;Specular light;25;334;382;335;327;377;338;342;317;307;304;297;298;336;292;293;295;287;361;289;384;360;282;286;387;366;Specular light;0.9961122,1,0.6367924,1;0;0
Node;AmplifyShaderEditor.SamplerNode;290;-2893.219,-643.7323;Float;True;Property;_BumpMap;Normal;3;0;Create;False;0;0;False;0;None;0381f296e844b2a4ea427cd25d16f381;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;286;-844.1134,851.8311;Float;False;Constant;_SpecularSize;SpecularSize;8;0;Create;True;0;0;False;0;7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;384;-845.5482,982.778;Float;False;383;RoughnessLS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;360;-1224.365,721.955;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;349;-2573.251,-639.64;Float;False;NormalTextureTS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;282;-1223.839,846.3983;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;269;-1281.705,-454.2627;Float;False;2401.988;722.4208;Lighting;23;424;428;356;325;315;430;420;431;381;311;341;421;408;316;407;308;359;300;385;432;433;438;439;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;287;-970.7871,720.2734;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-591.0246,855.6447;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;361;-1223.688,466.7522;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;288;-3629.822,-2094.782;Float;False;371;280;Base Color;1;291;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;292;-846.3546,711.5829;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;291;-3579.822,-2044.781;Float;True;Property;_MainTex;BaseColor;0;0;Create;False;0;0;False;0;None;1426d56546a18164986fea97dbbb3381;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;385;-1256.301,-201.3736;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;390;-3579.595,-1790.897;Float;False;Property;_Tint;Tint;6;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;295;-450.2976,853.2672;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;293;-836.6814,463.6072;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;336;-127.6414,792.6353;Float;False;328.4869;201.4724;Brighter Specular;2;305;302;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;297;-326.7256,854.8604;Float;False;2;0;FLOAT;2;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;298;-583.5837,465.9041;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-3197.865,-1790.781;Float;False;Property;_Contrast;Contrast;7;0;Create;True;0;0;False;0;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;-3070.154,-2045.619;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;-1249.838,-330.7926;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;300;-986.1351,-197.8672;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;308;-772.0815,-330.3071;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;304;-331.5329,598.8089;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-2813.052,-1789.557;Float;False;Property;_Brightness;Brightness;8;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;414;-3384.864,-1381.353;Float;False;Property;_MetallicMin;MetallicMin;12;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-77.64104,850.872;Float;False;2;2;0;FLOAT;0.0397808;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;413;-3387.136,-1284.197;Float;False;Property;_MetallicMax;MetallicMax;13;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;419;-2815.277,-2048.598;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;387;381.205,377.8886;Float;False;250;206;Hard Edge;1;314;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;415;-3001.827,-1382.832;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;307;-71.58379,599.3608;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;407;-987.4633,-26.19875;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;305;56.65502,849.3869;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0823107;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;316;-630.9806,-384.134;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-2556.253,-2046.457;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;338;397.2532,826.6021;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;422;-2857.718,-1382.692;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2304.748,-2055.687;Float;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;408;-444.0151,-386.6781;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;317;177.9223,599.5396;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;314;431.2881,411.6724;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;354;-2722.259,-889.7251;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;432;-344.8018,-99.13364;Float;True;Property;_PointLightCelRamp;PointLightCelRamp;17;0;Create;True;0;0;False;0;6daf97bc07c8414469b76460d2739c18;6daf97bc07c8414469b76460d2739c18;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;421;-286.6653,-240.563;Float;False;Property;_ShadowBrightness;ShadowBrightness;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;425;-2335.79,-95.31273;Float;False;LightType;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;327;819.5668,591.0507;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;341;-290.4676,-437.1338;Float;True;Property;_CelRampTexture;CelRampTexture;4;0;Create;False;0;0;False;0;b475a03964fa64642b09599531c2e016;e5534d2b3129db046b46197e6b222bd6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;377;816.9035,728.4226;Float;False;376;Diffuse;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;352;-2722.265,-1383.237;Float;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;433;67.73472,-274.2252;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;438;-615.7595,157.9308;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;303;-1947.294,-2100.134;Float;False;219;183;Applying AO;1;310;Applying AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;311;6.356878,-64.58514;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;431;39.41863,-155.3195;Float;False;425;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;378;-2304.012,-1918.321;Float;False;354;AmbientOcclusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;14.07701,90.2505;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;312;-1365.226,-1620.016;Float;False;1251.337;382.5177;Reflections;8;380;429;369;328;322;365;364;434;Reflections;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;1201.902,724.1425;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;382;1201.598,869.9569;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;309;-1277.541,-1193.413;Float;False;1172.302;309.561;Specular;6;379;372;367;326;332;435;Specular Brightness;0.9076021,1,0.6556604,1;0;0
Node;AmplifyShaderEditor.LerpOp;420;64.12621,-399.0583;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;-1918.93,-2047.986;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;334;1434.707,589.3342;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;313;-1286.069,-806.2059;Float;False;1151.336;281.2077;Ambient Light;5;373;330;375;368;323;Ambient Light;0.5704813,0.3537736,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;435;-1504.439,-1161.605;Float;False;586;122;default value 0.04;1;320;spec brightness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;428;513.0306,-142.2885;Float;False;267;292.1541;Square Light Fix;2;426;427;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;439;278.619,161.2251;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;380;-1318.84,-1411.657;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;430;275.7674,-405.8885;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;402;-1266.577,1231.471;Float;False;1465.171;439.3994;RimLighting;7;405;403;404;399;401;394;393;RimLighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;315;305.7849,-73.3353;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;426;563.0307,34.8656;Float;False;425;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;434;-1112.864,-1405.083;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;355;-1532.359,-2045.797;Float;False;DiffuseWithAO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;364;-1221.852,-1551.782;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;401;-1216.577,1543.504;Float;False;Property;_Rim;Rim;11;0;Create;True;0;0;False;0;0.125;0.125;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;366;1597.195,605.8813;Float;False;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-1208.541,-1126.413;Float;False;Property;_SpecularBrightness;SpecularBrightness;5;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;379;-1208.124,-998.3226;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;394;-959.0376,1288.363;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;514.8817,-391.1415;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;-1229.674,-731.6213;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;306;-1292.299,-2135.167;Float;False;771.9758;331.8977;Metallic Diffuse;3;324;319;363;Metallic Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;427;566.8821,-92.28851;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;-820.2244,-989.5296;Float;False;366;Specular;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;375;-1232.643,-605.5579;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;365;-951.1255,-1551.236;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectSpecularLight;322;-934.2877,-1457.492;Float;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;-1225.917,-1934.085;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;326;-820.4957,-1126.122;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;323;-969.9795,-732.2825;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;399;-702.337,1283.376;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;404;-702.5053,1537.468;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;424;692.6802,-402.4674;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;328;-573.0942,-1555.015;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;356;893.1022,-389.0227;Float;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-710.3452,-734.0469;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;405;-319.4314,1279.426;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-972.9394,-1951.116;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;321;-377.8059,-2138.762;Float;False;406;334;Applying Lighting;2;362;329;Applying Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;-556.2476,-1123.436;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;-350.292,-1971.815;Float;False;356;Lighting;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;403;-125.7527,1279.151;Float;False;RimLight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;324;-765.0978,-2046.306;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;372;-374.6343,-1127.816;Float;False;SpecularLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-459.4996,-729.7736;Float;False;AmbientLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;331;295.5503,-2101.319;Float;False;724.2165;700.7795;Applying Reflections, Specular, AO;5;406;374;371;370;333;Applying Reflections, Specular, AO, Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;369;-361.2097,-1553.392;Float;False;Reflections;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;411;-3395.911,488.034;Float;False;676.5906;228.4241;0 - back face, 1 - front face, 2 - dont cull antything;1;412;Cull Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;383.4047,-1790.195;Float;False;372;SpecularLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;370;389.2529,-1916.877;Float;False;369;Reflections;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;392;595.4392,-2612.795;Float;False;370.9999;280;Emissive;1;391;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-167.6868,-2058.974;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;381.5399,-1532.339;Float;False;403;RimLight;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;374;387.4229,-1664.526;Float;False;373;AmbientLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;342;424.8317,531.1313;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;391;645.4392,-2562.795;Float;True;Property;_EmissionMap;Emissive;1;0;Create;False;0;0;False;0;None;b6e2d8e3909fbd8469471938b8854af9;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;429;-1226.034,-1315.109;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;333;771.2284,-2051.318;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;393;-1215.672,1281.471;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;412;-3276.378,578.7854;Float;False;Property;_CullMode;CullMode;16;0;Create;True;0;0;True;0;2;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1414.163,-2569.095;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;CelShading/sh_amp_base_cel_shading_opaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;412;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;412;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;418;0;416;0
WireConnection;418;1;417;0
WireConnection;418;2;274;2
WireConnection;423;0;418;0
WireConnection;353;0;423;0
WireConnection;277;0;273;1
WireConnection;277;1;272;0
WireConnection;339;0;357;0
WireConnection;339;1;340;0
WireConnection;280;0;277;0
WireConnection;285;0;273;1
WireConnection;285;1;280;0
WireConnection;285;2;273;2
WireConnection;283;0;339;0
WireConnection;383;0;283;0
WireConnection;358;0;285;0
WireConnection;290;5;279;0
WireConnection;349;0;290;0
WireConnection;287;0;360;0
WireConnection;287;1;282;0
WireConnection;289;0;286;0
WireConnection;289;1;384;0
WireConnection;292;0;287;0
WireConnection;295;0;289;0
WireConnection;293;0;361;0
WireConnection;297;1;295;0
WireConnection;298;0;293;0
WireConnection;298;1;292;0
WireConnection;389;0;291;0
WireConnection;389;1;390;0
WireConnection;300;0;385;0
WireConnection;308;0;359;0
WireConnection;308;1;300;0
WireConnection;304;0;298;0
WireConnection;302;1;297;0
WireConnection;419;1;389;0
WireConnection;419;0;294;0
WireConnection;415;0;414;0
WireConnection;415;1;413;0
WireConnection;415;2;274;1
WireConnection;307;0;304;0
WireConnection;307;1;297;0
WireConnection;305;0;302;0
WireConnection;316;0;308;0
WireConnection;296;0;419;0
WireConnection;296;1;299;0
WireConnection;422;0;415;0
WireConnection;376;0;296;0
WireConnection;408;0;316;0
WireConnection;408;1;407;0
WireConnection;317;0;307;0
WireConnection;317;1;305;0
WireConnection;354;0;274;3
WireConnection;432;1;408;0
WireConnection;425;0;273;2
WireConnection;327;0;314;1
WireConnection;327;1;317;0
WireConnection;327;2;338;0
WireConnection;341;1;408;0
WireConnection;352;0;422;0
WireConnection;433;0;432;0
WireConnection;433;2;421;0
WireConnection;438;0;407;0
WireConnection;335;0;327;0
WireConnection;335;1;377;0
WireConnection;420;0;341;0
WireConnection;420;2;421;0
WireConnection;310;0;376;0
WireConnection;310;1;378;0
WireConnection;334;0;327;0
WireConnection;334;1;335;0
WireConnection;334;2;382;0
WireConnection;439;0;438;0
WireConnection;430;0;420;0
WireConnection;430;1;433;0
WireConnection;430;2;431;0
WireConnection;315;0;311;1
WireConnection;315;2;381;0
WireConnection;434;0;380;0
WireConnection;355;0;310;0
WireConnection;366;0;334;0
WireConnection;325;0;430;0
WireConnection;325;1;315;0
WireConnection;427;0;430;0
WireConnection;427;1;439;0
WireConnection;427;2;315;0
WireConnection;322;0;364;0
WireConnection;322;1;434;0
WireConnection;326;0;320;0
WireConnection;326;2;379;0
WireConnection;323;0;368;0
WireConnection;399;0;394;0
WireConnection;399;2;401;0
WireConnection;424;0;325;0
WireConnection;424;1;427;0
WireConnection;424;2;426;0
WireConnection;328;0;365;0
WireConnection;328;1;322;0
WireConnection;328;2;326;0
WireConnection;356;0;424;0
WireConnection;330;0;323;0
WireConnection;330;1;375;0
WireConnection;405;1;399;0
WireConnection;405;2;404;0
WireConnection;319;0;355;0
WireConnection;319;1;363;0
WireConnection;332;0;326;0
WireConnection;332;1;367;0
WireConnection;403;0;405;0
WireConnection;324;0;355;0
WireConnection;324;1;319;0
WireConnection;372;0;332;0
WireConnection;373;0;330;0
WireConnection;369;0;328;0
WireConnection;329;0;324;0
WireConnection;329;1;362;0
WireConnection;342;0;317;0
WireConnection;333;0;329;0
WireConnection;333;1;370;0
WireConnection;333;2;371;0
WireConnection;333;3;374;0
WireConnection;333;4;406;0
WireConnection;0;2;391;0
WireConnection;0;13;333;0
ASEEND*/
//CHKSM=07A9817DF94DD3E71CE03E3CEC1D09B550EB4DBF