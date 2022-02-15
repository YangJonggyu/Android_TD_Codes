using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameTest : MonoBehaviour
{

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AuthManager.User != null)
        {
            text.text = AuthManager.User.DisplayName;
        }
        
    }
}
