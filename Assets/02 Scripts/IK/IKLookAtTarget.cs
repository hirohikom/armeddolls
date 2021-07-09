using UnityEngine;
using System.Collections;

public class IKLookAtTarget : MonoBehaviour {

	private GameObject IKLookAtObject;
	public float IKWeight = 1.0f;
	public float IKBodyWeight = 0.65f;
	public float IKHeadWeight = 0.35f;
	public float IKClampWeight = 0.6f;
	private Animator animator;
    private GameObject AimObject;

    public PhotonView photonView;


	// Use this for initialization
	void Start ()
    {
		animator = GetComponent<Animator>();

        if (photonView.isMine)
        {
            AimObject = AimingTarget.CurrentAimedObject;
        }
        else
        {
            AimObject = AimingTarget2.CurrentAimedObject;
        }
    }
	
	void OnAnimatorIK()
	{
        if (AimObject != null)
            IKLookAtObject = AimObject;
        else
            IKLookAtObject = null;
		if (IKLookAtObject != null) {
			animator.SetLookAtWeight (IKWeight, IKBodyWeight, IKHeadWeight, 0, IKClampWeight);
			animator.SetLookAtPosition (IKLookAtObject.transform.position);
		}
	}
}