using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackGame : MonoBehaviour
{
    Deck deck;
    void Start()
    {
        deck = new Deck();
        Card[] cards = new Card[100];
        for(int i=0; i<100; i++)
        {
            cards[i] = deck.Draw();
            Debug.Log(cards[i].suit);
            Debug.Log(cards[i].value);
            Debug.Log(cards[i].blackJackValue);
        }
    }
}
