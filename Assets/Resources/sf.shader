Shader "Custom/sf" {
	Properties {
		_TexSize("TexSize",Float) = 256
		_BlurRadius ("BlurRadius",Range(1,15) ) = 1
		_MaskTex ("MaskTexture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:blend


		fixed4 GetBlurColor(float2 uv);

		struct Input {
			float2 uv_MaskTex;
		};

		sampler2D _MaskTex;
		float _TexSize;
		int _BlurRadius;


		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 col = GetBlurColor(IN.uv_MaskTex);
			o.Albedo = col.rgb;
			o.Alpha = col.a;
		}

		fixed4 GetBlurColor(float2 uv){

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
//	FallBack "Diffuse"
}
