// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "AwesomeTechnologies/Billboards/RenderNormalsAtlas"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Cutoff" , Range(0,1)) = .5
		_FlipBackNormals("Flip Backfacing Normals", Int) = 0
		_DepthBoundsSize("DepthBoundSize",Float) = 0
	}
	
	SubShader
	{
		Cull Off
		
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D	_MainTex;
			sampler2D	_CameraDepthTexture;
			float _Cutoff;
			int _FlipBackNormals;
			float _DepthBoundsSize;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 n : TEXCOORD1;
				float depth : TEXCOORD2;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;

				o.depth = (UnityObjectToViewPos( v.vertex ).z) + _DepthBoundsSize;						

				//half3 wNormal = UnityObjectToWorldNormal(v.normal);
                //half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                //half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                //half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the world to tangent space matrix
                //half3x3 worldToTangentMatrix = half3x3(wTangent, wBitangent, wNormal);


				o.n = mul( unity_ObjectToWorld, float4( v.normal, 0.0 ) ).xyz; // convert normal to world space at first (its in local space in data)
				//o.n = mul(worldToTangentMatrix, o.n);
				
				o.n = mul(UNITY_MATRIX_V,float4(o.n,0)).xyz; // then convert to view space, that means correct attenuation if texture quad stays in 0,0,0 with rotation 0,0,0	
				return o;
			}

			half4 frag(v2f i,fixed facing : VFACE) : COLOR
			{
				half4 c = tex2D (_MainTex, i.uv);
				clip(c.a-_Cutoff);
				i.n = normalize(i.n);						
				return half4 ((i.n+1.0)*0.5,i.depth/(_DepthBoundsSize*2));
				//return half4 ((i.n+1.0)*0.5,1);			
			}
			ENDCG

		}
	}
		Fallback "VertexLit"
}