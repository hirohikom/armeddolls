using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class AimingTarget : MonoBehaviour
{
    public bool online = true;

	public bool OnActive = true;
	public bool ShowCrosshair = true;
	private string TargetTag;
	public string ScopeTag = "Scope";
	public string IgnoreLayer = "Player";
	public float MaxSearchDistance = 30.0f;
	public float MaxSearchAngle = 10.0f;
	public GameObject ScopeRect;
	public Texture ScopeTexture;
	public int CurrentAimedMax = 1;
	public GameObject[] TargetArray;
	public static GameObject CurrentAimedObject;
    //public UISprite Crosshair;

    public PhotonView photonView;

    void Awake()
    {
        if (online && !photonView.isMine)
        {
            TargetTag = "Player";
            Destroy(this);
        }
        else
        {
            TargetTag = "Enemy";
        }
    }

    void Update ()
	{
		if (OnActive) {

			Vector3 currentPos = transform.position;
			Vector3 fwd = transform.forward;

			TargetArray = null;

			TargetArray = 
			//tagがEnemyのものを抽出
			GameObject.FindGameObjectsWithTag(TargetTag)
			//索敵角度内か？
			.Where(g => Vector3.Angle(fwd, g.transform.position - currentPos) <= MaxSearchAngle)
			//索敵距離内か？
			.Where(g => Vector3.Distance(g.transform.position, currentPos) <= MaxSearchDistance)
			//近い順にソート
			.OrderBy(g => Vector3.Distance(g.transform.position, currentPos))
			.ToArray();

			if (TargetArray.Length > 0) 
			{
				CurrentAimedObject = TargetArray[0];
			}
			else
			{
				CurrentAimedObject = null;
			}
		}
	}
}