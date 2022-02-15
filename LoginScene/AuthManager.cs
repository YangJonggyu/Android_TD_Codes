using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Firebase.Database;
using UpdateManager;

public class AuthManager : MonoBehaviourPunCallbacks
{
    public static AuthManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AuthManager>();

            return _instance;
        }
    }

    private static AuthManager _instance;

    public TextMeshProUGUI textbox;
    public GameObject namePanel;
    public GameObject errorPanel;

    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;
    public static Credential firebaseCredential;

    public static FirebaseUser User;
    public static FirebaseDatabase DatabaseInstance;

    public string authCode = null;

    public void Start()
    {
        Debug.Log("start app");
        

        textbox.text = "로그인 중";
        Debug.Log("firebase check");
        
        //파이어베이스 연동
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var result = task.Result;
                if(result != DependencyStatus.Available)
                {
                    Debug.LogError(result.ToString());
                    IsFirebaseReady = false;
                }
                else
                {
                    IsFirebaseReady = true;

                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;

                }
                if(firebaseAuth.CurrentUser != null) 
                {
                    Debug.LogFormat("Firebase User signed in successfully: {0} ({1})", firebaseAuth.CurrentUser.DisplayName, firebaseAuth.CurrentUser.UserId);
                    
                }
                else
                {
                    Debug.Log("new login");
                    //namePanel.SetActive(true);
                }
#if UNITY_EDITOR
                AnonymouslySignIn();
#else
                GooglePlaySignIn();
#endif
            }
        
        );
    }

    public void GooglePlaySignIn()
    {
        errorPanel.SetActive(false);
        //구글 플레이 연동
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /* Don't force refresh */)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        
        
        Debug.Log("google play sign in start");
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                Debug.Log("Authcode : " + authCode);
                firebaseCredential = PlayGamesAuthProvider.GetCredential(authCode);
                string error = "";
                firebaseAuth.SignInWithCredentialAsync(firebaseCredential).ContinueWith(task => {
                    if (task.IsCanceled) {
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        error = "SignInWithCredentialAsync was canceled.";
                        errorPanel.SetActive(true);
                    }
                    if (task.IsFaulted) {
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        error = "SignInWithCredentialAsync encountered an error: " + task.Exception;
                        errorPanel.SetActive(true);
                    }

                    User = task.Result;

                    Debug.LogFormat("Google Play User signed in successfully: {0} ({1})",
                        User.DisplayName, User.UserId);
                    MakeAcount();
            
                }); 
            }
        });

        

    }   

    public void AnonymouslySignIn()
    {
        Debug.Log("AnonymouslySignIn start");
        firebaseAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            User = task.Result;

            Debug.LogFormat("Anonymous User signed in successfully: {0} ({1})", User.DisplayName, User.UserId);
            MakeAcount();

            });


    }

    public void MakeAcount()
    {
        DatabaseInstance = FirebaseDatabase.DefaultInstance;
        
        DatabaseInstance.GetReference("users").Child(User.UserId).Child("username").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                MakeAcount();
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot);
                if (snapshot.Value == null)
                {
                    Debug.Log("new acount");
                    namePanel.SetActive(true);

                }
                else
                {
                    StartCoroutine(LoadMainScene());
                }
            }
        });

    }
    public IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(1f);
        textbox.text = "데이터 확인 중";
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Scene");
    }

    public void FinishMakeAccount()
    {
        StartCoroutine(LoadMainScene());
    }
    

}