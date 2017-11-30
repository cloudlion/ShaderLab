using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class ShrinkEffect : PostEffectsBase {

	[Range(0, 0.15f)]

	public float distortFactor = 1.0f;

	//扭曲中心(0-1)屏幕空间，默认为中心点

	public Vector2 distortCenter = new Vector2(0.5f, 0.5f);

	//噪声图

	public Texture NoiseTexture = null;

	//屏幕扰动强度

	[Range(0, 2.0f)]

	public float distortStrength = 1.0f;
	private Material _material;
	public Shader shrinkShader = null;

	public override bool CheckResources () {
		CheckSupport (false);

		_material = CheckShaderAndCreateMaterial (shrinkShader, _material);

		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)

	{

		if (_material)

		{

			_material.SetTexture("_NoiseTex", NoiseTexture);

			_material.SetFloat("_DistortFactor", distortFactor);

			_material.SetVector("_DistortCenter", distortCenter);

			_material.SetFloat("_DistortStrength", distortStrength);

			Graphics.Blit(source, destination, _material);

		}

		else

		{

			Graphics.Blit(source, destination);

		}

	}
}
