using UnityEngine;

public class WeaponManager_RH : MonoBehaviour
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
        IkHandObject = RootObject.GetComponentInChildren<IK_RightHand>().transform;
        AttackPosition = RootObject.GetComponentInChildren<RightAttackPosition>().transform;
        StandByPosition = RootObject.GetComponentInChildren<RightStandByPosition>().transform;
        IK_Ex.ikRightHandActive = true;
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
                photonView.RPC("AimingTargetPosition_RH", PhotonTargets.All, AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_RH = true;
                photonView.RPC("SetAttackPosition_RH", PhotonTargets.All, LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_RH = false;
                photonView.RPC("ReleaseAttackPosition_RH", PhotonTargets.All, WeaponManager.isLookAtTarget);
            }
        }
        else if (!online)
        {
            if (AimObject != null)
            {
                AimingTargetPosition_RH(AimObject.transform.position + AimOffset);
            }
            else if (isShoot)
            {
                WeaponManager.isShoot_RH = true;
                SetAttackPosition_RH(LookTargetForward.position);
            }
            else
            {
                WeaponManager.isShoot_RH = false;
                ReleaseAttackPosition_RH(WeaponManager.isLookAtTarget);
            }
        }
    }

    [PunRPC]
    public void AimingTargetPosition_RH(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        IkRootObject.LookAt(pos);
        IkHandObject.position = AttackPosition.position;
        IkHandObject.rotation = AttackPosition.rotation;
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void SetAttackPosition_RH(Vector3 pos)
    {
        IKLookAtObject.transform.position = pos;
        IK_Ex.ikLookAtActive = true;
        IkRootObject.LookAt(pos);
        IkHandObject.position = AttackPosition.position;
        IkHandObject.rotation = AttackPosition.rotation;
        WeaponObject.transform.LookAt(pos);
    }

    [PunRPC]
    public void ReleaseAttackPosition_RH(bool isLookAtTarget)
    {
        IK_Ex.ikLookAtActive = isLookAtTarget;
        IkRootObject.localRotation = Quaternion.Euler(0, 0, 90f);
        IkHandObject.position = StandByPosition.position;
        IkHandObject.rotation = StandByPosition.rotation;
        WeaponObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
