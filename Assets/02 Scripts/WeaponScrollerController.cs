using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

public class WeaponScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    
    private List<WeaponScrollerData> _data;

    public EnhancedScroller weaponScroller;

    public WeaponCellView weaponCellViewPrefab;

    void Start()
    {
        StartCoroutine("loadData");
    }

    IEnumerator loadData()
    {
        _data = new List<WeaponScrollerData>();

        for (int i = 0; i < 100; i++)
        {
            int maxB = (int)Random.Range(1.0f, 100.0f);
            int remainB = (int)Random.Range(1.0f, (float)maxB);

            _data.Add(new WeaponScrollerData()
            {
                weaponName = "SampleWeapon" + i.ToString(),
                weaponMaxBullets = maxB * 100,
                weaponRemainBullets = remainB * 50,
            });
        }

        weaponScroller.Delegate = this;
        weaponScroller.ReloadData();

        yield return null;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 80f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        WeaponCellView cellView = scroller.GetCellView(weaponCellViewPrefab) as WeaponCellView;
        cellView.SetData(_data[dataIndex]);
        return cellView;
    }
}
