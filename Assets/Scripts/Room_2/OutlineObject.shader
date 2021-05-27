
//This shader creates the matte
Shader "Custom/OutlineObject"
{
	SubShader
	{
		ZWrite Off
		ZTest Always
        Lighting Off

		Pass
		{
			CGPROGRAM
			#pragma vertex VShader
			#pragma fragment FShader
			#include <UnityShaderUtilities.cginc>

			struct VertexToFragment
			{
				float4 pos:POSITION; //was SV_POSITION
			};

			//just get the position correct
			VertexToFragment VShader(VertexToFragment i)
			{
				VertexToFragment o;
				o.pos = UnityObjectToClipPos(i.pos);
				return o;
			}

			//return white
			half4 FShader():COLOR0
			{
				return half4(1,1,1,1);
			}

			ENDCG
		}
	}
}
