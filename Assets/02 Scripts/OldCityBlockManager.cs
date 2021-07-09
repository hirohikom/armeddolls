using UnityEngine;
using System.Collections;

public class OldCityBlockManager : MonoBehaviour {

    public string LoadUIName;
    public string LoadUnitName;
    public string LoadWeaponName_LS;
    public string LoadWeaponName_RS;
    public string LoadWeaponName_LH;
    public string LoadWeaponName_RH;
    public string LoadWeaponName_BP;

    private GameObject BattleUI;
    public static GameObject DollUnit;

    //敵の出現ポイント
    public Transform[] SpawnPoints;
	//敵のプレハブ
	public GameObject EnemyPrefab;
	public GameObject BigEnemyPrefab;
	//敵の最大数
	public int EnemyCount = 12;
	public int BigEnemyCount = 3;
	private int MaxEnemy;
	//ステージ終了を示す変数
	public bool isGameOver;

    // Load UI & PlayerUnit
    void Awake ()
    {
        // Load PlayerUnit
        if (LoadUnitName != null)
        {
            DollUnit = Instantiate(Resources.Load(LoadUnitName), new Vector3(0, 0.1f, -60f), Quaternion.identity) as GameObject;
        }
        // Camera Start
        DollUnit.GetComponent<RPGCameraEx>().online = false;
        DollUnit.GetComponent<RPGCameraEx>().enabled = true;

        // Load Battle UI
        if (LoadUIName != null)
        {
            BattleUI = Instantiate(Resources.Load(LoadUIName), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            GameManager.UIReady = true;
        }

        // Load Weapons
        LoadWeapons();
    }

    // Use this for initialization
    void Start () 
	{
		MaxEnemy = EnemyCount + BigEnemyCount;

		SpawnPoints = GameObject.Find ("SpawnPoints").GetComponentsInChildren<Transform> ();

		if (SpawnPoints.Length <= MaxEnemy)
			MaxEnemy = SpawnPoints.Length - 1;

		if (SpawnPoints.Length > 0)
			StartCoroutine (this.CreateEnemy ());

        GameManager.LoadedScene = true;
    }

    void LoadWeapons ()
    {
        //Load Left Shoulder Weapon
        if (LoadWeaponName_LS != null)
        {
            GameObject parent = DollUnit.GetComponentInChildren<AttachPoint_LS>().gameObject;
            GameObject weapon = Instantiate(Resources.Load(LoadWeaponName_LS), parent.transform.position, Quaternion.identity) as GameObject;
            weapon.transform.SetParent(parent.transform);
        }

        //Load Right Shoulder Weapon
        if (LoadWeaponName_RS != null)
        {
            GameObject parent = DollUnit.GetComponentInChildren<AttachPoint_RS>().gameObject;
            GameObject weapon = Instantiate(Resources.Load(LoadWeaponName_RS), parent.transform.position, Quaternion.identity) as GameObject;
            weapon.transform.SetParent(parent.transform);
        }

        //Load Left Hand Weapon
        if (LoadWeaponName_LH != null)
        {
            GameObject parent = DollUnit.GetComponentInChildren<AttachPoint_LH>().gameObject;
            GameObject weapon = Instantiate(Resources.Load(LoadWeaponName_LH), parent.transform.position, Quaternion.identity) as GameObject;
            weapon.transform.SetParent(parent.transform);
        }

        //Load Right Hand Weapon
        if (LoadWeaponName_RH != null)
        {
            GameObject parent = DollUnit.GetComponentInChildren<AttachPoint_RH>().gameObject;
            GameObject weapon = Instantiate(Resources.Load(LoadWeaponName_RH), parent.transform.position, Quaternion.identity) as GameObject;
            weapon.transform.SetParent(parent.transform);
        }

        //Load Back Pack Weapon
        if (LoadWeaponName_BP != null)
        {
            GameObject parent = DollUnit.GetComponentInChildren<AttachPoint_BP>().gameObject;
            GameObject weapon = Instantiate(Resources.Load(LoadWeaponName_BP), parent.transform.position, Quaternion.identity) as GameObject;
            weapon.transform.SetParent(parent.transform);
        }
    }

    IEnumerator CreateEnemy () 
	{
		bool[] indexs = new bool[SpawnPoints.Length];
		for (int i = 1; i < SpawnPoints.Length; i++) 
		{
			indexs[i] = false;
		}
		int cnt = 1;
		while (cnt < MaxEnemy)
		{
			int i = Random.Range(0, SpawnPoints.Length - 1);
			if (!indexs[i])
			{
				indexs[i] = true;
				if (cnt <= BigEnemyCount)
					Instantiate(BigEnemyPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
				else
					Instantiate(EnemyPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
				cnt++;
			}
		}
		yield return null;
	}

}
