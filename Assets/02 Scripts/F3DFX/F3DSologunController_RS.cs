using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class F3DSologunController_RS : Photon.MonoBehaviour
{
    // Singleton instance
    public static F3DSologunController_RS instance;

    // Current firing socket
    int curSocket;
    // Timer reference                
    int timerID = -1;

    [Header("Weapon setup")]
    public GameObject MountArmObject;
    public GameObject WeaponObject;
    public Transform[] TurretSocket;            // Sockets reference
    public ParticleSystem[] ShellParticles;     // Bullet shells particle system
    public float errorRange = 1.0f;
    public float shootingInterval = 0.5f;
    private EventTrigger eventTrigger;

    [Header("Prefab setup")]
    public Transform soloGunProjectile;
    public Transform soloGunMuzzle;

    [Header("PUN setup")]
    public new PhotonView photonView;

    void Awake()
    {
        // Initialize singleton  
        instance = this;

        // Initialize bullet shells particles
        if (ShellParticles != null)
        {
            for (int i = 0; i < ShellParticles.Length; i++)
            {
                ShellParticles[i].enableEmission = false;
                ShellParticles[i].gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        if (photonView.isMine)
        {
            StartCoroutine(SetEventTriggers_RS());
        }
		
		this.transform.root.GetComponentInChildren<WeaponManager_RS>().MountArmObject = MountArmObject;
		this.transform.root.GetComponentInChildren<WeaponManager_RS>().WeaponObject = WeaponObject;
	}

    IEnumerator SetEventTriggers_RS()
    {
        while (!GameManager.UIReady)
        { }

        //Get EventTrigger and Make List of Event
        eventTrigger = GameObject.Find("RightShoulderButton").GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();

        //Add PointerDown Evevt
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { StartFire_RS(); });
        eventTrigger.triggers.Add(entry);

        //Add PointerUp Evevt
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener((eventData) => { StopFire_RS(); });
        eventTrigger.triggers.Add(entry2);

        yield return null;
    }

    public void StartFire_RS()
    {
        if (photonView.isMine)
        {
            WeaponManager_RS.isShoot = true;
            Fire_RS();
        }
    }

    public void StopFire_RS()
    {
        if (photonView.isMine)
        {
            photonView.RPC("Stop_RS", PhotonTargets.All);
            WeaponManager_RS.isShoot = false;
        }
    }
    public void Fire_RS()
    {
        timerID = F3DTime.time.AddTimer(shootingInterval, SoloGun);
        SoloGun();
    }

    // Stop firing 
    [PunRPC]
    public void Stop_RS()
    {
        // Remove firing timer
        if (timerID != -1)
        {
            F3DTime.time.RemoveTimer(timerID);
            timerID = -1;
        }
    }

    // Fire sologun weapon
    void SoloGun()
    {
        if (!WeaponManager_RS.isShoot)
            return;

        Quaternion offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere * errorRange);

        photonView.RPC("SpawnSologun_RS", PhotonTargets.All, offset);

        F3DAudioController.instance.SoloGunShot(TurretSocket[curSocket].position);

        AdvanceSocket();
    }

    [PunRPC]
    public void SpawnSologun_RS(Quaternion offset)
    {
        F3DPool.instance.Spawn(soloGunMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
        F3DPool.instance.Spawn(soloGunProjectile, TurretSocket[curSocket].position + TurretSocket[curSocket].forward, offset * TurretSocket[curSocket].rotation, null);
    }

    // Advance to next turret socket
    void AdvanceSocket()
    {
        curSocket++;
        if (curSocket > TurretSocket.Length - 1)
            curSocket = 0;
    }
}
