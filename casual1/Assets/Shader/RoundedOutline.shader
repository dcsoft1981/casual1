Shader "UI/RoundedOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.05
        _CornerRadius ("Corner Radius", Float) = 0.2
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _OutlineWidth;
            float _CornerRadius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5; // Center the UV coordinates
                float distance = length(uv); // Distance from the center
                float outline = smoothstep(_OutlineWidth, _OutlineWidth + 0.01, abs(distance - 0.5));
                float corner = smoothstep(_CornerRadius, _CornerRadius + 0.01, distance);

                return lerp(float4(0,0,0,0), _Color, outline * corner);
            }
            ENDCG
        }
    }
}
