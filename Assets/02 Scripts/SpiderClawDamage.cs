using UnityEngine;
using System.Collections;

public class SpiderClawDamage : MonoBehaviour {

	public float Damage = 1.8f;

	void OnTriggerEnter (Collider col) 
	{
		if (col.gameObject.tag != "Enemy") 
		{
            AppearForceField(col, Damage);

            object[] _params = new object[4];
			_params [3] = Damage;
            col.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
		}
	}

    void AppearForceField(Collider col, float force)
    {
        Forcefield forcefield = col.gameObject.GetComponent<Forcefield>();
        if (forcefield != null)
        {
            float hitPower = force * 0.01f;// Random.Range(-7.0f, 1.0f);
            forcefield.OnHit(col.transform.position, hitPower);
        }
    }
}
