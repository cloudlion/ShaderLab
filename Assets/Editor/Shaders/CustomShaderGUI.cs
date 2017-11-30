using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CustomShaderGUI : ShaderGUI {

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		base.OnGUI (materialEditor, properties);
		Material targetMat = materialEditor.target as Material;
		SetKeyword (targetMat, "HOBBY_ON");
		SetKeyword (targetMat, "HOBBY_OFF");
	}

	void SetKeyword(Material m, string keyword)
	{
		bool redify = Array.IndexOf(m.shaderKeywords, keyword) != -1;
		EditorGUI.BeginChangeCheck();
		redify = EditorGUILayout.Toggle(keyword, redify);
		if (EditorGUI.EndChangeCheck())
		{
			// enable or disable the keyword based on checkbox
			if (redify)
				m.EnableKeyword(keyword);
			else
				m.DisableKeyword(keyword);
		}
	}
}
