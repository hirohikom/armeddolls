using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class F3DSniperController_R : Photon.MonoBehaviour
{
    // Singleton instance
    public static F3DSniperController_R instance;

    // Current firing socket
    int curSocket;
    // Timer reference                
    int timerID = -1;

    public bool online = true;


    [Header("Weapon setup")]
    public Transform[] TurretSocket;            // Sockets reference
    public ParticleSystem[] ShellParticles;     // Bullet shells particle system
    public float errorRange = 1.0f;
    public float shootingInterval = 0.5f;
    private EventTrigger eventTrigger;

    [Header("Prefab setup")]
    public Transform sniperBeam;
    public Transform sniperMuzzle;

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
        if (!online || photonView.isMine)
        {
            StartCoroutine(SetEventTriggers());
        }

        this.transform.root.GetComponentInChildren<WeaponManager_RH>().WeaponObject = this.gameObject;
    }

    IEnumerator SetEventTriggers()
    {
        while (!GameManager.UIReady)
        { }

        //Get EventTrigger and Make List of Event
        eventTrigger = GameObject.Find("RightHandButton").GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();

        //Add PointerDown Evevt
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { StartFire(); });
        eventTrigger.triggers.Add(entry);

        //Add PointerUp Evevt
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener((eventData) => { StopFire(); });
        eventTrigger.triggers.Add(entry2);

        yield return null;
    }

    public void StartFire()
    {
        if (!online || photonView.isMine)
        {
            WeaponManager_RH.isShoot = true;
            Fire();
        }
    }

    public void StopFire()
    {
        if (!online)
        {
            Stop();
            WeaponManager_RH.isShoot = false;
        }
        else if (photonView.isMine)
        { 
            photonView.RPC("Stop", PhotonTargets.All);
            WeaponManager_RH.isShoot = false;
        }
    }
    public void Fire()
    {
        timerID = F3DTime.time.AddTimer(shootingInterval, Sniper);
        Sniper();
    }

    // Stop firing 
    [PunRPC]
    public void Stop()
    {
        // Remove firing timer
        if (timerID != -1)
        {
            F3DTime.time.RemoveTimer(timerID);
            timerID = -1;
        }
    }

    // Fire sniper weapon
    void Sniper()
    {
        if (!WeaponManager_RH.isShoot)
            return;

        Quaternion offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere * errorRange);

        if (online)
            photonView.RPC("SpawnSniper", PhotonTargets.All, offset);
        else
            SpawnSniper(offset);

        F3DAudioController.instance.SniperShot(TurretSocket[curSocket].position);

        //        ShellParticles[curSocket].Emit(1);

        AdvanceSocket();
    }

    [PunRPC]
    public void SpawnSniper(Quaternion offset)
    {
        F3DPool.instance.Spawn(sniperMuzzle, TurretSocket[curSocket].position, TurretSocket[curSocket].rotation, TurretSocket[curSocket]);
        F3DPool.instance.Spawn(sniperBeam, TurretSocket[curSocket].position, offset * TurretSocket[curSocket].rotation, null);
    }

    // Advance to next turret socket
    void AdvanceSocket()
    {
        curSocket++;
        if (curSocket > TurretSocket.Length - 1)
            curSocket = 0;
    }
}
