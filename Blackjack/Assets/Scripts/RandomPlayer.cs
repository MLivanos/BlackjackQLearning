using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : BlackjackPlayer
{
    public override bool Hit(int value, Card showing)
    {
        return value < 21 && Random.value < 0.5f;
    }
}
