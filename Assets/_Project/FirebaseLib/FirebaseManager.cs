using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum InitState
{
    None,
    Initiating,
    Inited,
}

public class FirebaseManager : MonoBehaviour, IInitCheckable
{
    #region Singleton
    public static FirebaseManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject gameObject = new GameObject("FirebaseManager");
                _instance = gameObject.AddComponent<FirebaseManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }
    private static FirebaseManager _instance;
    #endregion

    public Dictionary<int, List<IInitCheckable>> initTargets = new();
    InitState initState = InitState.None;

    public FirebaseApp App { get; private set; }
    public FirebaseDatabase Database { get; private set; }
    public FirebaseAuth Auth { get; private set; }

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
    
    public async UniTaskVoid Start()
    {
        Debug.Log("init start");
        await UniTask.NextFrame();

        Init().Forget();
    }

    public bool IsInited()
    {
        return initState==InitState.Inited;
    }

    public async UniTaskVoid Assign(int priority, IInitCheckable target)
    {
        if (initState!=InitState.None)
        {
            await UniTask.WaitUntil(IsInited);
            target.Init().Forget();
            return;
        }

        if(!initTargets.ContainsKey(priority))
        {
            initTargets.Add(priority, new List<IInitCheckable>());
        }

        initTargets[priority].Add(target);
    }

    public async UniTask<bool> Init()
    {
        initState = InitState.Initiating;
        //Init this

        try
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if(status != DependencyStatus.Available)
            {
                Debug.LogError($"의존성 오류 : {status}");
                return false;
            }

            App = FirebaseApp.DefaultInstance;
            Database = GetDatabase(App);
            Auth = FirebaseAuth.GetAuth(App);
        }catch(Exception ex)
        {
            Debug.LogError($"초기화 오류 : {ex.Message}");
            return false;
        }

        //Init Others
        List<int> keys = initTargets.Keys.ToList();
        keys.Sort();

        for(int i=0; i< keys.Count; i++)
        {
            List<UniTask> currentPriorityTasks = new();

            foreach(IInitCheckable initTarget in initTargets[keys[i]])
            {
                currentPriorityTasks.Add(initTarget.Init());
            }

            await UniTask.WhenAll(currentPriorityTasks);
        }

        initState = InitState.Inited;
        return true;
    }

    private FirebaseDatabase GetDatabase(FirebaseApp app)
    {
        FirebaseConfig config = Resources.Load<FirebaseConfig>("FirebaseConfig");
        if (config != null && !string.IsNullOrEmpty(config.databaseUrl))
        {
            return FirebaseDatabase.GetInstance(app, config.databaseUrl);
        }
        return FirebaseDatabase.GetInstance(app);
    }

}
