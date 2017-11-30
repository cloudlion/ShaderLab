using UnityEngine;
using System.Collections;

public class ShaderTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Renderer r = GetComponent<Renderer> ();
		Material m = r.sharedMaterial;
		m.EnableKeyword ("HOBBY_OFF");
		m.DisableKeyword ("HOBBY_ON");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
