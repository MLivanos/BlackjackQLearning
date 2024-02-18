using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoPlayer : BlackjackPlayer
{
    public override bool Hit(List<Card> cards, Card showing)
    {
        return GetValue(cards) < 17;
    }
}
