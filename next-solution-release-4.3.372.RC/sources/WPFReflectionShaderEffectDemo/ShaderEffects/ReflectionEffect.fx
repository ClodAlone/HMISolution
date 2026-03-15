//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL Template
//
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

float centerX : register(C0);
float leftAngle : register(C1);
float rightAngle : register(C2);
float height : register(C3);
float deep : register(C4);

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float radiansMultiplier = 3.141592 / 180;
	float edge = 0.5;
    
	if (uv.x >= centerX)
	{
		edge -= (tan(rightAngle * radiansMultiplier) * (uv.x - centerX));
	}       
	else if (uv.x < centerX)
	{
		edge -= (tan(leftAngle * radiansMultiplier) * (centerX - uv.x));
	}

	if (uv.y > edge && uv.y <= edge + height)
	{
		uv.y = edge - (uv.y - edge);
		return tex2D(implicitInputSampler, uv) * uv.y * (uv.y);
	}			

	return tex2D(implicitInputSampler, uv);
}
