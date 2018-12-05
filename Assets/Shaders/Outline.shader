// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Outline"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(1.0,2.0)) = 1.02

		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

	_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0

		_MetallicGlossMap("Metallic", 2D) = "white" {}

	_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}



	// Blending state
	[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float3 normal : NORMAL;
	};

	float _OutlineWidth;
	float4 _OutlineColor;

	v2f vert(appdata v)
	{
		v.vertex.xyz *= _OutlineWidth;

		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	ENDCG

		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass // Render the Outline
	{
		ZWrite Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

		half4 frag(v2f i) : COLOR
	{
		return _OutlineColor;
	}
		ENDCG
	}
		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }

		Blend[_SrcBlend][_DstBlend]

		CGPROGRAM
#pragma target 3.0

		// -------------------------------------

#pragma shader_feature _NORMALMAP
#pragma shader_feature _METALLICGLOSSMAP

#pragma vertex vertBase
#pragma fragment fragBase
#include "UnityStandardCoreForward.cginc"

		ENDCG
	}

		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
	{
		Tags{ "LightMode" = "ForwardAdd" }
		Blend[_SrcBlend] One
		ZWrite Off

		CGPROGRAM
#pragma target 3.0

		// -------------------------------------


#pragma shader_feature _NORMALMAP
#pragma shader_feature _METALLICGLOSSMAP

#pragma multi_compile_fwdadd_fullshadows

#pragma vertex vertAdd
#pragma fragment fragAdd
#include "UnityStandardCoreForward.cginc"

		ENDCG
	}
	}
		FallBack "VertexLit"
}
