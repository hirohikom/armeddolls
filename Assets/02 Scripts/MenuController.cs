using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	private GameObject mainMenuPanel;
	private GameObject settingMenuPanel;
    private GameObject FadeMaskPanel;
	
	// Use this for initialization
	void Start () {
		GameManager.LoadedScene = true;
		mainMenuPanel = GameObject.Find ("MainMenuPanel");
        settingMenuPanel = GameObject.Find("SettingMenuPanel");
        FadeMaskPanel = GameObject.Find("FadeMaskPanel");
		mainMenuPanel.SetActive(true);
        settingMenuPanel.SetActive(false);
        FadeMaskPanel.SetActive(false);
	}
	
	public void ClickBattleButton()
    {
        FadeMaskPanel.SetActive(true);
        FadeMaskPanel.GetComponent<FadeandLoad>().LoadSceneName = "uTestArena_offline01";
        FadeMaskPanel.GetComponent<FadeandLoad>().enabled = true;
    }

	public void ClickMaintenanceButton()
	{
        FadeMaskPanel.SetActive(true);
        FadeMaskPanel.GetComponent<FadeandLoad>().LoadSceneName = "uLaboratory";
        FadeMaskPanel.GetComponent<FadeandLoad>().enabled = true;
	}

	public void ClickSettingButton()
	{
		mainMenuPanel.SetActive(false);
        settingMenuPanel.SetActive(true);
	}

	public void ClickQuitButton()
	{
		Application.Quit();
	}

	public void ClickReturn2MenuButton()
	{
        settingMenuPanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}

}
