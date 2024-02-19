using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackjackPlayer : MonoBehaviour
{
    public bool isLearner{get; protected set;}
    protected List<bool> actionHistory;
    protected List<float> rewardHistory;
    protected Card opponentShowing;
    protected List<List<Card>> stateHistory;
    protected List<List<Card>> nextStateHistory;

    protected virtual void Start()
    {
        ResetHistory();
    }

    public abstract bool Hit(List<Card> cards, Card showing);

    public void AddToHistory(bool action, float reward, List<Card> state, List<Card> newState)
    {
        actionHistory.Add(action);
        stateHistory.Add(state);
        nextStateHistory.Add(newState);
        rewardHistory.Add(reward);
    }

    public void ResetHistory()
    {
        actionHistory = new List<bool>();
        stateHistory = new List<List<Card>>();
        nextStateHistory = new List<List<Card>>();
        rewardHistory = new List<float>();
    }

    public void ModifyLastReward(float reward)
    {
        rewardHistory[rewardHistory.Count - 1] = reward;
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

    public void SetShowing(Card opponentCard)
    {
        opponentShowing = opponentCard;
    }

    public virtual void Train()
    {
        Debug.LogWarning("BlackjackPlayer.Train called without method being implemented");
        return;
    }

    public virtual void PrintTable()
    {
        
    }
}
