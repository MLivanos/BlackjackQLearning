using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnerPlayer : BlackjackPlayer
{
    private QTable qTable;
    [SerializeField] private float alpha;
    [SerializeField] private float gamma;
    [SerializeField] private float epsilon;
    [SerializeField] private float minEpsilon;
    [SerializeField] private float epsilonDecay;
    private bool isTraining = true;

    public QLearnerPlayer()
    {
        qTable = new ValueShowingTable();
        isLearner = true;
    }

    protected override void Start()
    {
        base.Start();
    }
    
    public override bool Hit(List<Card> cards, Card showing)
    {
        if (Random.value < epsilon)
        {
            return Random.value < 0.5f;
        }
        return qTable.GetAction(cards, showing);
    }

    public void TrainEntry(List<Card> cards, List<Card> newHand, Card showing, int action, float reward)
    {
        if (!isTraining)
        {
            return;
        }
        float oldQValue = qTable.GetEntry(cards, showing, action);
        float argmaxQ = 0.0f;
        if (newHand != null && GetValue(newHand) <= 21)
        {
            argmaxQ = qTable.GetBestQValue(newHand, showing);
        }
        float newQValue = (1-alpha) * oldQValue + alpha * (reward + gamma * argmaxQ);
        qTable.SetEntry(cards, showing, action, newQValue);
    }

    public override void Train()
    {
        epsilon = Mathf.Max(epsilon*epsilonDecay, minEpsilon);
        for(int i=0; i<actionHistory.Count; i++)
        {
            int action = actionHistory[i] ? 0 : 1;
            float reward = rewardHistory[i];
            List<Card> state = stateHistory[i];
            List<Card> nextState = nextStateHistory[i];
            TrainEntry(state, nextState, opponentShowing, action, reward);
        }
    }

    public override void PrintTable()
    {
        qTable.PrintTable();
        Debug.Log("====");
    }
}
