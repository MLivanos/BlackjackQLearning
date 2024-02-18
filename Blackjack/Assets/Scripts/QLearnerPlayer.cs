using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnerPlayer : BlackjackPlayer
{
    private QTable qTable;
    [SerializeField] private float alpha;
    [SerializeField] private float gamma;
    [SerializeField] private float epsilon;
    private bool isTraining = true;

    public QLearnerPlayer()
    {
        isLearner = true;
    }
    
    public override bool Hit(List<Card> cards, Card showing)
    {
        return qTable.GetAction(cards, showing);;
    }
}
