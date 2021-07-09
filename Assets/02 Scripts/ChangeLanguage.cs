using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using I2.Loc;


public class ChangeLanguage : MonoBehaviour
{
    private Text lbl;
    private Localize lcz;
    private String txt;

    void Start()
    {
        lbl = GetComponentInChildren<Text>();
        lcz = GetComponent<Localize>();
    }
    public void Change()
    {
		//lbl.SetCurrentSelection();
        
		StartCoroutine ("ChangeLang");
	}

	IEnumerator ChangeLang()
	{
		switch (lbl.text)
        {
            case "Français": 
                lcz.SetGlobalLanguage("French");
                break;
            case "Español":
                lcz.SetGlobalLanguage("Spanish");
                break;
            case "Português":
                lcz.SetGlobalLanguage("Portuguese");
                break;
            case "Deutsche":
                lcz.SetGlobalLanguage("German");
                break;
            case "italiano":
                lcz.SetGlobalLanguage("Italian");
                break;
            case "русский":
                lcz.SetGlobalLanguage("Russian");
                break;
            case "Türk":
                lcz.SetGlobalLanguage("Turkish");
                break;
            case "العربية":
                lcz.SetGlobalLanguage("Arabic");
                break;
            case "简体中文":
                lcz.SetGlobalLanguage("Chinese (Simplified)");
                break;
            case "繁体中文":
                lcz.SetGlobalLanguage("Chinese (Traditional)");
                break;
            case "Tiếng Việt":
                lcz.SetGlobalLanguage("Vietnamese");
                break;
            case "हिन्दी":
                lcz.SetGlobalLanguage("Hindi");
                break;
            case "தமிழ் மொழி":
                lcz.SetGlobalLanguage("Tamil");
                break;
            case "한국어":
                lcz.SetGlobalLanguage("Korean");
                break;
            case "日本語":
                lcz.SetGlobalLanguage("Japanese");
                break;
            default:
                lcz.SetGlobalLanguage("English");
                break;
        }

		yield return null;
    }
}
