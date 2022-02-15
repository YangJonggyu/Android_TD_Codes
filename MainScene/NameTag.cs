using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;

public class NameTag : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    void Start()
    {
        text.text = AuthManager.firebaseAuth.CurrentUser.DisplayName;
    }

}
