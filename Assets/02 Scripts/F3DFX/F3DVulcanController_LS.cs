using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class F3DVulcanController_LS : Photon.MonoBehaviour
{
    // Singleton instance
    public static F3DVulcanController_LS instance;

    // Current firing socket
    private int curSocket;          
    // Timer reference                
    private int timerID = -1;

    [Header("Weapon setup")]
    public GameObject MountArmObject;
    public GameObject WeaponObject;
    public GameObject BarrelObject;
    public Transform[] TurretSocket;            // Sockets reference
    public ParticleSystem[] ShellParticles;     // Bullet shells particle system
    public float errorRange = 1.0f;
    public float shootingInterval = 0.1f;
    private EventTrigger eventTrigger;

    [Header("Prefab setup")]
    public Transform vulcanProjectile;          // Projectile prefab
    public Transform vulcanMuzzle;              // Muzzle flash prefab  

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
            StartCoroutine(SetEventTriggers_LS());
        }

        this.transform.root.GetComponentInChildren<WeaponManager_LS>().MountArmObject = MountArmObject;
        this.transform.root.GetComponentInChildren<WeaponManager_LS>().WeaponObject = WeaponObject;
    }

    IEnumerator SetEventTriggers_LS()
    {
        while (!GameManager.UIReady)
        { }

        //Get EventTrigger and Make List of Event
        eventTrigger = GameObject.Find("LeftShoulderButton").GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();

        //Add PointerDown Evevt
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { StartFire_LS(); });
        eventTrigger.triggers.Add(entry);

        //Add PointerUp Evevt
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener((eventData) => { StopFire_LS(); });
        eventTrigger.triggers.Add(entry2);

        yield return null;
    }

    public void StartFire_LS()
    {
        if (photonView.isMine)
        {
            WeaponManager_LS.isShoot = true;
            Fire_LS();
        }
    }

    public void StopFire_LS()
    {
        if (photonView.isMine)
        {
            photonView.RPC("Stop_LS", PhotonTargets.All);
            WeaponManager_LS.isShoot = false;
        }
    }

    // Fire turret weapon
    public void Fire_LS()
    {
        timerID = F3DTime.time.AddTimer(shootingInterval, Vulcan);
        Vulcan();
    }

    // Stop firing 
    [PunRPC]
    public void Stop_LS()
    {
        // Remove firing timer
        if (timerID != -1)
        {
            F3DTime.time.RemoveTimer(timerID);
            timerID = -1;
        }
    }

    // Fire vulcan weapon
    void Vulcan()
    {
        if (!WeaponManager_LS.isShoot)
            return;

        // Get random rotation that offset spawned projectile
        Quaternion offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere * errorRange);

        photonView.RPC("ShootVulcan_LS", PhotonTargets.All, offset);
    }

    [PunRPC]
    public void ShootVulcan_LS(Quaternion offset)
    {
        F3DPool.instance.Spawn(vulcanMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
        F3DPool.instance.Spawn(vulcanProjectile, TurretSocket[curSocket].position + TurretSocket[curSocket].forward, offset * TurretSocket[curSocket].rotation, null);

        BarrelObject.transform.Rotate(0, 0, 15f);

        // Emit one bullet shell
        ShellParticles[curSocket].Emit(1);

        // Play shot sound effect
        F3DAudioController.instance.VulcanShot(TurretSocket[curSocket].position);

        // Advance to next turret socket
        AdvanceSocket();
    }

    // Advance to next turret socket
    void AdvanceSocket()
    {
        curSocket++;
        if (curSocket > TurretSocket.Length - 1)
            curSocket = 0;
    }
}
