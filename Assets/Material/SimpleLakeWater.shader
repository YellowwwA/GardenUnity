Shader "Custom/SimpleLakeWater"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.15, 0.55, 0.65, 0.7)
        _DeepColor    ("Deep Color"   , Color) = (0.02, 0.15, 0.22, 0.9)
        _FoamColor    ("Foam Color"   , Color) = (0.90, 0.95, 1.0, 1.0)

        _Normal1 ("Normal Map 1", 2D) = "bump" {}
        _Normal2 ("Normal Map 2", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1.0
        _Tiling1 ("Tiling 1", Vector) = (0.3, 0.3, 0, 0)
        _Tiling2 ("Tiling 2", Vector) = (0.15, 0.15, 0, 0)
        _Speed1  ("Scroll Speed 1 (xy)", Vector) = (0.05, 0.02, 0, 0)
        _Speed2  ("Scroll Speed 2 (xy)", Vector) = (-0.03, 0.01, 0, 0)

        _Smoothness ("Smoothness", Range(0,1)) = 0.85
        _Metallic   ("Metallic"  , Range(0,1)) = 0.0

        _WaveAmp  ("Wave Amplitude", Range(0,0.5)) = 0.06
        _WaveLen  ("Wave Length"  , Range(0.1,10)) = 4.0
        _WaveSpeed("Wave Speed"   , Range(0,5))    = 1.2

        _FresnelPow ("Fresnel Power", Range(0.1,8)) = 4.0
        _FoamFresnel ("Foam by Fresnel", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert addshadow
        #pragma target 3.0

        sampler2D _Normal1;
        sampler2D _Normal2;

        half4 _ShallowColor, _DeepColor, _FoamColor;
        half _NormalStrength, _Smoothness, _Metallic;
        float4 _Tiling1, _Tiling2, _Speed1, _Speed2;
        half _WaveAmp, _WaveLen, _WaveSpeed, _FresnelPow, _FoamFresnel;

        struct Input {
            float2 uv_Normal1;
            float2 uv_Normal2;
            float3 worldPos;
            float3 viewDir;
        };

        // 간단한 2방향 사인 웨이브(저수면 잔물결 느낌)
        void vert (inout appdata_full v)
        {
            float2 dir1 = normalize(float2(1, 0.3));
            float2 dir2 = normalize(float2(-0.4, 1));

            float t = _Time.y * _WaveSpeed;

            float phase1 = dot(v.vertex.xz, dir1) * (6.28318 / _WaveLen) + t;
            float phase2 = dot(v.vertex.xz, dir2) * (6.28318 / (_WaveLen*1.7)) - t*0.8;

            float disp = (sin(phase1) + sin(phase2)) * 0.5;
            v.vertex.y += disp * _WaveAmp;
        }

        inline fixed3 UnpackNormalStrength(sampler2D map, float2 uv, float strength)
        {
            fixed3 n = UnpackNormal(tex2D(map, uv));
            n.xy *= strength;
            n = normalize(n);
            return n;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv1 = IN.uv_Normal1 * _Tiling1.xy + _Time.y * _Speed1.xy;
            float2 uv2 = IN.uv_Normal2 * _Tiling2.xy + _Time.y * _Speed2.xy;

            fixed3 n1 = UnpackNormalStrength(_Normal1, uv1, _NormalStrength);
            fixed3 n2 = UnpackNormalStrength(_Normal2, uv2, _NormalStrength*0.7);

            fixed3 n = normalize(n1 + n2);
            o.Normal = n;

            // 프레넬로 얕은/깊은 색 블렌드
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPow);
            fixed4 col = lerp(_DeepColor, _ShallowColor, fresnel);

            // 약간의 폼(거품) 강조
            float foam = saturate((fresnel - (1.0 - _FoamFresnel)) * 5.0);
            col.rgb = lerp(col.rgb, _FoamColor.rgb, foam * 0.15);

            o.Albedo = col.rgb;
            o.Alpha  = col.a;          // 투명도
            o.Smoothness = _Smoothness; // 하이라이트
            o.Metallic   = _Metallic;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
