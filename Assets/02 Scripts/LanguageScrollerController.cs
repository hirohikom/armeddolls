using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

public class LanguageScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{

    private List<ScrollerData> _data;

    public EnhancedScroller langScroller;

    public LanguageCellView languageCellViewPrefab;

	void Start () 
	{
		loadData ();
	}
	
	void loadData () 
	{
        _data = new List<ScrollerData>();

        _data.Add(new ScrollerData() { languageName = "English" });
        _data.Add(new ScrollerData() { languageName = "Français" });
        _data.Add(new ScrollerData() { languageName = "Español" });
        _data.Add(new ScrollerData() { languageName = "Português" });
        _data.Add(new ScrollerData() { languageName = "Deutsche" });
        _data.Add(new ScrollerData() { languageName = "italiano" });
        _data.Add(new ScrollerData() { languageName = "русский" });
        _data.Add(new ScrollerData() { languageName = "Türk" });
        _data.Add(new ScrollerData() { languageName = "العربية" });
        _data.Add(new ScrollerData() { languageName = "简体中文" });
        _data.Add(new ScrollerData() { languageName = "繁体中文" });
        _data.Add(new ScrollerData() { languageName = "Tiếng Việt" });
        _data.Add(new ScrollerData() { languageName = "हिन्दी" });
        _data.Add(new ScrollerData() { languageName = "தமிழ் மொழி" });
        _data.Add(new ScrollerData() { languageName = "한국어" });
        _data.Add(new ScrollerData() { languageName = "日本語" });

        langScroller.Delegate = this;
        langScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 60f;
    }
    
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        LanguageCellView cellView = scroller.GetCellView(languageCellViewPrefab) as LanguageCellView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }
}
