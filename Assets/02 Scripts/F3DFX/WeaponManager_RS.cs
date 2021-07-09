using UnityEngine;

public class WeaponManager_RS : MonoBehaviour
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
                photonView.RPC("AimingTargetPosition_RS", PhotonTargets.All, AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_RS = true;
                photonView.RPC("SetAttackPosition_RS", PhotonTargets.All, LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_RS = false;
                Quaternion rot = ChestObject.rotation;
                photonView.RPC("ReleaseAttackPosition_RS", PhotonTargets.All, rot, WeaponManager.isLookAtTarget);
            }
        }
        else if (!online)
        {
            if (AimObject != null)
            {
                AimingTargetPosition_RS(AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_RS = true;
                SetAttackPosition_RS(LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_RS = false;
                Quaternion rot = ChestObject.rotation;
                ReleaseAttackPosition_RS(rot, WeaponManager.isLookAtTarget);
            }
        }
    }

    [PunRPC]
    public void AimingTargetPosition_RS(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        MountArmObject.transform.LookAt(pos);
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void SetAttackPosition_RS(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        MountArmObject.transform.LookAt(pos);
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void ReleaseAttackPosition_RS(Quaternion rot, bool isLookAtTarget)
    {
        IK_Ex.ikLookAtActive = isLookAtTarget;
        MountArmObject.transform.rotation = rot;
        MountArmObject.transform.Rotate(0, -90.0f, 90.0f);
        WeaponObject.transform.rotation = rot;
        WeaponObject.transform.Rotate(0, -90.0f, 90.0f);
    }
}
