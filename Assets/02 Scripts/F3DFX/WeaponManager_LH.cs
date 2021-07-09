using UnityEngine;

public class WeaponManager_LH : MonoBehaviour
{
    public bool online = true;

    private GameObject RootObject;
    private Transform LookTargetForward;
    private Transform IkRootObject;
    private Transform IKLookAtObject;
    private Transform IkHandObject;
    private Transform AttackPosition;
    private Transform StandByPosition;

    public GameObject AimObject;
    public Vector3 AimOffset = new Vector3(0, 0.3f, 0);
    public static bool isShoot = false;

    public GameObject WeaponObject;

    public PhotonView photonView;

    void Start()
    {
        RootObject = transform.root.gameObject;
        LookTargetForward = RootObject.GetComponentInChildren<LookTargetForward>().transform;
        IkRootObject = RootObject.GetComponentInChildren<IKPositionRoot>().transform;
        IKLookAtObject = RootObject.GetComponentInChildren<IK_LookAt>().transform;
        IkHandObject = RootObject.GetComponentInChildren<IK_LeftHand>().transform;
        AttackPosition = RootObject.GetComponentInChildren<LeftAttackPosition>().transform;
        StandByPosition = RootObject.GetComponentInChildren<LeftStandByPosition>().transform;
        IK_Ex.ikLeftHandActive = true;
    }

    private void Update()
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
                photonView.RPC("AimingTargetPosition_LH", PhotonTargets.All, AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_LH = true;
                photonView.RPC("SetAttackPosition_LH", PhotonTargets.All, LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_LH = false;
                photonView.RPC("ReleaseAttackPosition_LH", PhotonTargets.All, WeaponManager.isLookAtTarget);
            }
        }
        else if (!online)
        {
            if (AimObject != null)
            {
                AimingTargetPosition_LH(AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_LH = true;
                SetAttackPosition_LH(LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_LH = false;
                ReleaseAttackPosition_LH(WeaponManager.isLookAtTarget);
            }
        }
    }

    [PunRPC]
    public void AimingTargetPosition_LH(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        IkRootObject.LookAt(pos);
        IkHandObject.position = AttackPosition.position;
        IkHandObject.rotation = AttackPosition.rotation;
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void SetAttackPosition_LH(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        IkRootObject.LookAt(pos);
        IkHandObject.position = AttackPosition.position;
        IkHandObject.rotation = AttackPosition.rotation;
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void ReleaseAttackPosition_LH(bool isLookAtTarget)
    {
        IK_Ex.ikLookAtActive = isLookAtTarget;
        IkRootObject.localRotation = Quaternion.Euler(0, 0, 90f);
        IkHandObject.position = StandByPosition.position;
        IkHandObject.rotation = StandByPosition.rotation;
        WeaponObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
