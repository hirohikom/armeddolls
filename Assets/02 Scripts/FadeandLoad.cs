using UnityEngine;
using System;
using System.Collections;

public class FadeandLoad : MonoBehaviour {

    private CanvasGroup canvasGroup;
    private float pastTime;
	public float FadeTime;
	public string LoadSceneName;
    public bool LoadingIndicator;
    private bool loadable;

	void Awake (){
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        pastTime = 0;
        loadable = true;
	}

	void Update ()
	{
        if (pastTime <= FadeTime)
        {
            canvasGroup.alpha = pastTime / FadeTime;
            pastTime += Time.deltaTime;
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            if (loadable)
            {
                StartCoroutine("LoadScene");
                loadable = false;
            }
        }
    }

	IEnumerator LoadScene()
    {
        if (LoadingIndicator)
        {
            GameManager.LoadedScene = false;
            Application.LoadLevelAsync("uLoading");
            Application.LoadLevelAdditiveAsync(LoadSceneName);
        }
        else
        {
            Application.LoadLevelAsync(LoadSceneName);
        }
        yield return null;
	}
}