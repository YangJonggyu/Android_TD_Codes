using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCopyBug : MonoBehaviour
{
    public void BuyGemCopyBug()
    {
        GemTag.Instance.Gem = 1;
        print("GemCopyBug");
    }
}
