using UnityEngine;
using System.Collections;
using System.Linq;

public class BarrelController : MonoBehaviour {

    public bool online = true;

    public GameObject ExplosionPrefab;
	private Transform tr;
	public float ExplosionRadius = 20.0f;
	public float ExplosionForce = 30.0f;
	private float Damage = 0;
	public float DamageLimit = 10.0f;
    private GameObject[] TargetArray;

    public PhotonView photonView;

	void Start()
	{
		tr = this.GetComponent<Transform>();
 
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.relativeVelocity.magnitude > 5) 
		{
            if (online)
                photonView.RPC("StartExplosion", PhotonTargets.All);
            else
                StartExplosion();
        }
	}

	void OnDamage(object[] _params)
	{
		float dmg = (float)_params [3];
		Damage += dmg;
        if (Damage >= DamageLimit)
        {
            if (online)
                photonView.RPC("StartExplosion", PhotonTargets.All);
            else
                StartExplosion();
        }
    }

    [PunRPC]
    void StartExplosion()
	{
        StartCoroutine(this.ExplosionBarrel());
        if (gameObject != null)
            Destroy(gameObject, 0.0f);
/*        if (ExplosionPrefab.gameObject != null)
            Destroy(ExplosionPrefab.gameObject, 0.0f);*/
    }

    IEnumerator ExplosionBarrel()
	{
        Instantiate(ExplosionPrefab, tr.position, Quaternion.identity);

        TargetArray = null;

        TargetArray =
        GameObject.FindObjectsOfType<GameObject>()
        .Where(g => Vector3.Distance(g.transform.position, tr.position) <= ExplosionRadius)
        .ToArray();

		object[] _params = new object[4];
		_params[0] = null;
        _params[1] = null;
        _params[2] = null;
        _params[3] = ExplosionForce;
        foreach (GameObject obj in TargetArray)
        {
            obj.SendMessage("OnDamege", _params, SendMessageOptions.DontRequireReceiver);
        }

        if (gameObject != null)
            Destroy(gameObject, 0.0f);
        if (ExplosionPrefab.gameObject != null)
            Destroy(ExplosionPrefab.gameObject, 0.0f);

        yield return null;
	}




}
