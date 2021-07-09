using UnityEngine;
using System.Collections;

public class ShowLogo : MonoBehaviour {

    private CanvasGroup canvasGroup;
    public bool Fade= true;
	public float FadeInTime = 1.5f;
    public float KeepTime = 3.0f;
    public float FadeOutTime = 1.5f;
    public string NextScene;
    private float pastTime;
    private int stage;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (Fade)
            canvasGroup.alpha = 0;
        else
            canvasGroup.alpha = 1.0f;
        pastTime = 0;
        stage = 0;
    }

    void Update()
    {
        if (Fade)
        {
            switch (stage)
            {
                case 0:
                    if (pastTime <= FadeInTime)
                    {
                        canvasGroup.alpha = pastTime / FadeInTime;
                        pastTime += Time.deltaTime;
                    }
                    else
                    {
                        canvasGroup.alpha = 1.0f;
                        pastTime = 0;
                        stage = 1;
                    }
                    break;
                case 1:
                    StartCoroutine("WaitTime");
                    break;
                case 2:
                    if (pastTime <= FadeOutTime)
                    {
                        canvasGroup.alpha = 1.0f - (pastTime / FadeInTime);
                        pastTime += Time.deltaTime;
                    }
                    else
                    {
                        canvasGroup.alpha = 0;
                        stage = 3;
                    }
                    break;
            }
        }
        else
        {
             StartCoroutine("WaitTime");
 
        }
        if (stage == 3)
        {
            StartCoroutine("LoadScene");
            stage = 99;
        }
    }

	IEnumerator LoadScene()
	{
		Application.LoadLevelAsync(NextScene);
		yield return null;
	}

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(KeepTime);
        pastTime = 0;
        if (Fade)
            stage = 2;
        else
            stage = 3;
    }
}
