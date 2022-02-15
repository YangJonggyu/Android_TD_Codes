using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCopyBug : MonoBehaviour
{
    public void BuyMoneyCopyBug()
    {
        MoneyTag.Instance.Money = 1;
        print("MoneyCopyBug");
    }
}
