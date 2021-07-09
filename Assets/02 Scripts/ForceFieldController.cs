using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class ForceFieldController : Photon.MonoBehaviour {

    public bool online = true;

    public float ForceFieldMaxStrength = 100.0f;
    public float ForceFieldNowStrength = 100.0f;
    private Image ForceFieldSprite;

    public float E_ForceFieldMaxStrength = 100.0f;
    public float E_ForceFieldNowStrength = 100.0f;
    private Image E_ForceFieldSprite;

    private float fillRate;
    private float E_fillRate;

    private float damage;

    public new PhotonView photonView;

    // Use this for initialization
    void Start () {
        if (!online || photonView.isMine)
        {
            ForceFieldSprite = GameObject.Find("ForceFieldMeter").GetComponent<Image>();
            ForceFieldSprite.fillAmount = ForceFieldNowStrength / ForceFieldMaxStrength;
        }
        else
        {
            E_ForceFieldSprite = GameObject.Find("E_ForceFieldMeter").GetComponent<Image>();
            E_ForceFieldSprite.fillAmount = E_ForceFieldNowStrength / E_ForceFieldMaxStrength;
        }
    }

    // ダメージ処理
    // param[0]:Vector3　発射地点
    // param[1]:Vector3　着弾地点
    // param[2]:Vector3　着弾地点の法線
    // param[3]:float　ダメージ量
    void OnDamage(object[] param)
    {
        if (param == null)
            return;
        if (param[0] == null || param[1] == null || param[2] == null)
        {
            damage = (float)param[3];
        }
        else
        {
            float hitAngle = Vector3.Angle(((Vector3)param[1] - (Vector3)param[0]), (Vector3)param[2]);
            if (hitAngle > 90f)
                return;
            damage = (float)param[3] * (90f - hitAngle) / 90f;
            if (damage < 0)
                return;
        }
        if (online)
            photonView.RPC("AddDamage", PhotonTargets.All, damage);
        else
            AddDamage(damage);
    }

    void Update()
    {
        if (!online || photonView.isMine)
        {
            if (ForceFieldNowStrength <= 0)
            {
                if (!PlayerController.isDown)
                {
                    if (online)
                        photonView.RPC("FFzeroDown", PhotonTargets.All);
                    else
                        FFzeroDown();
                    PlayerController.isDown = true;
                }
                ForceFieldNowStrength = 0;
                fillRate = 0;
                ForceFieldSprite.fillAmount = 0;
            }
            else
            {
                fillRate = ForceFieldNowStrength / ForceFieldMaxStrength;
                ForceFieldSprite.fillAmount = fillRate;
                ForceFieldSprite.color = new Color(1.0f - fillRate, fillRate, 0);
            }
        }
        else
        {
            if (E_ForceFieldNowStrength <= 0)
            {
                if (!PlayerController.isDown)
                {
                    if (online)
                        photonView.RPC("FFzeroDown", PhotonTargets.All);
                    else
                        FFzeroDown();
                    PlayerController.isDown = true;
                }
                E_ForceFieldNowStrength = 0;
                E_fillRate = 0;
                E_ForceFieldSprite.fillAmount = 0;
            }
            else
            {
                E_fillRate = E_ForceFieldNowStrength / E_ForceFieldMaxStrength;
                E_ForceFieldSprite.fillAmount = E_fillRate;
                E_ForceFieldSprite.color = new Color(1.0f - E_fillRate, E_fillRate, 0);
            }
        }
    }

    [PunRPC]
    public void AddDamage(float damage)
    {
        if (!online || photonView.isMine)
        {
            ForceFieldNowStrength -= damage;
        }
        else
        {
            E_ForceFieldNowStrength -= damage;
        }
    }

    [PunRPC]
    public void FFzeroDown()
    {
        String unittag;
        if (!online || photonView.isMine)
        {
            unittag = "Player";
        }
        else
        {
            unittag = "Enemy";
        }
        GameObject Unit = GameObject.FindGameObjectWithTag(unittag);
        Unit.GetComponent<CharacterController>().enabled = false;
        Unit.GetComponent<Animator>().SetBool("Down", true);
    }
}
