using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackGame : MonoBehaviour
{
    BlackjackPlayer[] players = new BlackjackPlayer[2];
    [SerializeField] BlackjackPlayer[] opponentPrefabs;
    [SerializeField] BlackjackPlayer player2;
    [SerializeField] UIManager uiManager;
    CardDisplay cardDisplay;
    QLearnerPlayer player1;
    Deck deck;
    bool isWaitingForHuman;
    int humanValue;
    int epochs = 100000;

    private void Start()
    {
        DontDestroyOnLoad(this);
        deck = new Deck();
        CreateQLearner();
        player1.ResetHistory();
        player2.ResetHistory();
        players[0] = player1;
        players[1] = player2;
    }

    private void Play()
    {
        List<Card> p1Cards = DrawHand();
        List<Card> p2Cards = DrawHand();
        int p1Value = PlayPlayer(0, p1Cards, p2Cards[0]);
        int p2Value = PlayPlayer(1, p2Cards, p1Cards[1]);
        int outcome = DetermineOutcome(p1Value, p2Value);
        float p1Reward = outcome == 1 ? 1.0f : outcome == 2 ? -1.0f : 0;
        float p2Reward = outcome == 1 ? -1.0f : outcome == 2 ? 1.0f : 0;
        players[0].ModifyLastReward(p1Reward);
        players[1].ModifyLastReward(p2Reward);
        TrainPlayer(players[0]);
        TrainPlayer(players[1]);
        players[0].ResetHistory();
        players[1].ResetHistory();
    }

    public IEnumerator PlayWithHuman()
    {
        List<Card> p1Cards = DrawHand();
        List<Card> p2Cards = DrawHand();
        isWaitingForHuman = true;
        StartCoroutine(DealPhysicalCards(p1Cards, p2Cards));
        while(isWaitingForHuman)
        {
            yield return null;
        }
        HumanPlayer human = player2 as HumanPlayer;
        StartCoroutine(PlayHuman(human, p2Cards, p1Cards[0]));
        while(isWaitingForHuman)
        {
            yield return null;
        }
        StartCoroutine(cardDisplay.FlipFaceDownCard());
        yield return new WaitForSeconds(0.5f);
        while(cardDisplay.isRotating)
        {
            yield return null;
        }
        int p1Value = PlayPlayer(0, p1Cards, p2Cards[0]);
        DisplayOutcome(p1Value, humanValue, p1Cards.Count, p2Cards.Count);
    }

    private IEnumerator DealPhysicalCards(List<Card> p1Cards, List<Card> p2Cards)
    {
        float cardWaitTime = 0.5f;
        DisplayCard(p2Cards[0], false, true);
        yield return new WaitForSeconds(cardWaitTime);
        DisplayCard(p2Cards[1], false, true);
        yield return new WaitForSeconds(cardWaitTime);
        DisplayCard(p1Cards[0], false, false);
        yield return new WaitForSeconds(cardWaitTime);
        DisplayCard(p1Cards[1], true, false);
        yield return new WaitForSeconds(cardWaitTime);
        isWaitingForHuman = false;
    }

    private void DisplayOutcome(int p1Value, int p2Value, int p1Cards, int p2Cards)
    {
        string message = "";
        if ((p1Value == 21 && p1Cards == 2) || p2Value == 21 && p2Cards == 2)
        {
            message = "Blackjack!\n";
        }
        if (p1Value > 21)
        {
            message += "Agent Busts!\n";
        }
        if (p2Value > 21)
        {
            message += "You Bust!\n";
        }
        int outcome = DetermineOutcome(p1Value, humanValue);
        switch (outcome)
        {
            case 0:
                message += "Push!";
                break;
            case 1:
                message += "Agent wins!";
                break;
            case 2:
                message += "You win!";
                break;
            default:
                break;
        }
        cardDisplay.DisplayMessage(message);
    }

    private IEnumerator PlayHuman(HumanPlayer human, List<Card> cards, Card showing)
    {
        isWaitingForHuman = true;
        humanValue = human.GetValue(cards);
        while(humanValue < 21)
        {
            human.Hit(cards, showing);
            while(!human.moveIsSet)
            {
                yield return null;
            }
            if(!human.move)
            {
                break;
            }
            Card newCard = deck.Draw();
            DisplayCard(newCard, false, true);
            cards.Add(newCard);
            humanValue = human.GetValue(cards);
        }
        humanValue = human.GetValue(cards);
        isWaitingForHuman = false;
    }

    private List<Card> DrawHand()
    {
        List<Card> playerHand = new List<Card>();
        playerHand.Add(deck.Draw());
        playerHand.Add(deck.Draw());
        return playerHand;
    }

    private void CreateQLearner()
    {
        gameObject.AddComponent<QLearnerPlayer>();
        player1 = gameObject.GetComponent<QLearnerPlayer>();
        if (uiManager == null)
        {
            return;
        }
        uiManager.InstantiateQLearner();
    }

    private void TrainPlayer(BlackjackPlayer player)
    {
        if (player.isLearner)
        {
            player.Train();
        }
    }

    private int DetermineOutcome(int p1Value, int p2Value)
    {
        bool p1Bust = p1Value > 21;
        bool p2Bust = p2Value > 21;
        int outcome = 0;
        if(p1Bust && !p2Bust)
        {
            outcome = 2;
        }
        else if(p2Bust && !p1Bust)
        {
            outcome = 1;
        }
        else if((!p1Bust && !p2Bust) && p1Value > p2Value)
        {
            outcome = 1;
        }
        else if((!p1Bust && !p2Bust) && p1Value < p2Value)
        {
            outcome = 2;
        }
        return outcome;
    }

    private int PlayPlayer(int playerIndex, List<Card> cards, Card showing)
    {
        players[playerIndex].SetShowing(showing);
        int currentValue = players[playerIndex].GetValue(cards);
        while(currentValue < 21 && players[playerIndex].Hit(cards, showing))
        {
            Card newCard = deck.Draw();
            DisplayCard(newCard, false, playerIndex==1);
            List<Card> oldHand = new List<Card>(cards);
            cards.Add(newCard);
            List<Card> newHand = new List<Card>(cards);
            currentValue = players[playerIndex].GetValue(cards);
            players[playerIndex].AddToHistory(true, 0.0f, oldHand, newHand);
        }
        if(currentValue <= 21)
        {
            players[playerIndex].AddToHistory(false, 0.0f, cards, null);
        }
        return currentValue;
    }

    private void DisplayCard(Card newCard, bool isFaceDown, bool isPlayerCard)
    {
        if (cardDisplay == null)
        {
            return;
        }
        cardDisplay.CreateCard(newCard, isFaceDown, isPlayerCard);
    }

    private float ClampHyperparameter(string hyperparameter, float minValue=0.0f, float maxValue=1.0f)
    {
        return Mathf.Clamp(float.Parse(hyperparameter), minValue, maxValue);
    }

    public void SetGamma(string gammaString)
    {
        player1.SetGamma(ClampHyperparameter(gammaString));
    }

    public void SetAlpha(string alphaString)
    {
        player1.SetAlpha(ClampHyperparameter(alphaString));
    }

    public void SetEpsilon(string epsilonString)
    {
        player1.SetEpsilon(ClampHyperparameter(epsilonString));
    }

    public void SetMinEpsilon(string minEpsilonString)
    {
        player1.SetEpsilon(ClampHyperparameter(minEpsilonString));
    }

    public void SetEpsilonDecay(string epsilonDecayString)
    {
        player1.SetEpsilonDecay(ClampHyperparameter(epsilonDecayString));
    }

    public void SetEpochs(string epochsString)
    {
        epochs = int.Parse(epochsString);
    }

    public void SetOpponent(int opponentIndex)
    {
        player2 = gameObject.AddComponent(opponentPrefabs[opponentIndex].GetType()) as BlackjackPlayer;
        players[1] = opponentPrefabs[opponentIndex];
    }

    public void Run()
    {
        uiManager.UpdateHyperparameters();
        deck = new Deck();
        for(int i=0; i<epochs+1; i++)
        {
            if (i%((int)(epochs/100)) == 0)
            {
                uiManager.StoreQTable();
            }
            Play();
        }
    }

    public void SetDisplay(CardDisplay display)
    {
        cardDisplay = display;
    }

    public void SetStateSpace(int qTableIndex)
    {
        player1.SetStateSpace(qTableIndex);
    }

    public void SeedQTable(float initialQValue)
    {
        player1.SeedQTable(initialQValue);
    }

    public void ClearQTable()
    {
        player1.ClearTable();
    }

    public BlackjackPlayer GetPlayer2()
    {
        return players[1];
    }
}
