Shader "AwesomeTechnologies/TouchReact/BlendTouchReactBuffer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Float) = 1
        _offsetU("Offset U",Float) = 0
        _offsetV("Offset V",Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Speed;
            float _offsetU;
            float _offsetV;

            float4 frag (v2f i) : COLOR
            {
                float2 sampleUV = float2(i.uv.x + _offsetU, i.uv.y + _offsetV);
                if (sampleUV.x < 0 || sampleUV.x > 1 || sampleUV.y < 0 || sampleUV.y > 1)
                {
                    float4 data = float4(1,1,0,0);
                    return data;
                }
                else
                {
                    float4 data = tex2D(_MainTex, sampleUV);
                    data.x += (unity_DeltaTime * _Speed * data.y)/10000;
                    return data;
                }                
            }
            ENDCG
        }
    }
}
