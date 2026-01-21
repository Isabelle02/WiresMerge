Shader "Custom/LightLineUniform"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _UniformAlpha ("Uniform Alpha", Range(0, 1)) = 0.8
        _EdgeThickness ("Edge Thickness", Range(0, 0.2)) = 0.05
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
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
            
            sampler2D _MainTex;
            float4 _Color;
            float4 _EdgeColor;
            float _UniformAlpha;
            float _EdgeThickness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Равномерная альфа по всей линии
                float alpha = _UniformAlpha;
                
                // Добавляем только края без срезания центра
                float edge = smoothstep(0.5 - _EdgeThickness, 0.5, abs(i.uv.y - 0.5));
                
                // Основной цвет с равномерной альфой
                fixed4 col = _Color;
                col.a = alpha;
                
                // Добавляем край другого цвета (опционально)
                if (edge > 0.1)
                {
                    col.rgb = lerp(_Color.rgb, _EdgeColor.rgb, edge);
                }
                
                return col;
            }
            ENDCG
        }
    }
}