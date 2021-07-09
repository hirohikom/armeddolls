using UnityEngine;
using System.Collections;
using Apex;
using Apex.Steering;
using Apex.Steering.Behaviours;
using Apex.Units;


public class SpiderController : MonoBehaviour
{
	private Transform OwnTrfm;
	private GameObject Player;
	private Transform PlayerTrfm;
	private Animator _animator;
	private WanderBehaviour _wanderBehaviour;
	private float _speed; 

	public enum OwnState { idle, trace, attack, damage, die };
	public OwnState nowOwnState = OwnState.idle;

	//Spiderの索敵距離
	public float SearchDistance = 30.0f;
	//Spiderの索敵角度
	public float SearchAngle = 30.0f;
	//Spiderの攻撃距離
	public float AttackDistance = 3.0f;

	public float MaxHP = 100.0f;
	private float NowHP;

	//Spiderの死亡判定用
	private bool isDie = false;


	void Awake()
	{
	}


	// Use this for initialization
	void Start () 
	{
        OwnTrfm = this.gameObject.GetComponent<Transform>();
        Player = GameObject.FindWithTag("Player");
        PlayerTrfm = Player.gameObject.GetComponent<Transform>();
        _animator = this.gameObject.GetComponent<Animator>();
        _wanderBehaviour = this.gameObject.GetComponent<WanderBehaviour>();
        NowHP = MaxHP;
        //一定間隔で自分の状態をチェックする
        StartCoroutine(this.CheckOwnState ());
		//状態に応じて動作を実行する
		StartCoroutine (this.OwnAction ());
	}

	void OnEnable()
	{
        PlayerController.OnPlayerDown += this.OnPlayerDown;
	}

	void ONDisable()
	{
		PlayerController.OnPlayerDown -= this.OnPlayerDown;
	}


	//一定間隔でPlayerの位置をチェックして状態を変える
	IEnumerator CheckOwnState()
	{
		while (!isDie) 
		{
			yield return new WaitForSeconds(0.25f);

			Vector3 PlayerPos = new Vector3(PlayerTrfm.position.x, 0, PlayerTrfm.position.z);
			Vector3 OwnPos = new Vector3(OwnTrfm.position.x, 0, OwnTrfm.position.z);

			//Playerが視界に入っているか
			float angle = Vector3.Angle(OwnTrfm.forward, (PlayerPos - OwnPos));
			if (angle <= SearchAngle)
			{
				//Playerとの距離を測定
				float dist =Vector3.Distance(OwnPos, PlayerPos);

				//距離に応じて状態を変える
				if (dist <= AttackDistance)
				{
					if (!PlayerController.isDown)
						nowOwnState = OwnState.attack;
					else
						nowOwnState = OwnState.idle;
				}
				else if (dist <= SearchDistance)
				{
					if (!PlayerController.isDown)
						nowOwnState = OwnState.trace;
					else
						nowOwnState = OwnState.idle;
				}
				else
				{
					nowOwnState = OwnState.idle;
				}
			}
			else
			{
				nowOwnState = OwnState.idle;
			}
		}
	}

	//状態に応じてアクションを実行
	IEnumerator OwnAction()
	{
		while(!isDie)
		{
			switch (nowOwnState)
			{
			case OwnState.idle:
				//攻撃を停止
				_animator.SetBool("isAttack", false);
				//徘徊を開始
				_wanderBehaviour.enabled = true;
				break;
			case OwnState.trace:
				//攻撃を停止
				_animator.SetBool("isAttack", false);
				//徘徊を停止
				_wanderBehaviour.enabled = false;
				//Playerの位置へ向けて移動
				var unit = this.gameObject.GetComponent<IMovable>();
				unit.MoveTo(PlayerTrfm.position, false);
				break;
			case OwnState.attack:
				//徘徊を停止
				_wanderBehaviour.enabled = false;
				//Playerの方向を向く
				OwnTrfm.LookAt(PlayerTrfm.position);
				//攻撃を開始
				_animator.SetBool("isAttack", true);
				break;
			case OwnState.damage:
				//ダメージ状態に遷移
				_animator.SetBool("gotDamage", true);
				//徘徊を停止
				_wanderBehaviour.enabled = false;
				break;
			}
			yield return null;
		}
	}

	void LateUpdate()
	{
		if (nowOwnState == OwnState.damage) 
		{
			_animator.SetBool("gotDamage", false);
			nowOwnState = OwnState.idle;
		}
	}

    //ダメージ処理
    //_params[0]:Vector3　発射地点
    //_params[1]:Vector3　着弾地点
    //_params[2]:Vector3　着弾地点の法線
    //_params[3]:float　最大ダメージ量
    void OnDamage(object[] _params)
	{
		float _damage = (float)_params[3];
		NowHP -= _damage;
		if (NowHP <= 0) 
		{
			StopAllCoroutines();
			this.gameObject.tag = "Untagged";
			foreach(Transform trfm in transform)
			{
				trfm.tag = "Untagged";
			}
			NowHP = 0;
			isDie = true;
			var unit = this.GetUnitFacade();
			unit.DisableMovementOrders();
			_animator.SetBool("isDie", true);

			this.gameObject.GetComponentInChildren<SphereCollider>().enabled = false;
			foreach(Collider col in this.gameObject.GetComponentsInChildren<CapsuleCollider>())
			{
				col.enabled = false;
			}
		}
		Debug.Log ("SpiderHP:" + NowHP);
	}

	void OnPlayerDown()
	{
		if (!isDie) 
		{
			//攻撃を停止
			_animator.SetBool("isAttack", false);
			//徘徊を開始
			_wanderBehaviour.enabled = true;
			nowOwnState = OwnState.idle;
		}
	}

}
