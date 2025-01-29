Shader "Custom/Cloud"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _CloudSpeed ("Cloud Speed", Float) = 0.1
        _CloudScale ("Cloud Scale", Float) = 3.0
        _CloudDensity ("Cloud Density", Float) = 1.0
        _CloudColor ("Cloud Color", Color) = (1,1,1,1)
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

            sampler2D _MainTex;
            float _CloudSpeed;
            float _CloudScale;
            float _CloudDensity;
            float4 _CloudColor;

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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Perlin noise function for cloud texture
            float noise(float2 uv)
            {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;

                for (int i = 0; i < 4; i++) // Add more octaves for detail
                {
                    value += amplitude * noise(uv * frequency);
                    uv *= 2.0;
                    amplitude *= 0.5;
                }

                return value;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x += _CloudSpeed * _Time.y; // Animate the clouds horizontally

                // Generate cloud pattern
                float cloud = fbm(uv * _CloudScale);

                // Apply density
                cloud = smoothstep(0.5 - _CloudDensity * 0.5, 0.5 + _CloudDensity * 0.5, cloud);

                float4 baseColor = tex2D(_MainTex, uv);
                return lerp(baseColor, _CloudColor, cloud); // Blend the clouds with the base texture
            }
            ENDCG
        }
    }
}
