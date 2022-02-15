using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Firebase.Extensions;
using UnityEngine;
using Firebase.Database;
using TMPro;


public class MoneyTag : MonoBehaviour
{
    private static MoneyTag _instance;
    private int money;
    
    public static MoneyTag Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<MoneyTag>();

            return _instance;
        }
    }

    public int Money
    {
        get => money;
        set
        {
            AuthManager.DatabaseInstance.GetReference("users").Child(AuthManager.User.UserId).Child("money")
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
                                .Child(AuthManager.User.UserId).Child("money")
                                .SetValueAsync(int.Parse(task.Result.Value.ToString()) + value);
                            Debug.Log("Add Money" + int.Parse(task.Result.Value.ToString()) + value);
                        }
                    });
        }
        
    }

    public TextMeshProUGUI text;
    void Start()
    {
        AuthManager.DatabaseInstance.GetReference("users").Child(AuthManager.User.UserId).Child("money")
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
                                .Child(AuthManager.User.UserId).Child("money")
                                .SetValueAsync(0);
                        }
                        else
                        {
                            money = int.Parse(snapshot.Value.ToString());
                            text.text = Money.ToString();
                        }

                        AuthManager.DatabaseInstance.GetReference("users")
                                .Child(AuthManager.User.UserId).Child("money").ValueChanged +=
                            MoneyValueChanged;
                    }
                });
    }

    private void Update()
    {
        text.text = Money.ToString();
    }

    void MoneyValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("change money");
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        money = int.Parse(args.Snapshot.Value.ToString());
    }
}
