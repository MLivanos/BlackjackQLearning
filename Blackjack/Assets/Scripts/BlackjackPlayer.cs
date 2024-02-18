using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackjackPlayer : MonoBehaviour
{
    protected bool isLearner;
    protected List<bool> actionHistory;
    protected List<int> valueHistory;

    protected void Start()
    {
        ResetHistory();
    }

    public abstract bool Hit(List<Card> cards, Card showing);

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

    public int GetValue(List<Card> cards)
    {
        int numberOfAces = 0;
        int value = 0;
        foreach(Card card in cards)
        {
            if(card.value == "A")
            {
                numberOfAces ++;
            }
            value += card.Value();
        }
        while(value > 21 && numberOfAces > 0)
        {
            value -= 10;
            numberOfAces --;
        }
        return value;
    }
}
