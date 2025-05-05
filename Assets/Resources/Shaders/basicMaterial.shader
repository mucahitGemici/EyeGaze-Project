Shader "Custom/basicMaterial" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}

	Category{

		BindChannels{
			Bind "Color", color
		}

		SubShader {
			Pass{
				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				Color[_Color]
				Cull Front
				Fog{ Mode Off }
			}
		}
	}
}