using UnityEngine;
using System.Collections;

public class WeaponScrollerData : MonoBehaviour 
{
    public int weaponIndex;

    public enum weaponPlace {Shoulder, Hand, BackPack};

    public string weaponName;

    public GameObject weaponPrefab;

    public int weaponMaxBullets;

    public int weaponRemainBullets;

    public float weaponBulletPower;

    public float weaponFireRapid;

    public float weaponPrice;

    public float weaponBulletPrice;

}
