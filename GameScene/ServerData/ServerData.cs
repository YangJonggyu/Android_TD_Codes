using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

[CreateAssetMenu(fileName = "ServerData", menuName = "Scriptable Object/ServerData")]
public class ServerData : ScriptableObject
{
    public SerializeDicStringInt damages;
    

    public void LoadData()
    {
        Debug.Log("load server data");
        AuthManager.DatabaseInstance.GetReference("game").Child("damage")
            .GetValueAsync().ContinueWithOnMainThread(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log("fail");
                        return;
                    }
                    else if (task.IsCompleted) {
                        Debug.Log("loading");
                        DataSnapshot snapshot = task.Result;
                        object value = snapshot.Value;
                        Dictionary<string, object> dic;
                        if (null != (value as IDictionary))
                        {
                            dic = (Dictionary<string,object>)snapshot.Value;
                        }
                        else
                        {
                            dic = new Dictionary<string,object>();
                            if (null != snapshot.Value) dic.Add(snapshot.Key, snapshot.Value);
                        }
                        Debug.Log("make serialize dic");
                        foreach (var data in dic)
                        {
                            damages[data.Key] = Int32.Parse(data.Value.ToString());
                        }

                    }
                });
    }
}
