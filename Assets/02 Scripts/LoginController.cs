using UnityEngine;
using System.Collections;

public class LoginController : Photon.MonoBehaviour
{
    public static bool isHost = false;
    public static bool inRoom = false;
//    public static bool UIReady = false;

    public string LoadUIName;
    public string LoadUnitName;
    public string LoadWeaponName_LS;
    public string LoadWeaponName_RS;
    public string LoadWeaponName_LH;
    public string LoadWeaponName_RH;
    public string LoadWeaponName_BP;
    //NonPlayerObjecのプレハブ
    public string LoadObjectName;
    //NonPlayerObjecの出現ポイント
    public Transform[] ObjectSpawnPoints;
    //NonPlayerObjecの最大数
    public int MaxObjects = 12;

    private GameObject BattleUI;
    public static GameObject DollUnit;
    private GameObject Weapon_LS;
    private GameObject Weapon_RS;
    private GameObject Weapon_LH;
    private GameObject Weapon_RH;
    private GameObject Weapon_BP;

    public new PhotonView photonView;

    void Awake()
    {
        //Photon Realtimeサーバーに接続、ロビーに入室
        PhotonNetwork.ConnectUsingSettings("testLobby");
    }

    //ロビーに入室した
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入室しました");
        //いずれかのルームに入室
        PhotonNetwork.JoinRandomRoom();
    }

    //ルームの入室に失敗した
    void OnPhotonRandomJoinFailed()
    {
        isHost = true;
        //ルームを作成
        PhotonNetwork.CreateRoom("testRoom");
        Debug.Log("ルームを作成しました");
    }

    //ルームに入室した
    void OnJoinedRoom()
    {
        Debug.Log("ルームに入室しました");
        inRoom = true;

        if (PhotonNetwork.room.playerCount == 1)
            isHost = true;

        //Load Doll Unit
        if (LoginController.isHost)
        {
            DollUnit = PhotonNetwork.Instantiate(LoadUnitName, new Vector3(0, 0.1f, -60.0f), Quaternion.identity, 0);
            LoadNonPlayerObject();
        }
        else
        {
            DollUnit = PhotonNetwork.Instantiate(LoadUnitName, new Vector3(0, 0.1f, 60.0f), Quaternion.identity, 0);
        }

        //Load Battle UI
        if (LoadUIName != null)
        {
            BattleUI = Instantiate(Resources.Load(LoadUIName), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            GameManager.UIReady = true;
        }

        //Load Left Shoulder Weapon
        if (LoadWeaponName_LS != null)
        {
            GameObject parent_LS = DollUnit.GetComponentInChildren<AttachPoint_LS>().gameObject;
            Weapon_LS = PhotonNetwork.Instantiate(LoadWeaponName_LS, parent_LS.transform.position, Quaternion.identity, 0);
            int parentID = parent_LS.GetComponent<PhotonView>().viewID;
            int childID = Weapon_LS.GetComponent<PhotonView>().viewID;
            photonView.RPC("SetParental", PhotonTargets.AllBuffered, parentID, childID);
        }

        //Load Right Shoulder Weapon
        if (LoadWeaponName_RS != null)
        {
            GameObject parent_RS = DollUnit.GetComponentInChildren<AttachPoint_RS>().gameObject;
            Weapon_RS = PhotonNetwork.Instantiate(LoadWeaponName_RS, parent_RS.transform.position, Quaternion.identity, 0);
            int parentID = parent_RS.GetComponent<PhotonView>().viewID;
            int childID = Weapon_RS.GetComponent<PhotonView>().viewID;
            photonView.RPC("SetParental", PhotonTargets.AllBuffered, parentID, childID);
        }

        //Load Left Hand Weapon
        if (LoadWeaponName_LH != null)
        {
            GameObject parent_LH = DollUnit.GetComponentInChildren<AttachPoint_LH>().gameObject;
            Weapon_LH = PhotonNetwork.Instantiate(LoadWeaponName_LH, parent_LH.transform.position, Quaternion.identity, 0);
            int parentID = parent_LH.GetComponent<PhotonView>().viewID;
            int childID = Weapon_LH.GetComponent<PhotonView>().viewID;
            photonView.RPC("SetParental", PhotonTargets.AllBuffered, parentID, childID);
        }

        //Load Right Hand Weapon
        if (LoadWeaponName_RH != null)
        {
            GameObject parent_RH = DollUnit.GetComponentInChildren<AttachPoint_RH>().gameObject;
            Weapon_RH = PhotonNetwork.Instantiate(LoadWeaponName_RH, parent_RH.transform.position, Quaternion.identity, 0);
            int parentID = parent_RH.GetComponent<PhotonView>().viewID;
            int childID = Weapon_RH.GetComponent<PhotonView>().viewID;
            photonView.RPC("SetParental", PhotonTargets.AllBuffered, parentID, childID);
        }

        //Load BackPack Weapon
        if (LoadWeaponName_BP != null)
        {
            GameObject parent_BP = DollUnit.GetComponentInChildren<AttachPoint_BP>().gameObject;
            Weapon_BP = PhotonNetwork.Instantiate(LoadWeaponName_BP, parent_BP.transform.position, Quaternion.identity, 0);
            int parentID = parent_BP.GetComponent<PhotonView>().viewID;
            int childID = Weapon_BP.GetComponent<PhotonView>().viewID;
            photonView.RPC("SetParental", PhotonTargets.AllBuffered, parentID, childID);
        }

        if (!LoginController.isHost)
        {
            DollUnit.transform.Rotate(new Vector3(0, 180f, 0));
        }

        DollUnit.GetComponent<RPGCameraEx>().online = true;
        DollUnit.GetComponent<RPGCameraEx>().enabled = true;
    }

    void LoadNonPlayerObject()
    {
        ObjectSpawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        if (ObjectSpawnPoints == null)
            return;
        if (ObjectSpawnPoints.Length - 1 <= MaxObjects)
            MaxObjects = ObjectSpawnPoints.Length - 1;
        if (ObjectSpawnPoints.Length > 1)
        {
            bool[] indexs = new bool[ObjectSpawnPoints.Length];
            indexs[0] = true;
            for (int i = 1; i < ObjectSpawnPoints.Length; i++)
            {
                indexs[i] = false;
            }
            int cnt = 1;
            while (cnt < MaxObjects)
            {
                int i = Random.Range(1, ObjectSpawnPoints.Length - 1);
                if (!indexs[i])
                {
                    indexs[i] = true;
                    PhotonNetwork.Instantiate(LoadObjectName, ObjectSpawnPoints[i].position, ObjectSpawnPoints[i].rotation, 0);
                    cnt++;
                }
            }
        }
    }

    [PunRPC]
    public void SetParental(int parentID, int childID)
    {
        PhotonView parentView = PhotonView.Find(parentID);
        PhotonView childView = PhotonView.Find(childID);
        childView.transform.SetParent(parentView.transform);
        childView.transform.localPosition = new Vector3(0, 0, 0);
    }
}
