#ifndef RIPPLE_EFFECT_INCLUDED
#define RIPPLE_EFFECT_INCLUDED

float2 RippleOffset(float2 uv, float rangeFactor, float lengthFactor, float height)
{
	float2 dv = float2(0.5, 0.5) - uv;
	float dist = sqrt(dv.x * dv.x + dv.y * dv.y);
	float PI = 3.1415;
	float sinFactor =  (sin((dist- _Time.y * 0.1)*lengthFactor*PI ));
	float finalFactor = sinFactor*height;
	float2 dv1 = normalize(dv);
	return dv1*finalFactor;
}

#endif
