using UnityEngine;
using System.Collections;

public class displayFPS : MonoBehaviour {

	public bool onDisplay;
	private float fps;
//	private string gt;
	private int n;

	// Use this for initialization
	void Start () {
//		this.guiText.fontSize = (int)(Screen.height / 20.0f);
//		this.guiText.pixelOffset = new Vector2 (Screen.width / -2.0f + 10.0f, Screen.height/ 2.0f - 10.0f);
		n = 0;
		fps = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//FPS表示（for debug）
		if (onDisplay){
			if (n < 10){
				fps += Time.deltaTime;
				n++;
			} else {
				fps = 10.0f / fps;
//				gt = "FPS:" + fps;
//				this.guiText.text = gt;
				n = 0;
				fps = 0;
			}
		}
	}
}
