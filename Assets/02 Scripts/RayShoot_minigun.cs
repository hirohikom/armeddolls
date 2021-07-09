using UnityEngine;
using System.Collections;

public class RayShoot_minigun : MonoBehaviour
{
	public float Damage = 2.0f;
	public float Range = 1000.0f;
	public Vector3 AimPoint;
	public GameObject Explosion;
	public float LifeTime = 0.2f;
	public LineRenderer Trail;
	
	void Start ()
	{
		Vector3 StartPos = this.gameObject.transform.position;
		RaycastHit hit;
		GameObject explosion = null;
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, Range)) {
			AimPoint = hit.point;
			if (Explosion != null) {
				explosion = (GameObject)GameObject.Instantiate (Explosion, AimPoint, this.transform.rotation);
			}
			object[] _params = new object[3];
			_params [0] = StartPos;
			_params [1] = AimPoint;
			_params [2] = Damage;
			hit.collider.gameObject.SendMessage ("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
		} else {
			AimPoint = this.transform.forward * Range;
			explosion = (GameObject)GameObject.Instantiate (Explosion, AimPoint, this.transform.rotation);
			
		}
		if (Trail) {
			Trail.SetPosition (0, this.transform.position);
			Trail.SetPosition (1, AimPoint);
		}
		Destroy (this.gameObject, LifeTime);
	}

}
