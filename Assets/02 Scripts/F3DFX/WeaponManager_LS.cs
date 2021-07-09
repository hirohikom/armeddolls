using UnityEngine;

public class WeaponManager_LS : MonoBehaviour
{
    public bool online = true;

    private GameObject RootObject;
    private Transform ChestObject;
    private Transform LookTargetForward;
    private Transform IKLookAtObject;

    private GameObject AimObject;
    public Vector3 AimOffset = new Vector3(0, 0.3f, 0);
    public static bool isShoot = false;

    public GameObject MountArmObject;
    public GameObject WeaponObject;

    public PhotonView photonView;

    void Start()
    {
        RootObject = transform.root.gameObject;
        ChestObject = RootObject.GetComponentInChildren<Chest>().transform;
        LookTargetForward = RootObject.GetComponentInChildren<LookTargetForward>().transform;
        IKLookAtObject = RootObject.GetComponentInChildren<IK_LookAt>().transform;
    }

    void Update()
    {
        //search enemy
        if (online && !photonView.isMine)
        {
            AimObject = AimingTarget2.CurrentAimedObject;
        }
        else
        {
            AimObject = AimingTarget.CurrentAimedObject;
        }

        if (online && photonView.isMine)
        {
            if (AimObject != null)
            {
                photonView.RPC("AimingTargetPosition_LS", PhotonTargets.All, AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_LS = true;
                photonView.RPC("SetAttackPosition_LS", PhotonTargets.All, LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_LS = false;
                Quaternion rot = ChestObject.rotation;
                photonView.RPC("ReleaseAttackPosition_LS", PhotonTargets.All, rot, WeaponManager.isLookAtTarget);
            }
        }
        else if (!online)
        {
            if (AimObject != null)
            {
                AimingTargetPosition_LS(AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_LS = true;
                SetAttackPosition_LS(LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_LS = false;
                Quaternion rot = ChestObject.rotation;
                ReleaseAttackPosition_LS(rot, WeaponManager.isLookAtTarget);
            }
        }
    }

    [PunRPC]
    public void AimingTargetPosition_LS(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        MountArmObject.transform.LookAt(pos);
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void SetAttackPosition_LS(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        MountArmObject.transform.LookAt(pos);
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void ReleaseAttackPosition_LS(Quaternion rot, bool isLookAtTarget)
    {
        IK_Ex.ikLookAtActive = isLookAtTarget;
        MountArmObject.transform.rotation = rot;
        MountArmObject.transform.Rotate(0, -90.0f, 90.0f);
        WeaponObject.transform.rotation = rot;
        WeaponObject.transform.Rotate(0, -90.0f, 90.0f);
    }
}
