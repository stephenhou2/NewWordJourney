Shader "Custom/FOWShader"
{
	Properties
	{
		_MaskTex ("MaskTexture", 2D) = "white" {}
		_TexSize("TexSize",Float) = 256
		_BlurRadius ("BlurRadius",Range(1,15) ) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			fixed4 GetBlurColor(float2 uv);

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


			sampler2D _MaskTex;
			float _TexSize;
			int _BlurRadius;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return GetBlurColor(i.uv);
			}


			fixed4 GetBlurColor(float2 uv)
			{

			    float space = 1.0/_TexSize; //算出一个像素的空间
			    int count = _BlurRadius * 2 +1; //取值范围
			    count *= count;

			    //将以自己为中心，周围半径的所有颜色相加，然后除以总数，求得平均值
			    fixed3 rgbTemp = fixed3(0.1,0.1,0.1);

			    float alphaTemp = 0;

			    for( int x = -_BlurRadius ; x <= _BlurRadius ; x++ )
			    {
			        for( int y = -_BlurRadius ; y <= _BlurRadius ; y++ )
			        {
			            float alpha = 1 - tex2D(_MaskTex,uv + float2(x * space,y * space)).a;

			            alphaTemp += alpha;
			        }
			    }

			    alphaTemp = alphaTemp/count;

			    return fixed4(rgbTemp,alphaTemp);
			}
			ENDCG
		}
	}
}
