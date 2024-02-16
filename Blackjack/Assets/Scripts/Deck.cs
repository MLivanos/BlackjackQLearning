using System;
using System.Collections;
using System.Collections.Generic;

public class Deck
{
    private List<Card> deck;
    private List<Card> discarded;

    public Deck()
    {
        deck = new List<Card>();
        discarded = new List<Card>();
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        string[] values = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
        string[] suits = {"Hearts", "Spades", "Diamonds", "Clubs"};
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<13; j++)
            {
                Card newCard = new Card(values[j], suits[i], Math.Min(j+1,10));
                deck.Add(newCard);
            }
        }
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        Random randomNumberGenerator = new Random();
        List<Card> shuffledDeck = new List<Card>();
        for(int i=0; i<deck.Count; i++)
        {
            int index = randomNumberGenerator.Next(deck.Count);
            Card card = deck[index];
            deck.RemoveAt(index);
            shuffledDeck.Add(card);
        }
        deck = shuffledDeck;
    }

    public Card Draw()
    {
        Card card = deck[0];
        discarded.Add(card);
        deck.RemoveAt(0);
        if (deck.Count == 0)
        {
            deck = discarded;
            ShuffleDeck();
        }
        return card;
    }
}

public class Card
{
    public string value {get; private set;}
    public int blackJackValue {get; private set;}
    public string suit {get; private set;}

    public Card(string value_, string suit_, int blackJackValue_)
    {
        value = value_;
        suit = suit_;
        blackJackValue = blackJackValue_;
    }
}
