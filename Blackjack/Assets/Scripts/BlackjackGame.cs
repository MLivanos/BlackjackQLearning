using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackGame : MonoBehaviour
{
    [SerializeField] BlackjackPlayer[] players;
    Deck deck;

    private void Start()
    {
        deck = new Deck();
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
        List<Card> p1Cards = new List<Card>();
        List<Card> p2Cards = new List<Card>();
        p1Cards.Add(deck.Draw());
        p1Cards.Add(deck.Draw());
        p2Cards.Add(deck.Draw());
        p2Cards.Add(deck.Draw());
        int p1Value = PlayPlayer(0, p1Cards, p2Cards[0]);
        int p2Value = PlayPlayer(1, p2Cards, p2Cards[1]);
        bool p1Bust = p1Value > 21;
        bool p2Bust = p2Value > 21;
        if(p1Bust && !p2Bust)
        {
            Debug.Log("P1 Busts, P2 Wins");
        }
        else if(p2Bust && !p1Bust)
        {
            Debug.Log("P2 Busts, P1 Wins");
        }
        else if(p1Bust && p2Bust)
        {
            Debug.Log("Both players bust! Push");
        }
        else if(p1Value > p2Value)
        {
            Debug.Log("P1 Wins");
        }
        else if(p1Value < p2Value)
        {
            Debug.Log("P2 Wins");
        }
        else
        {
            Debug.Log("Push");
        }
    }

    private int PlayPlayer(int playerIndex, List<Card> cards, Card showing)
    {
        int currentValue = GetPlayerValue(cards);
        while(currentValue < 21 && players[playerIndex].Hit(currentValue, showing))
        {
            Card newCard = deck.Draw();
            cards.Add(newCard);
            currentValue = GetPlayerValue(cards);
            players[playerIndex].AddToHistory(true, currentValue);
        }
        if(currentValue < 21)
        {
            players[playerIndex].AddToHistory(false, currentValue);
        }
        PrintAllCards(cards);
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

    private int GetPlayerValue(List<Card> cards)
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
