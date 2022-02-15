using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;
using TMPro;

public class GemTag : MonoBehaviour
{
    private static GemTag _instance;
    private int gem;
    
    public static GemTag Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GemTag>();

            return _instance;
        }
    }

    public int Gem
    {
        get => gem;
        set
        {
            AuthManager.DatabaseInstance.GetReference("users").Child(AuthManager.User.UserId).Child("gem")
                .GetValueAsync().ContinueWithOnMainThread(
                    task =>
                    {
                        if (task.IsFaulted)
                        {
                            return;
                        }
                        else if (task.IsCompleted) {
                            
                            DataSnapshot snapshot = task.Result;
                            AuthManager.DatabaseInstance.GetReference("users")
                                .Child(AuthManager.User.UserId).Child("gem")
                                .SetValueAsync(int.Parse(task.Result.Value.ToString()) + value);
                            Debug.Log("Add Money" + int.Parse(task.Result.Value.ToString()) + value);
                        }
                    });
        }
        
    }

    public TextMeshProUGUI text;
    void Start()
    {
        AuthManager.DatabaseInstance.GetReference("users").Child(AuthManager.User.UserId).Child("gem")
            .GetValueAsync().ContinueWithOnMainThread(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    else if (task.IsCompleted) {
                        DataSnapshot snapshot = task.Result;
                        if (snapshot.Value == null)
                        {
                            AuthManager.DatabaseInstance.GetReference("users")
                                .Child(AuthManager.User.UserId).Child("gem")
                                .SetValueAsync(0);
                        }
                        else
                        {
                            gem = int.Parse(snapshot.Value.ToString());
                            text.text = Gem.ToString();
                        }

                        AuthManager.DatabaseInstance.GetReference("users")
                                .Child(AuthManager.User.UserId).Child("gem").ValueChanged +=
                            MoneyValueChanged;
                    }
                });
    }

    private void Update()
    {
        text.text = Gem.ToString();
    }

    void MoneyValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("change money");
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        gem = int.Parse(args.Snapshot.Value.ToString());
    }
}