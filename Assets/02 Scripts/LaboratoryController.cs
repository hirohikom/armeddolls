using UnityEngine;
using System;
using System.Collections;
using I2.Loc;
using UnityEngine.UI;

public class LaboratoryController : MonoBehaviour
{

	public GameObject[] ActiveButtons;
    public GameObject[] WeaponScrollViews;
    public GameObject[] WeaponScrollBars;
    private int WeaponScrollViewsLastIndex = 0;
    private int WeaponScrollBarsLastIndex = 0;
    private bool SelectedDoll = false;
    private GameObject SelectDollBtn;
	private bool MenuListOn = false;
	private GameObject EasyTouchObj;
    private GameObject BasePanel;
    private GameObject ArrowPanel;
    private GameObject MaintenancePanel;

    void Awake()
    {
        GameManager.LoadedScene = true;
    }
    
	void Start () 
	{
        EasyTouchObj = GameObject.Find("EasyTouch");
        SelectDollBtn = GameObject.Find("SelectDollButton");
        BasePanel = GameObject.Find("BasePanel");
        ArrowPanel = GameObject.Find("ArrowPanel");
        MaintenancePanel = GameObject.Find("MaintenancePanel");

        foreach (GameObject obj in WeaponScrollViews)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in WeaponScrollBars)
        {
            obj.SetActive(false);
        }
        ArrowPanel.SetActive(true);
        MaintenancePanel.SetActive(false);
    }

    public void onSelectDoll()
    {
        SelectDoll(SelectDollBtn);
	}

    public void onMoveLeft()
	{
        LaboUnselectedCamera.PosNumber++;
        if (LaboUnselectedCamera.PosNumber > 7)
        LaboUnselectedCamera.PosNumber = 0;
        LaboUnselectedCamera._mouseX = LaboUnselectedCamera.MovingAngle * LaboUnselectedCamera.PosNumber;
    }

    public void onMoveRight()
	{
        LaboUnselectedCamera.PosNumber--;
        if (LaboUnselectedCamera.PosNumber < 0)
            LaboUnselectedCamera.PosNumber = 7;
        LaboUnselectedCamera._mouseX = LaboUnselectedCamera.MovingAngle * LaboUnselectedCamera.PosNumber;
    }

    public void onGemStore()
    {
        GameObject mask = GameObject.Find("FadeMaskPanel");
        mask.GetComponent<FadeandLoad>().LoadSceneName = "uGemStore";
        mask.GetComponent<FadeandLoad>().enabled = true;
    }

    public void onReturnMainMenu()
    {
        GameObject mask = GameObject.Find("FadeMaskPanel");
        mask.GetComponent<FadeandLoad>().LoadSceneName = "uMainMenu";
        mask.GetComponent<FadeandLoad>().LoadingIndicator = false;
        mask.GetComponent<FadeandLoad>().enabled = true;
	}

    public void onMenuListAppear00()
    {
        MenuListAppearance(ActiveButtons[0]);
    }

    public void onMenuListAppear01()
    {
        MenuListAppearance(ActiveButtons[1]);
    }

    public void onMenuListAppear02()
    {
        MenuListAppearance(ActiveButtons[2]);
    }

    public void onMenuListAppear03()
    {
        MenuListAppearance(ActiveButtons[3]);
    }

    public void onMenuListAppear04()
    {
        MenuListAppearance(ActiveButtons[4]);
    }

    public void onMenuListAppear05()
    {
        MenuListAppearance(ActiveButtons[5]);
    }

	private void SelectDoll(GameObject SelectButton)
	{
		SelectedDoll = !SelectedDoll;

		Text txt = SelectButton.GetComponentInChildren<Text> ();

        LaboSelectedCamera._mouseXSmooth = 0;
        LaboSelectedCamera._mouseYSmooth = 0;
        LaboSelectedCamera._mouseX = 0;
        LaboSelectedCamera._desiredMouseY = 0;
        

        if (SelectedDoll) 
		{
			Camera.main.GetComponent<LaboUnselectedCamera>().enabled = false;
			Camera.main.GetComponent<LaboSelectedCamera>().enabled = true;

            txt.GetComponent<Localize>().SetTerm("Select Other Doll", null);

            foreach (GameObject obj in WeaponScrollBars)
            {
                obj.SetActive(false);
            }
            ArrowPanel.SetActive(false);
            MaintenancePanel.SetActive(true);
        }
		else 
		{
			Camera.main.GetComponent<LaboUnselectedCamera>().enabled = true;
			Camera.main.GetComponent<LaboSelectedCamera>().enabled = false;

			txt.GetComponent<Localize>().SetTerm("Select This Doll", null);

            ArrowPanel.SetActive(true);
            MaintenancePanel.SetActive(false);
        }

        LaboSelectedCamera.TargetArray = null;
	}

	private void MenuListAppearance(GameObject activeButton)
	{
		MenuListOn = !MenuListOn;

        for (int i = 0; i < ActiveButtons.Length; i++ )
        {
            if (ActiveButtons[i] != activeButton && MenuListOn)
            {
                EasyTouchObj.SetActive(false);
                ActiveButtons[i].SetActive(false);
            }
            else if (ActiveButtons[i] != activeButton && !MenuListOn)
            {
                EasyTouchObj.SetActive(true);
                ActiveButtons[i].SetActive(true);
            }
            else if (ActiveButtons[i] == activeButton && i < WeaponScrollViews.Length)
            {
                WeaponScrollViews[WeaponScrollViewsLastIndex].SetActive(false);
                WeaponScrollViews[i].SetActive(MenuListOn);
                WeaponScrollViewsLastIndex = i;
                WeaponScrollBars[WeaponScrollBarsLastIndex].SetActive(false);
                WeaponScrollBars[i].SetActive(MenuListOn);
                WeaponScrollBarsLastIndex = i;
            }
        }

        BasePanel.SetActive(!MenuListOn);

/*        if (activeButton.name != "DollStatesButton")
        {
            WeaponScroller.SetActive(MenuListOn);
            float AspectWidth = Screen.width / 1280f;
            float AspectHeight = Screen.height / 720f;
            Vector3 pos = activeButton.GetComponent<RectTransform>().position;
            float BtnWidth = activeButton.GetComponent<Image>().GetComponent<RectTransform>().rect.width;
            float BtnHeight = activeButton.GetComponent<Image>().GetComponent<RectTransform>().rect.height;
            float WsclWidth = WeaponScroller.GetComponent<Image>().GetComponent<RectTransform>().rect.width;
            float WsclHeight = WeaponScroller.GetComponent<Image>().GetComponent<RectTransform>().rect.height;
            float posX = pos.x - ((BtnWidth - WsclWidth) / 2.0f * AspectWidth);
            float posY = pos.y - ((BtnHeight + WsclHeight) / 2.0f * AspectHeight);
            WeaponScroller.GetComponent<RectTransform>().position = new Vector3(posX, posY, 0);
        }*/
    }


}
