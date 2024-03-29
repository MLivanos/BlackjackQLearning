using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : BlackjackPlayer
{
    protected override void Start()
    {
        base.Start();
    }
    
    public override bool Hit(List<Card> cards, Card showing)
    {
        return GetValue(cards) < 21 && Random.value < 0.5f;
    }
}
