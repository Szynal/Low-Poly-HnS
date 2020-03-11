// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CelShading/sh_amp_base_cel_shading_masked"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EmissionMap("Emissive", 2D) = "black" {}
		_MainTex("BaseColor", 2D) = "white" {}
		_SpecGlossMap("MRAO", 2D) = "white" {}
		_BumpMap("Normal", 2D) = "bump" {}
		_CelRampTexture("CelRampTexture", 2D) = "white" {}
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
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
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
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Contrast;
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
		uniform float _Rim;
		uniform float _Cutoff = 0.5;


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
			float4 tex2DNode291 = tex2D( _MainTex, uv_MainTex );
			float4 Diffuse376 = ( CalculateContrast(_Contrast,( tex2DNode291 * _Tint )) * _Brightness );
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
			float3 tex2DNode290 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), _NormalIntensity );
			float3 NormalTextureTS349 = tex2DNode290;
			float dotResult308 = dot( LightDirection358 , normalize( (WorldNormalVector( i , NormalTextureTS349 )) ) );
			float temp_output_408_0 = ( saturate( dotResult308 ) * ase_lightAtten );
			float2 temp_cast_1 = (temp_output_408_0).xx;
			float4 lerpResult420 = lerp( tex2D( _CelRampTexture, temp_cast_1 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			float2 temp_cast_2 = (temp_output_408_0).xx;
			float4 lerpResult432 = lerp( tex2D( _PointLightCelRamp, temp_cast_2 ) , float4( 1,1,1,0 ) , _ShadowBrightness);
			float LightType428 = _WorldSpaceLightPos0.w;
			float4 lerpResult433 = lerp( lerpResult420 , lerpResult432 , LightType428);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 lerpResult315 = lerp( ase_lightColor.rgb , float3( 1,1,1 ) , Metallic352);
			float4 lerpResult425 = lerp( ( lerpResult433 * float4( lerpResult315 , 0.0 ) ) , ( lerpResult433 * ase_lightAtten * float4( lerpResult315 , 0.0 ) ) , LightType428);
			float4 Lighting356 = lerpResult425;
			float3 indirectNormal322 = WorldNormalVector( i , NormalTextureTS349 );
			float lerpResult418 = lerp( _RoughnessMin , _RoughnessMax , tex2DNode274.g);
			float Roughness353 = saturate( lerpResult418 );
			Unity_GlossyEnvironmentData g322 = UnityGlossyEnvironmentSetup( ( 1.0 - Roughness353 ), data.worldViewDir, indirectNormal322, float3(0,0,0));
			float3 indirectSpecular322 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal322, g322 );
			float lerpResult326 = lerp( 1.0 , 1.0 , Metallic352);
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
			clip( tex2DNode291.a - _Cutoff );
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
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
-1904;14;1896;986;5221.119;4152.451;3.948646;True;False
Node;AmplifyShaderEditor.CommentaryNode;271;-3326.845,-1489.675;Float;False;1085.68;696.1782;MRAO;12;352;354;353;274;413;414;415;416;417;418;422;423;MRAO;0.699,1,0.9582815,1;0;0
Node;AmplifyShaderEditor.SamplerNode;274;-3285.646,-1008.369;Float;True;Property;_SpecGlossMap;MRAO;3;0;Create;False;0;0;False;0;None;d98a8a7474093fe44bf206ae5891c7e3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;416;-3274.852,-1198.384;Float;False;Property;_RoughnessMin;RoughnessMin;14;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;417;-3278.381,-1098.364;Float;False;Property;_RoughnessMax;RoughnessMax;15;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;418;-2878.833,-1152.12;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;386;-3316.178,-324.4014;Float;False;1319.308;360.7815;Light Direction (All lights);7;273;272;277;280;285;358;428;Light Direction (All lights);1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;422;-2716.161,-1149.84;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;272;-3266.178,-126.0304;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2558.936,-1152.712;Float;False;Roughness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;337;-3309.348,117.1009;Float;False;1016.674;301.7413;Convert to linear space;5;383;340;357;283;339;Convert roughness texture to linear space;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;273;-3266.178,-270.6198;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;357;-3255.912,174.5626;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;340;-3257.696,277.9835;Float;False;2;0;FLOAT;1;False;1;FLOAT;2.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;277;-2877.655,-123.0141;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;280;-2685.655,-123.0141;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;276;-3328.36,-708.6316;Float;False;1087.245;281.9016;Normal;4;349;290;279;435;Normal;0,0.681592,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;339;-2960.802,173.9547;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-3291.166,-645.8256;Float;False;Property;_NormalIntensity;NormalIntensity;9;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;283;-2749.352,167.1008;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;285;-2494.399,-250.1044;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;290;-3002.744,-647.7375;Float;True;Property;_BumpMap;Normal;4;0;Create;False;0;0;False;0;None;3930bde0688129c49bc68fdf30b75f1f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;275;-1286.205,342.8051;Float;False;3153.768;758.9798;Specular light;25;366;334;382;335;327;377;338;342;317;307;304;297;298;336;292;293;295;287;361;289;384;360;282;286;387;Specular light;0.9961122,1,0.6367924,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;358;-2239.872,-253.1817;Float;False;LightDirection;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;383;-2526.103,161.9987;Float;False;RoughnessLS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;360;-1233.892,727.5427;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;269;-1277.237,-379.8776;Float;False;2681.108;706.6115;Lighting;25;311;439;325;381;431;421;437;432;356;425;426;433;315;420;430;341;408;316;407;308;300;359;385;442;443;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;286;-853.6403,857.4188;Float;False;Constant;_SpecularSize;SpecularSize;8;0;Create;True;0;0;False;0;7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;384;-855.0751,988.3657;Float;False;383;RoughnessLS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;349;-2489.759,-644.4476;Float;False;NormalTextureTS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;282;-1233.366,851.986;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;385;-1251.833,-126.9885;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;361;-1233.215,472.3399;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-600.5515,861.2324;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;288;-3629.822,-2094.782;Float;False;371;280;Base Color;1;291;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;287;-980.314,725.8611;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;295;-459.8246,858.8549;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;291;-3579.822,-2044.781;Float;True;Property;_MainTex;BaseColor;2;0;Create;False;0;0;False;0;None;0f01da4acf59ca44e91bc189d8835e1a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;293;-846.2083,469.1949;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;359;-1245.37,-256.4075;Float;False;358;LightDirection;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;390;-3579.595,-1790.897;Float;False;Property;_Tint;Tint;6;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;300;-981.6669,-123.4821;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;292;-855.8815,717.1707;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;298;-593.1106,471.4918;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;407;-986.2292,125.8057;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;308;-767.6132,-255.922;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;336;-137.1683,798.223;Float;False;328.4869;201.4724;Brighter Specular;2;305;302;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-3197.865,-1790.781;Float;False;Property;_Contrast;Contrast;7;0;Create;True;0;0;False;0;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;297;-336.2525,860.4481;Float;False;2;0;FLOAT;2;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;389;-3070.154,-2045.619;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-2813.052,-1789.557;Float;False;Property;_Brightness;Brightness;8;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;316;-637.1469,-258.0952;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;419;-2816.223,-2045.879;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;413;-3272.711,-1326.127;Float;False;Property;_MetallicMax;MetallicMax;13;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;414;-3270.439,-1423.283;Float;False;Property;_MetallicMin;MetallicMin;12;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;304;-341.0598,604.3966;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;439;-669.4395,-134.3111;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-87.16797,856.4597;Float;False;2;2;0;FLOAT;0.0397808;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;307;-81.11072,604.9485;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;408;-433.0691,-255.6138;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;305;47.12809,854.9746;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0823107;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;415;-2880.697,-1399.614;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;387;362.2082,381.623;Float;False;250;206;Hard Edge;1;314;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-2561.895,-2046.277;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;376;-2304.748,-2055.687;Float;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;338;389.0429,817.7078;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;423;-2722.161,-1399.84;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;430;-291.8962,-49.65116;Float;True;Property;_PointLightCelRamp;PointLightCelRamp;17;0;Create;True;0;0;False;0;6daf97bc07c8414469b76460d2739c18;6daf97bc07c8414469b76460d2739c18;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;317;168.3954,605.1273;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;314;421.7611,417.2601;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;352;-2560.066,-1406.157;Float;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;437;23.21558,-73.09979;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;428;-2238.256,-119.5491;Float;False;LightType;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;327;810.0399,596.6384;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;377;807.3766,734.0103;Float;False;376;Diffuse;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;421;-257.6378,-147.3163;Float;False;Property;_ShadowBrightness;ShadowBrightness;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;354;-2561.928,-898.3681;Float;False;AmbientOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;341;-255.0137,-335.4292;Float;True;Property;_CelRampTexture;CelRampTexture;5;0;Create;False;0;0;False;0;b475a03964fa64642b09599531c2e016;e5534d2b3129db046b46197e6b222bd6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;1192.375,729.7302;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;-99.85979,251.517;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;420;82.46994,-253.7229;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;312;-1275.226,-1620.016;Float;False;1160.154;384.9964;Reflections;8;429;369;328;365;322;364;380;436;Reflections;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;311;-107.5884,136.8547;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;378;-2304.012,-1918.321;Float;False;354;AmbientOcclusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;303;-1947.294,-2100.134;Float;False;219;183;Applying AO;1;310;Applying AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;443;-376.1862,261.503;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;431;71.3534,19.72219;Float;False;428;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;432;90.38793,-105.6889;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;382;1192.071,875.5446;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;380;-1221.84,-1420.657;Float;False;353;Roughness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;309;-1277.541,-1193.413;Float;False;1172.302;309.561;Specular;6;320;379;372;367;326;332;Specular Brightness;0.9076021,1,0.6556604,1;0;0
Node;AmplifyShaderEditor.LerpOp;433;285.1755,-257.959;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;313;-1275.598,-769.5593;Float;False;1151.336;281.2077;Ambient Light;5;373;330;375;368;323;Ambient Light;0.5704813,0.3537736,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;334;1451.18,597.9219;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;426;463.4766,-53.85947;Float;False;267;292.1541;Square Light Fix;2;427;424;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;-1918.93,-2047.986;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;402;-1266.577,1231.471;Float;False;1465.171;439.3994;RimLighting;7;405;403;404;399;401;394;393;RimLighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;315;181.5106,106.3844;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;442;333.6138,266.703;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;364;-1219.553,-1551.782;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;394;-959.0376,1288.363;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;320;-1208.541,-1126.413;Float;False;Constant;_SpecularBrightness;SpecularBrightness;5;0;Create;True;0;0;False;0;1;0.04;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;427;517.3279,-3.859471;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;366;1641.565,598.3406;Float;False;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;424;553.7766,138.8947;Float;False;428;LightType;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;379;-1208.124,-998.3226;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;355;-1532.359,-2045.797;Float;False;DiffuseWithAO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;-1219.203,-694.9748;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;401;-1216.577,1543.504;Float;False;Property;_Rim;Rim;11;0;Create;True;0;0;False;0;0.125;0.125;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;436;-999.2515,-1407.283;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;494.9132,-276.3805;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;306;-1292.299,-2135.167;Float;False;771.9758;331.8977;Metallic Diffuse;3;324;319;363;Metallic Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;-1225.917,-1934.085;Float;False;352;Metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;399;-702.337,1283.376;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;404;-702.5053,1537.468;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;326;-820.4957,-1126.122;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;425;630.6108,-255.4549;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;-820.2244,-989.5296;Float;False;366;Specular;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;323;-959.5091,-695.6359;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;365;-951.1255,-1551.236;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectSpecularLight;322;-769.2877,-1426.492;Float;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;375;-1222.172,-568.9113;Float;False;355;DiffuseWithAO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;321;-377.8059,-2138.762;Float;False;406;334;Applying Lighting;2;362;329;Applying Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-972.9394,-1951.116;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;-699.8748,-697.4003;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;332;-556.2476,-1123.436;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;405;-319.4314,1279.426;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;328;-492.3279,-1481.591;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;356;921.6427,-238.3549;Float;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-449.0291,-694.1271;Float;False;AmbientLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;-350.292,-1971.815;Float;False;356;Lighting;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;369;-344.6894,-1487.31;Float;False;Reflections;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;372;-374.6343,-1127.816;Float;False;SpecularLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;324;-765.0978,-2046.306;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;403;-125.7527,1279.151;Float;False;RimLight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;331;295.5503,-2101.319;Float;False;724.2165;700.7795;Applying Reflections, Specular, AO;5;406;374;371;370;333;Applying Reflections, Specular, AO, Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;370;389.2529,-1919.44;Float;False;369;Reflections;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;411;-3312.419,485.5252;Float;False;676.5906;228.4241;0 - back face, 1 - front face, 2 - dont cull antything;1;412;Cull Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;374;387.4229,-1664.526;Float;False;373;AmbientLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;392;595.4392,-2612.795;Float;False;370.9999;280;Emissive;1;391;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;383.4047,-1790.195;Float;False;372;SpecularLight;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;406;381.5399,-1532.339;Float;False;403;RimLight;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-167.6868,-2058.974;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;444;-3047.956,-2230.138;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;342;403.3482,540.559;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;429;-1225.142,-1331.949;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;435;-2628.965,-516.7311;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;391;645.4392,-2562.795;Float;True;Property;_EmissionMap;Emissive;1;0;Create;False;0;0;False;0;None;caea0ea7869d415439ac1414ff39b1eb;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TangentSignVertexDataNode;434;-2953.012,-448.0121;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;393;-1215.672,1281.471;Float;False;349;NormalTextureTS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;333;771.2284,-2051.318;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;412;-3192.885,576.2766;Float;False;Property;_CullMode;CullMode;16;0;Create;True;0;0;True;0;2;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1414.163,-2569.095;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;CelShading/sh_amp_base_cel_shading_masked;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;412;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;True;412;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;445;-2191.257,-2723.415;Float;False;889.8076;244.4055;SHADER QUEUE CHANGED TO TRANSPARENT;0;;1,1,1,1;0;0
WireConnection;418;0;416;0
WireConnection;418;1;417;0
WireConnection;418;2;274;2
WireConnection;422;0;418;0
WireConnection;353;0;422;0
WireConnection;277;0;273;1
WireConnection;277;1;272;0
WireConnection;280;0;277;0
WireConnection;339;0;357;0
WireConnection;339;1;340;0
WireConnection;283;0;339;0
WireConnection;285;0;273;1
WireConnection;285;1;280;0
WireConnection;285;2;273;2
WireConnection;290;5;279;0
WireConnection;358;0;285;0
WireConnection;383;0;283;0
WireConnection;349;0;290;0
WireConnection;289;0;286;0
WireConnection;289;1;384;0
WireConnection;287;0;360;0
WireConnection;287;1;282;0
WireConnection;295;0;289;0
WireConnection;293;0;361;0
WireConnection;300;0;385;0
WireConnection;292;0;287;0
WireConnection;298;0;293;0
WireConnection;298;1;292;0
WireConnection;308;0;359;0
WireConnection;308;1;300;0
WireConnection;297;1;295;0
WireConnection;389;0;291;0
WireConnection;389;1;390;0
WireConnection;316;0;308;0
WireConnection;419;1;389;0
WireConnection;419;0;294;0
WireConnection;304;0;298;0
WireConnection;439;0;407;0
WireConnection;302;1;297;0
WireConnection;307;0;304;0
WireConnection;307;1;297;0
WireConnection;408;0;316;0
WireConnection;408;1;439;0
WireConnection;305;0;302;0
WireConnection;415;0;414;0
WireConnection;415;1;413;0
WireConnection;415;2;274;1
WireConnection;296;0;419;0
WireConnection;296;1;299;0
WireConnection;376;0;296;0
WireConnection;423;0;415;0
WireConnection;430;1;408;0
WireConnection;317;0;307;0
WireConnection;317;1;305;0
WireConnection;352;0;423;0
WireConnection;437;0;430;0
WireConnection;428;0;273;2
WireConnection;327;0;314;1
WireConnection;327;1;317;0
WireConnection;327;2;338;0
WireConnection;354;0;274;3
WireConnection;341;1;408;0
WireConnection;335;0;327;0
WireConnection;335;1;377;0
WireConnection;420;0;341;0
WireConnection;420;2;421;0
WireConnection;443;0;407;0
WireConnection;432;0;437;0
WireConnection;432;2;421;0
WireConnection;433;0;420;0
WireConnection;433;1;432;0
WireConnection;433;2;431;0
WireConnection;334;0;327;0
WireConnection;334;1;335;0
WireConnection;334;2;382;0
WireConnection;310;0;376;0
WireConnection;310;1;378;0
WireConnection;315;0;311;1
WireConnection;315;2;381;0
WireConnection;442;0;443;0
WireConnection;427;0;433;0
WireConnection;427;1;442;0
WireConnection;427;2;315;0
WireConnection;366;0;334;0
WireConnection;355;0;310;0
WireConnection;436;0;380;0
WireConnection;325;0;433;0
WireConnection;325;1;315;0
WireConnection;399;0;394;0
WireConnection;399;2;401;0
WireConnection;326;0;320;0
WireConnection;326;2;379;0
WireConnection;425;0;325;0
WireConnection;425;1;427;0
WireConnection;425;2;424;0
WireConnection;323;0;368;0
WireConnection;322;0;364;0
WireConnection;322;1;436;0
WireConnection;319;0;355;0
WireConnection;319;1;363;0
WireConnection;330;0;323;0
WireConnection;330;1;375;0
WireConnection;332;0;326;0
WireConnection;332;1;367;0
WireConnection;405;1;399;0
WireConnection;405;2;404;0
WireConnection;328;0;365;0
WireConnection;328;1;322;0
WireConnection;328;2;326;0
WireConnection;356;0;425;0
WireConnection;373;0;330;0
WireConnection;369;0;328;0
WireConnection;372;0;332;0
WireConnection;324;0;355;0
WireConnection;324;1;319;0
WireConnection;403;0;405;0
WireConnection;329;0;324;0
WireConnection;329;1;362;0
WireConnection;444;0;291;4
WireConnection;342;0;317;0
WireConnection;435;0;290;0
WireConnection;435;1;434;0
WireConnection;333;0;329;0
WireConnection;333;1;370;0
WireConnection;333;2;371;0
WireConnection;333;3;374;0
WireConnection;333;4;406;0
WireConnection;0;2;391;0
WireConnection;0;10;444;0
WireConnection;0;13;333;0
ASEEND*/
//CHKSM=78B2E065C834B9DCA5F586F46A1D73687E0D9FD0