using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearnerPlayer : BlackjackPlayer
{
    private QTable qTable;
    private UIManager uiManager;
    [SerializeField] private float alpha;
    [SerializeField] private float gamma;
    [SerializeField] private float epsilon;
    [SerializeField] private float minEpsilon;
    [SerializeField] private float epsilonDecay;
    private bool isTraining = true;

    public void SetStateSpace(int stateSpaceType=0)
    {
        switch (stateSpaceType)
        {
            case 0:
                qTable = new ValueShowingTable();
                break;
            case 1:
                qTable = new ValueCardShowingTable();
                break;
            case 2:
                qTable = new ValueQTable();
                break;
            default:
                break;
        }
        isLearner = true;
    }

    protected override void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        base.Start();
        SetStateSpace();
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
        Debug.Log(alpha);
        Debug.Log(reward);
        Debug.Log(gamma);
        Debug.Log(newQValue);
        Debug.Log("======");
        qTable.SetEntry(cards, showing, action, newQValue);
        uiManager.UpdateCell(qTable.GetValue(cards) - 2, qTable.GetShowingIndex(showing), newQValue);
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

    public void SetGamma(float gamma_)
    {
        gamma = gamma_;
    }

    public void SetAlpha(float alpha_)
    {
        alpha = alpha_;
    }

    public void SetEpsilon(float epsilon_)
    {
        epsilon = epsilon_;
    }

    public void SetEpsilonDecay(float epsilonDecay_)
    {
        epsilonDecay = epsilonDecay_;
    }

    public void SetMinEpsilon(float minEpsilon_)
    {
        minEpsilon = minEpsilon_;
    }
}
