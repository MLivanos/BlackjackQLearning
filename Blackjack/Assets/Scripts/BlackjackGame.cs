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
        Play();
        Debug.Log("-----");
        Play();
        Debug.Log("-----");
        Play();
    }

    private void Play()
    {
        Card[] p1Cards = {deck.Draw(), deck.Draw()};
        Card[] p2Cards = {deck.Draw(), deck.Draw()};
        int p1Value = PlayPlayer(0, p1Cards);
        int p2Value = PlayPlayer(1, p2Cards);
        bool p1Bust = p1Value > 21;
        bool p2Bust = p2Value > 21;
        Debug.Log(p1Value);
        Debug.Log(p2Value);
        if(p1Bust && !p2Bust)
        {
            Debug.Log("P1 Busts, P2 Wins");
        }
        else if(!p2Bust && p1Bust)
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

    private int PlayPlayer(int playerIndex, Card[] cards)
    {
        Debug.Log(playerIndex);
        Debug.Log(cards[0].value);
        Debug.Log(cards[1].value);
        Debug.Log("========");
        return cards[0].blackJackValue + cards[1].blackJackValue;
    }
}
