using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardImage", menuName = "Scriptable Object/CardImage")]
public class CardImage : ScriptableObject
{
    public Sprite[] spades;
    public Sprite[] diamonds;
    public Sprite[] hearts;
    public Sprite[] clovers;
    public Sprite[] suits;
    
    public Color[] suitColor;
    
    public Sprite GetCardImage(int shape, int number)
    {
        switch (shape)
        {
            case 1:
                return spades[number];
                break;
            case 2:
                return diamonds[number];
                break;
            case 3:
                return hearts[number];
                break;
            case 4:
                return clovers[number];
                break;
        }

        return null;
    }

    public Sprite GetSuitImage(int suit)
    {
        return suits[suit];
    }

    public Color GetSuitColor(int suit)
    {
        return suitColor[suit];
    }

}
