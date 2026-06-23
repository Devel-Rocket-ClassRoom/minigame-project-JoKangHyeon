using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;


public class FirebaseDatabaseManager : MonoBehaviour, IInitCheckable
{
    #region Singleton
    public static FirebaseDatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("FirebaseDatabaseManager");
                _instance = gameObject.AddComponent<FirebaseDatabaseManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }
    private static FirebaseDatabaseManager _instance;
    #endregion

    private FirebaseDatabase database;

    private bool inited = false;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }


    private void Start()
    {
        FirebaseManager.Instance.Assign(2, this).Forget();
    }

    public bool IsInited() => inited;

    public async UniTask<bool> Init()
    {
        database = FirebaseManager.Instance.Database;

        inited = true;
        return true;
    }

    public async UniTask<DataSnapshot> LoadData(string path) 
    {
        DataSnapshot data = await database.RootReference.Child(path).GetValueAsync();
        return data;
    }

    public async UniTask<(bool success, string error)> SaveData(string path, string data, bool isRaw=false, bool push = false)
    {
        try
        {
            Debug.Log("[Database] 저장 시작");
            DatabaseReference reference = database.RootReference.Child(path);
            
            if (push)
            {
                reference = reference.Push();
            }

            if (isRaw)
            {
                await reference.SetRawJsonValueAsync(data);
            }
            else
            {
                await reference.SetValueAsync(data);
            }

            Debug.Log("[Database] 저장 완료");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Database] 저장 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> UpdateData(string path, Dictionary<string,object> data)
    {
        try
        {
            Debug.Log("[Database] 업데이트 시작");
            DatabaseReference reference = database.RootReference.Child(path);

            await reference.UpdateChildrenAsync(data);

            Debug.Log("[Database] 업데이트 완료");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Database] 저장 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }


    public async UniTask<DataSnapshot> LoadWithUserId(string path)
    {
        string pathWithUid = string.Format(path, FirebaseAuthManager.Instance.CurrentUser.UserId);
        return await LoadData(pathWithUid);
    }

    public async UniTask<(bool success, string error)> SaveWithUserId(string path, string data, bool isRaw = false)
    {
        string pathWithUid = string.Format(path, FirebaseAuthManager.Instance.CurrentUser.UserId);
        return await SaveData(pathWithUid, data, isRaw);
    }

    public async UniTask<(bool success, string error)> UpdateWithUserId(string path, Dictionary<string, object> data)
    {
        string pathWithUid = string.Format(path, FirebaseAuthManager.Instance.CurrentUser.UserId);
        return await UpdateData(pathWithUid, data);

    }

}
