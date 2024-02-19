using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackGame : MonoBehaviour
{
    [SerializeField] BlackjackPlayer[] players;
    Deck deck;
    float nWins = 0.0f;

    private void Start()
    {
        deck = new Deck();
        players[0].ResetHistory();
        players[1].ResetHistory();
        for(int i=0; i<200000; i++)
        {
            Play();
            if (i%50000 == 0 && i>0)
            {
                Debug.Log(nWins/1000);
                nWins = 0.0f;
                players[0].PrintTable();
            }
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown("g"))
        {
            Play();
        }
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
        if (outcome == 1)
        {
            nWins ++;
        }
        players[0].ResetHistory();
        players[1].ResetHistory();
    }

    private List<Card> DrawHand()
    {
        List<Card> playerHand = new List<Card>();
        playerHand.Add(deck.Draw());
        playerHand.Add(deck.Draw());
        return playerHand;
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
            //Debug.Log("P1 Busts, P2 Wins");
        }
        else if(p2Bust && !p1Bust)
        {
            outcome = 1;
            //Debug.Log("P2 Busts, P1 Wins");
        }
        else if(p1Bust && p2Bust)
        {
            //Debug.Log("Both players bust! Push");
        }
        else if(p1Value > p2Value)
        {
            outcome = 1;
            //Debug.Log("P1 Wins");
        }
        else if(p1Value < p2Value)
        {
            outcome = 2;
            //Debug.Log("P2 Wins");
        }
        else
        {
            //Debug.Log("Push");
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
        //PrintAllCards(cards);
        return currentValue;
    }

    private void PrintAllCards(List<Card> cards)
    {
        Debug.Log("=======");
        foreach(Card card in cards)
        {
            Debug.Log(card.value);
        }
    }
}
