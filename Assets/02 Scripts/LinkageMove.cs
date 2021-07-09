using UnityEngine;
using System.Collections;

public class LinkageMove: MonoBehaviour {

	public GameObject linkageBone;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		this.transform.rotation = linkageBone.transform.rotation;
	}
}
