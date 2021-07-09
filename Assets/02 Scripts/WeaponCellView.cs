using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

public class WeaponCellView : EnhancedScrollerCellView
{
    public Text weaponNameText;

    public Text weaponBulletsStateText;

    public void SetData(WeaponScrollerData data)
    {
        weaponNameText.text = data.weaponName;

        weaponBulletsStateText.text = data.weaponRemainBullets.ToString() + " / " + data.weaponMaxBullets.ToString();
    }
}
