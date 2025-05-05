Shader "Custom/brushMaterial" {
	Properties {
		_Color ("_Color", Color) = (1,1,1,1)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
	}

		Category{

		SubShader{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector"="True"}

			Pass{

				SetTexture[_MainTex]{
					constantColor[_Color]
					Combine texture * constant, texture * constant
				}
			}
		}
	}
}
