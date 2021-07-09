using UnityEngine;
using System;
using System.Collections;

public class Fade : MonoBehaviour {

	private Texture2D Tex;
	private float fA = 0;
	public enum Mode {Fadein, Fadeout};
	public Mode mode;
	public float FadeTime;
	public static bool FadeEnd;

	void Awake (){
		Tex = new Texture2D(32, 32, TextureFormat.RGB24, false);
//		Tex.ReadPixels (new Rect (0, 0, 32, 32), 0, 0, false);
		Tex.SetPixel (0, 0, Color.white);
		Tex.Apply ();
	}

	void Start ()
	{
		FadeEnd = false;
		FadeScreen (mode, FadeTime);
	}

	void OnGUI (){
		if (!FadeEnd){
			GUI.color = new Color (0, 0, 0, fA);
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Tex);
		} else {
			return;
		}
	}

	public bool FadeScreen(Mode m, float fTime){
		StartCoroutine (TransScene(m, fTime));
		return FadeEnd;
	}
		
	IEnumerator TransScene(Mode s, float ft){
		FadeEnd = false;
		if (s == Mode.Fadein){
			float t = 0;
			while (t <= ft) {
				fA = Mathf.Lerp (1f, 0f, t / ft);
				t += Time.deltaTime;
				yield return 0;
			}
		} else if (s == Mode.Fadeout){
			float t = 0;
			while (t <= ft) {
				fA = Mathf.Lerp (0f, 1f, t / ft);
				t += Time.deltaTime;
				yield return 0;
			}
		}
		FadeEnd = true;
	}
}