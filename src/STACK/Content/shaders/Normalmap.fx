float DrawNormals = 0;
float CellShading = 0;
float EnableRotation = 1;
float3 LightPosition;
float3 LightColor = 1.5;
float3 AmbientColor = 0;
float4x4 MatrixTransform;

sampler TextureSampler : register(s0);
sampler NormalSampler : register(s1);

float2x2 RotationMatrix(float rotation)
{
	float c = cos(rotation);
	float s = sin(rotation);
	return float2x2(c, -s, s, c);
}

static const float PI = 3.14159265f;

struct VSOutput
{
	float4 PositionPS : SV_POSITION;
	float2 TexCoord   : TEXCOORD0;
	float3 Normal     : TEXCOORD1;
	float2 Pos        : TEXCOORD2;
	float4 Color      : COLOR;
};

struct VSInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal   : NORMAL;
	float4 Color    : COLOR;
};

float4 main(VSOutput input) : COLOR0
{	
	float4 tex = tex2D(TextureSampler, input.TexCoord);
	float3 normal = normalize((2.0 * (tex2D(NormalSampler, input.TexCoord)) - 1.0) * -1);
	float rotation = 0;

	if (EnableRotation == 1)
	{
		rotation = input.Normal.x;
	}
	
	float2x2 rotationMatrix = RotationMatrix(rotation);
	float2 rot = mul(normal.xy, rotationMatrix);

	normal.x = -rot.x;
	normal.y = rot.y;
	normal.b = 1;	

	float3 LightDirection = float3(0, 0, 0);
	LightDirection.xy = -LightPosition + input.Pos;
	LightDirection.z = LightPosition.z;
			
	float lightAmount = saturate(dot(normal, normalize(LightDirection)));
	
	if (CellShading == 1) 
	{
		if (lightAmount > 0.95) 
		{
			lightAmount = 1.0;
		}
		else if (lightAmount > 0.5) 
		{
			lightAmount = 0.7f;
		}
		else if (lightAmount > 0.05) 
		{
			lightAmount = 0.35f;
		}
		else 
		{
			lightAmount = 0.1f;
		}
	}
	
	input.Color.rgb *= AmbientColor + lightAmount * LightColor;	
	
	return tex  * input.Color * (1 - DrawNormals) + float4(normal.rgb, tex.a) * DrawNormals; 
}

VSOutput VSBasic(VSInput vin)
{
	VSOutput vout;

	vout.PositionPS = mul(vin.Position, MatrixTransform);
	vout.TexCoord = vin.TexCoord;
	vout.Normal = vin.Normal;
	vout.Color = vin.Color;
	vout.Pos = vin.Position;

	return vout;
}

technique Depthmap
{
    pass Pass1
    {   
		VertexShader = compile vs_2_0 VSBasic();
        PixelShader = compile ps_2_0 main();
    }
}
