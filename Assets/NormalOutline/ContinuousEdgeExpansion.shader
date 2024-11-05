Shader "Custom/AdjustableEdgeExpansionWithManualTexelSize"
{
    Properties
    {
        _OutlineTex ("Edge Texture", 2D) = "white" {}      // 输入的边缘检测纹理
        _ExpansionRange ("Expansion Range", Float) = 1.0   // 控制扩散范围
        _Samples ("Sample Count", Int) = 10                // 控制采样数量
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _OutlineTex;
            float _ExpansionRange;
            int _Samples;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // 使用传入的 _TexelSize.xy 作为 texel 大小
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float color =  0;

                // 多方向采样
                for (int s = 1; s <= _Samples; s++)
                {
                    float offsetFactor = (s / float(_Samples)) * _ExpansionRange;

                    float2 offsetX = float2(offsetFactor * texelSize.x, 0);
                    float2 offsetY = float2(0, offsetFactor * texelSize.y);
                    //Linear01Depth()
                    // 八个方向采样并累加
                    color += tex2D(_OutlineTex, i.uv + offsetX).r;
                    color += tex2D(_OutlineTex, i.uv - offsetX).r;
                    color += tex2D(_OutlineTex, i.uv + offsetY).r;
                    color += tex2D(_OutlineTex, i.uv - offsetY).r;

                    color += tex2D(_OutlineTex, i.uv + offsetX + offsetY).r;
                    color += tex2D(_OutlineTex, i.uv - offsetX + offsetY).r;
                    color += tex2D(_OutlineTex, i.uv + offsetX - offsetY).r;
                    color += tex2D(_OutlineTex, i.uv - offsetX - offsetY).r;
                }
                color = step(0.000001,color.r);
                return float4(color,color,color,1);
            }
            ENDCG
        }
    }
}
