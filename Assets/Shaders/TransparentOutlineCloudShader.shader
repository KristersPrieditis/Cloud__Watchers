Shader "Custom/TransparentOutlineCloudShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 0.03
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _CloudTex ("Cloud Texture", 2D) = "white" {}
        _CloudSpeed ("Cloud Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _MainTex;
            sampler2D _CloudTex;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _Transparency;
            float _CloudSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            // Vertex function
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Fragment function
            float4 frag (v2f i) : SV_Target
            {
                // Base texture and transparency
                float4 baseColor = tex2D(_MainTex, i.uv);
                baseColor.a *= _Transparency;

                // Outline effect using normals
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float outline = 1.0 - saturate(dot(viewDir, i.worldNormal));
                outline = smoothstep(1.0 - _OutlineThickness, 1.0, outline);
                float4 outlineColor = _OutlineColor * outline;

                // Cloud movement
                float2 cloudUV = i.uv + _Time.yx * _CloudSpeed;
                float4 cloudTex = tex2D(_CloudTex, frac(cloudUV));
                baseColor.rgb += cloudTex.rgb * 0.2; // Add subtle cloud effect

                // Combine base color and outline
                return lerp(baseColor, outlineColor, outline);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
