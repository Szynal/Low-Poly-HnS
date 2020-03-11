// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify/sh_amp_Base"
{
	Properties
	{
		_MainTex("BaseColor", 2D) = "gray" {}
		_Tint("Tint", Color) = (1,1,1,0)
		_SpecGlossMap("MRAO", 2D) = "black" {}
		[Normal]_BumpMap("NormalMap", 2D) = "bump" {}
		_NormalStrength("NormalStrength", Float) = 1
		[Toggle(_NORMALDETAIL_ON)] _NormalDetail("NormalDetail", Float) = 0
		_NormalDetailScale("NormalDetailScale", Float) = 1
		[Normal]_DetailNormalMap("NormalDetailMap", 2D) = "bump" {}
		_NormalDetailStrength("NormalDetailStrength", Float) = 1
		_MetallicExp("MetallicExp", Float) = 1
		_RoughnessExp("RoughnessExp", Float) = 1
		[Toggle(_USEEMISSIVE_ON)] _UseEmissive("UseEmissive", Float) = 0
		_EmissionMap("EmissionMap", 2D) = "black" {}
		_EmissiveTint("EmissiveTint", Color) = (1,1,1,0)
		_CullingType("CullingType", Int) = 2
		_EmissiveStrength("EmissiveStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_CullingType]
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 5.0
		#pragma multi_compile __ _NORMALDETAIL_ON
		#pragma shader_feature _USEEMISSIVE_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform int _CullingType;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _NormalStrength;
		uniform sampler2D _DetailNormalMap;
		uniform float _NormalDetailScale;
		uniform float _NormalDetailStrength;
		uniform float4 _Tint;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform float4 _EmissiveTint;
		uniform float _EmissiveStrength;
		uniform sampler2D _SpecGlossMap;
		uniform float4 _SpecGlossMap_ST;
		uniform float _MetallicExp;
		uniform float _RoughnessExp;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 tex2DNode33 = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float2 appendResult116 = (float2(tex2DNode33.r , tex2DNode33.g));
			float3 appendResult118 = (float3(( appendResult116 * _NormalStrength ) , tex2DNode33.b));
			float3 tex2DNode34 = UnpackNormal( tex2D( _DetailNormalMap, ( i.uv_texcoord * 5.0 * _NormalDetailScale ) ) );
			float2 appendResult121 = (float2(tex2DNode34.r , tex2DNode34.g));
			float3 appendResult120 = (float3(( appendResult121 * _NormalDetailStrength ) , tex2DNode34.b));
			#ifdef _NORMALDETAIL_ON
				float3 staticSwitch108 = BlendNormals( appendResult118 , appendResult120 );
			#else
				float3 staticSwitch108 = appendResult118;
			#endif
			o.Normal = staticSwitch108;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = ( _Tint * tex2D( _MainTex, uv_MainTex ) ).rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			#ifdef _USEEMISSIVE_ON
				float4 staticSwitch146 = ( tex2D( _EmissionMap, uv_EmissionMap ) * _EmissiveTint * _EmissiveStrength );
			#else
				float4 staticSwitch146 = temp_cast_1;
			#endif
			o.Emission = staticSwitch146.rgb;
			float2 uv_SpecGlossMap = i.uv_texcoord * _SpecGlossMap_ST.xy + _SpecGlossMap_ST.zw;
			float4 tex2DNode35 = tex2D( _SpecGlossMap, uv_SpecGlossMap );
			float clampResult115 = clamp( pow( tex2DNode35.r , _MetallicExp ) , 0.0 , 1.0 );
			o.Metallic = clampResult115;
			float clampResult111 = clamp( pow( tex2DNode35.g , _RoughnessExp ) , 0.0 , 1.0 );
			o.Smoothness = ( 1.0 - clampResult111 );
			o.Occlusion = tex2DNode35.b;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16300
276;4;1265;1002;2115.657;1562.806;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;39;-2039.489,-436.5537;Float;False;Property;_NormalDetailScale;NormalDetailScale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-2040.489,-532.5535;Float;False;Constant;_Float4;Float 4;7;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-2048,-672;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1782.326,-659.0496;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;34;-1400.487,-660.5535;Float;True;Property;_DetailNormalMap;NormalDetailMap;7;1;[Normal];Create;False;0;0;False;0;8178c5ce4aa3d5341804ce7d0ff18428;8178c5ce4aa3d5341804ce7d0ff18428;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-1400.487,-916.5535;Float;True;Property;_BumpMap;NormalMap;3;1;[Normal];Create;False;0;0;False;0;8178c5ce4aa3d5341804ce7d0ff18428;ac0330764dc527b40ba26c429ab95983;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;121;-984.4712,-585.3241;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-998.4717,-1024.124;Float;False;Property;_NormalStrength;NormalStrength;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;116;-1005.271,-890.3239;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-1368.471,-729.3241;Float;False;Property;_NormalDetailStrength;NormalDetailStrength;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;35;-640,-384;Float;True;Property;_SpecGlossMap;MRAO;2;0;Create;False;0;0;False;0;bf46abed9136d94419950c75dd25e10b;38ce1d9c173efab44a684b547cbe6415;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;113;-30.87884,-229.5148;Float;False;Property;_RoughnessExp;RoughnessExp;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-808.4712,-585.3241;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-809.7712,-889.0235;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;118;-616.4712,-863.0237;Float;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;145;-953,513;Float;True;Property;_EmissionMap;EmissionMap;12;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;120;-616.4712,-585.3241;Float;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-948.056,966.9938;Float;False;Property;_EmissiveStrength;EmissiveStrength;15;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;22.63497,-473.4045;Float;False;Property;_MetallicExp;MetallicExp;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;149;-953,769;Float;False;Property;_EmissiveTint;EmissiveTint;13;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;110;188.2161,-314.8398;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;142;-640,-1408;Float;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;114;236.6093,-632.5922;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;36;-248.5787,-867.2704;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;32;-640,-1152;Float;True;Property;_MainTex;BaseColor;0;0;Create;False;0;0;False;0;None;218a2a5ead3b9eb4ea9c52e9156c0667;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;147;-569,385;Float;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-441,513;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;111;444.2164,-314.8398;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;103;957.0539,-580.3367;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;115;492.6099,-632.5922;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-128,-1152;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;108;112.8892,-955.439;Float;False;Property;_NormalDetail;NormalDetail;5;0;Create;True;0;0;False;0;1;0;0;True;;Toggle;2;Key0;Key1;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;146;-185,385;Float;False;Property;_UseEmissive;UseEmissive;11;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;141;-1402.099,-1398.3;Float;False;Property;_CullingType;CullingType;14;0;Create;True;0;0;True;0;2;2;0;1;INT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;144;1282.371,-892.1458;Float;False;True;7;Float;ASEMaterialInspector;0;0;Standard;Amplify/sh_amp_Base;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;141;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;37;0
WireConnection;38;1;40;0
WireConnection;38;2;39;0
WireConnection;34;1;38;0
WireConnection;121;0;34;1
WireConnection;121;1;34;2
WireConnection;116;0;33;1
WireConnection;116;1;33;2
WireConnection;117;0;121;0
WireConnection;117;1;122;0
WireConnection;119;0;116;0
WireConnection;119;1;123;0
WireConnection;118;0;119;0
WireConnection;118;2;33;3
WireConnection;120;0;117;0
WireConnection;120;2;34;3
WireConnection;110;0;35;2
WireConnection;110;1;113;0
WireConnection;114;0;35;1
WireConnection;114;1;112;0
WireConnection;36;0;118;0
WireConnection;36;1;120;0
WireConnection;148;0;145;0
WireConnection;148;1;149;0
WireConnection;148;2;150;0
WireConnection;111;0;110;0
WireConnection;103;0;111;0
WireConnection;115;0;114;0
WireConnection;143;0;142;0
WireConnection;143;1;32;0
WireConnection;108;1;118;0
WireConnection;108;0;36;0
WireConnection;146;1;147;0
WireConnection;146;0;148;0
WireConnection;144;0;143;0
WireConnection;144;1;108;0
WireConnection;144;2;146;0
WireConnection;144;3;115;0
WireConnection;144;4;103;0
WireConnection;144;5;35;3
ASEEND*/
//CHKSM=CAFE81AD8705B857178F11604DE9F585991C1CD0