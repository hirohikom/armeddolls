using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

public class LanguageCellView : EnhancedScrollerCellView
{
    public Text languageNameText;

    public void SetData(ScrollerData data)
    {
        languageNameText.text = data.languageName;
    }
}