using UnityEngine;
using System.Collections;

public class MiniRocketDamage : MonoBehaviour
{
	public string[] TargetTag = new string[1]{"Enemy"};
	public GameObject Effect;
	public float Damage = 20.0f;
	public bool Explosive = false;
	public float ExplosionRadius = 3.0f;
	public float ExplosionForce = 20.0f;
	public bool HitedActive = true;
	public float TimeActive = 0;
	private float timetemp = 0;
	private Vector3 StartPos;

	private void Start ()
	{
		if (!GetComponent<Collider>())
			return;
		
		timetemp = Time.time;
		StartPos = this.gameObject.transform.position;
	}

	private void Update ()
	{
		if (!HitedActive || TimeActive > 0) {
			if (Time.time >= (timetemp + TimeActive)) {
				Active ();
			}
		}
	}
	
	private void OnCollisionEnter (Collision col)
	{
		if (HitedActive)
		{
			if (col.gameObject.tag != this.gameObject.tag) 
			{
				if (!Explosive)
					NormalDamage (col);
				Active ();
			}
		}
	}

	public void Active ()
	{
		if (Effect) {
			GameObject obj = (GameObject)Instantiate (Effect, transform.position, transform.rotation);
			Destroy (obj, 3);
		}

		if (Explosive)
			ExplosionDamage ();


		Destroy (gameObject);
	}

	private void ExplosionDamage ()
	{
		Collider[] hitColliders = Physics.OverlapSphere (transform.position, ExplosionRadius);
		for (int i = 0; i < hitColliders.Length; i++) {
			Collider col = hitColliders [i];
			if (!col)
				continue;
			Vector3 pos = col.gameObject.transform.position;
			object[] _params = new object[3];
			if (Vector3.Distance(StartPos, pos) > ExplosionRadius)
			{
				_params [2] = Damage;
				col.gameObject.SendMessage ("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
				Rigidbody rigidbody = col.GetComponent<Rigidbody>();
				if (rigidbody)
					rigidbody.AddExplosionForce (ExplosionForce, transform.position, ExplosionRadius, 3.0f);
			}
		}
	}

	private void NormalDamage (Collision col)
	{
		Vector3 pos = col.gameObject.transform.position;
		if (Vector3.Distance(StartPos, pos) > ExplosionRadius)
		{
			object[] _params = new object[3];
			_params [0] = StartPos;
			_params [1] = pos;
			_params [2] = Damage;
			col.gameObject.SendMessage ("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
		}
	}
}
