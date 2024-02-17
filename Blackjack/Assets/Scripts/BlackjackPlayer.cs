using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackjackPlayer : MonoBehaviour
{
    bool isLearner;
    List<bool> actionHistory;
    List<int> valueHistory;

    private void Start()
    {
        ResetHistory();
    }

    public abstract bool Hit(int value, Card showing);

    public void AddToHistory(bool action, int value)
    {
        actionHistory.Add(action);
        valueHistory.Add(value);
    }

    public void ResetHistory()
    {
        actionHistory = new List<bool>();
        valueHistory = new List<int>();
    }
}
