using UnityEngine;
using System.Collections;

public class NowLoading : MonoBehaviour {

    GameObject obj;
    
    // Use this for initialization
	void Start () {

        obj = GameObject.Find("LoadingUIRoot");
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (GameManager.LoadedScene)
        {
            Destroy(obj);
        }
	
	}
}
