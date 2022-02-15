using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class NamePanel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI placeHolder;
    public Button enterButton;
    
    FirebaseUser user;
    FirebaseDatabase databaseInstance;

    // Start is called before the first frame update
    public void SetName()
    {
        user = AuthManager.firebaseAuth.CurrentUser;
        databaseInstance = AuthManager.DatabaseInstance;
        
        if (nameText.text != "")
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = nameText.text
                
            };

            user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    placeHolder.text = "error";
                    nameText.text = "";
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    placeHolder.text = "error";
                    nameText.text = "";
                    return;
                }

                Debug.Log("User display name updated successfully.");
                databaseInstance.RootReference.Child("users").Child(user.UserId).Child("username")
                    .SetValueAsync(nameText.text).ContinueWithOnMainThread(task2 =>
                    {
                        if (task2.IsCanceled)
                        {
                            Debug.LogError("UpdateUserProfileAsync was canceled.");
                            placeHolder.text = "error";
                            nameText.text = "";
                            return;
                        }

                        if (task2.IsFaulted)
                        {
                            Debug.LogError("UpdateUserProfileAsync encountered an error: " + task2.Exception);
                            placeHolder.text = "error";
                            nameText.text = "";
                            return;
                        }
                        Debug.Log("User database name updated successfully.");
                        gameObject.SetActive(false);
                        AuthManager.Instance.FinishMakeAccount();
                    });
                
                
            });


        }
    }
}
