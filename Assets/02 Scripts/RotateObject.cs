using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

    public float RotateSpeed;

	void Update () 
	{
        transform.Rotate(new Vector3(0, 0, RotateSpeed * Time.deltaTime));
	}
}
