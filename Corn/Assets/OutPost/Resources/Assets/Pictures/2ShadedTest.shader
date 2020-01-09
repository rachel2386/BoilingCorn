Shader "Custom/2ShadedTest" {

    Properties {
         _Color ("Main Color", Color) = (1,1,1,1)

        _MainTex ("Base (RGB)", 2D) = "white" {}

    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 200
        

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        uniform half4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

 

        void surf(Input IN, inout SurfaceOutput o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.xyz;   // multiplicamos por el color para ajustar su luminosidad
            o.Alpha = c.a * _Color.a;
        }

        ENDCG

    } 

    FallBack "Diffuse"
}