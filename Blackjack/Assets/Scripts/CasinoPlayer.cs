using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoPlayer : BlackjackPlayer
{
    public override bool Hit(int value, Card showing)
    {
        return value < 17;
    }
}
