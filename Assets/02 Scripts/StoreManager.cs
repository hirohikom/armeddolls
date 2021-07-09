using UnityEngine;
using System.Collections;

public class StoreManager : MonoBehaviour {

    public GameObject[] UIButtons;
    public GameObject[] MoveButtons;
    private GameObject fadeMaskPanel;

    // Use this for initialization
    void Start()
    {
        GameManager.LoadedScene = true;
        fadeMaskPanel = GameObject.Find("FadeMaskPanel");
    }

    public void onBack()
    {
        fadeMaskPanel.GetComponent<FadeandLoad>().LoadSceneName = "uLaboratory";
        fadeMaskPanel.GetComponent<FadeandLoad>().enabled = true;
    }


}
