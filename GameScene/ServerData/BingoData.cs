using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBingoData
{
    int Damage(int damage);
}

public abstract class BingoData : IBingoData
{
    public abstract int Damage(int damage);

    public void GetDataFromServer()
    {
        
    }
}

public class BingoEffectList : BingoData
{
    public Rank[] RankList;

    public void SetBingoEffectList(int length)
    {
        RankList = new Rank[length];
    }

        public override int Damage(int damage)
    {
        return 0;
    }
}