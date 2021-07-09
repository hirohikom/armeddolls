using UnityEngine;
using System.Collections;

public class ChangeTag : Photon.MonoBehaviour {

    public new PhotonView photonView;

	// Use this for initialization
	void Start ()
    {
        if (photonView.isMine)
        {
            this.tag = ("Player");
        }
        else
        {
            this.tag = ("Enemy");
        }

    }
}
