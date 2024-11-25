Shader "Custom/OutlinedShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 0.5, 0, 1) // Cor do contorno (amarelo por padrão)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.2)) = 0.1 // Espessura do contorno
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // Renderizar o contorno
        Pass
        {
            Name "OUTLINE"
            Cull Front // Inverter culling para expandir o contorno para fora
            ZWrite On
            ZTest LEqual
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                // Expande os vértices para criar o contorno
                v2f o;
                float3 normalDir = normalize(v.normal) * _OutlineThickness;
                o.pos = UnityObjectToClipPos(v.vertex + float4(normalDir, 0));
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color; // Apenas a cor do contorno
            }
            ENDCG
        }

        // Renderizar a textura normal do objeto
        Pass
        {
            Name "TEXTURE"
            Cull Back // Renderizar apenas a geometria visível
            ZWrite On
            ZTest LEqual
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv); // Renderiza a textura normal do objeto
            }
            ENDCG
        }
    }
}
