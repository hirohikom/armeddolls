using UnityEngine;
using System.Collections;
using I2.Loc;

public class SetDefaultLanguage : MonoBehaviour {

	public string Language;
	
	// Use this for initialization
	void Awake () {
		GetComponent<Localize>().SetGlobalLanguage(Language);
	}
}
