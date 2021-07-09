using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class IK_Ex : MonoBehaviour {
	
	protected Animator avatar;

    public static bool ikBodyActive = false;
    public static bool ikLeftFootActive = false;
    public static bool ikRightFootActive = false;
    public static bool ikLeftHandActive = false;
    public static bool ikRightHandActive = false;
    public static bool ikLookAtActive = false;

    public Transform bodyObj = null;
	public Transform leftFootObj = null;
	public Transform rightFootObj = null;
	public Transform leftHandObj = null;
	public Transform rightHandObj = null;
	public Transform lookAtObj = null;
	
	public float leftFootWeightPosition = 1;
	public float leftFootWeightRotation = 1;

	public float rightFootWeightPosition = 1;
	public float rightFootWeightRotation = 1;
	
	public float leftHandWeightPosition = 1;
	public float leftHandWeightRotation = 1;
	
	public float rightHandWeightPosition = 1;
	public float rightHandWeightRotation = 1;

	public float lookAtWeight = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
		avatar = GetComponent<Animator>();
	}

/*	void OnGUI()
	{

		GUILayout.Label("Activate IK and move the Effectors around in Scene View");
		ikActive = GUILayout.Toggle(ikActive, "Activate IK");
	}*/
		
    
	void OnAnimatorIK(int layerIndex)
	{		
		if(avatar)
		{
			if(ikBodyActive && bodyObj != null)
			{
				avatar.bodyPosition = bodyObj.position;
				avatar.bodyRotation = bodyObj.rotation;
			}
            else if (bodyObj != null)
            {
                bodyObj.position = avatar.bodyPosition;
                bodyObj.rotation = avatar.bodyRotation;
            }

            if (ikLeftFootActive && leftFootObj != null)
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeightPosition);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeightRotation);
                avatar.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
                avatar.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
            }
            else
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
                if (leftFootObj != null)
                {
                    leftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
                    leftFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
                }
            }


            if (ikRightFootActive && rightFootObj != null)
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeightPosition);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeightRotation);
                avatar.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                avatar.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
            }
            else
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                if (rightFootObj != null)
                {
                    rightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
                    rightFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
                }
            }

            if (ikLeftHandActive && leftHandObj != null)
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeightPosition);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeightRotation);
                avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }
            else
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                if (leftHandObj != null)
                {
                    leftHandObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);
                    leftHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
                }
            }

            if (ikRightHandActive && rightHandObj != null)
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeightPosition);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeightRotation);
                avatar.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                avatar.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
            else
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                if (rightHandObj != null)
                {
                    rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
                    rightHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightHand);
                }
            }

            if (ikLookAtActive && lookAtObj != null)
			{
				avatar.SetLookAtPosition(lookAtObj.position);
                avatar.SetLookAtWeight(lookAtWeight, 0.3f, 0.6f, 1.0f, 0.5f);
            }
			else
			{
				avatar.SetLookAtWeight(0.0f);
				if(lookAtObj != null)
				{
					lookAtObj.position = avatar.bodyPosition + avatar.bodyRotation * new Vector3(0,0.5f,1);
				}		
			}
		}
	}  
}
