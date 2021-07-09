using System.Collections;
using System.Collections.Generic;
using BackendlessAPI;
using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Persistence;
using UnityEngine;

public class hello : MonoBehaviour
{
    // データ型を＃C クラスで定義します
    class Contact: BackendlessEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
    }

    void Start ()
    {
        // オブジェクト生成します。
        Contact contact = new Contact
        {
            Name = "Jack Daniels",
            Age = 47,
            Phone = "0123-45-6789",
            Title = "Favorites"
        };

        // 非同期呼び出し完了時のコールバックメソッド
        AsyncCallback<Contact> callback = new AsyncCallback<Contact>(
        savedData =>
        {
            Debug.Log ("saved" + savedData.Name);
        },
        fault =>
        {
            Debug.Log (fault);
        });

        //上記コールバックメソッドを指定してデータ保存
        Backendless.Persistence.Of<Contact>().Save(contact, callback);
    }

    // Update is called once per frame
    void Update()
    {
    }
}