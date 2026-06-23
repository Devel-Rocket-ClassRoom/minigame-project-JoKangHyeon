using Cysharp.Threading.Tasks;
using Firebase.Auth;
using System;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour, IInitCheckable
{
    #region Singleton
    public static FirebaseAuthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("FirebaseAuthManager");
                _instance = gameObject.AddComponent<FirebaseAuthManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }
    private static FirebaseAuthManager _instance;
    #endregion

    private FirebaseAuth auth;

    private FirebaseUser currentUser;
    public FirebaseUser CurrentUser=>currentUser;

    public bool IsLoggedIn => currentUser != null;

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
        FirebaseManager.Instance.Assign(1, this).Forget();
    }

    public bool IsInited() => inited;

    public async UniTask<bool> Init()
    {
        auth = FirebaseManager.Instance.Auth;
        Debug.Log(auth);    
        currentUser = auth.CurrentUser;

        inited = true;
        return true;
    }

    public async UniTask<(bool success, string error)> SignInAnonymousAsync()
    {
        try
        {
            Debug.Log("[Auth] 익명 로그인 시작");

            AuthResult result = await auth.SignInAnonymouslyAsync();
            currentUser = result.User;

            Debug.Log("[Auth] 익명 로그인 완료");
            return (true, null);
        }
        catch(Exception ex)
        {
            Debug.LogError($"[Auth] 익명 로그인 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> SignInWithEmail(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] 이메일 로그인 시작");

            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;

            Debug.Log("[Auth] 이메일 로그인 종료");
            return (true, null);
        }
        catch(Exception ex)
        {
            Debug.LogError($"[Auth] 이메일 로그인 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(bool success, string error)> CreateUserWithEmail(string email, string password)
    {
        try
        {
            Debug.Log("[Auth] 이메일 가입 시작");

            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            currentUser = result.User;

            Debug.Log("[Auth] 이메일 가입 종료");
            return (true, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Auth] 이메일 가입 실패 : {ex.Message}");
            return (false, ex.Message);
        }
    }

    public void SignOut()
    {
        try
        {
            Debug.Log("[Auth] 로그아웃");
            auth.SignOut();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Auth] 로그아웃 실패 : {ex.Message}");
        }
    }
}
