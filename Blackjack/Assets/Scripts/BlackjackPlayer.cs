using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackjackPlayer : MonoBehaviour
{
    bool isLearner;
    bool[] actionHistory;
    int[] valueHistory;

    public abstract bool Play(int value, Card showing);
}
