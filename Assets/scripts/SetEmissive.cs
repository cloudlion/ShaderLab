using UnityEngine;
using System.Collections;

public class SetEmissive : MonoBehaviour {

	public Renderer lightwall;
	bool lighton = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.T))
		{

			lighton = !lighton;

		}


//		if (lighton)
		{
			Color final = lighton ? Color.green : Color.black;
			lightwall.material.SetColor("_EmissionColor", final);
			DynamicGI.SetEmissive(lightwall, final);
		}
	}
}
