using UnityEngine;
using System.Collections;

public class LifeTimeController : MonoBehaviour {

	public float LifeTime = 1.0f;

	// Use this for initialization
	void Start () 
	{
		Destroy (this.gameObject, LifeTime);	
	}
}
