using UnityEngine;
using System.Collections;

public class SpringManagerEx : MonoBehaviour
{
	public SpringBoneEx[] springBones;

    void Awake()
    {
        springBones = (SpringBoneEx[])gameObject.GetComponentsInChildren<SpringBoneEx>();
    }

	private void LateUpdate()
	{
		for (int i = 0; i < springBones.Length; i++)
		{
			springBones[i].UpdateSpring();
		}
	}
}
