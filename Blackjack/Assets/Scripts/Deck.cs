using System;
using System.Collections;
using System.Collections.Generic;

public class Deck
{
    private List<Card> deck;
    private List<Card> discarded;
    private Random randomNumberGenerator = new Random();

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
                Card newCard = new Card(values[j], suits[i]);
                deck.Add(newCard);
            }
        }
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
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
    public string suit {get; private set;}
    public static Dictionary<string, int> blackjackValueLookup = new Dictionary<string, int>()
    {
        { "A", 11},
        { "J", 10},
        { "Q", 10},
        { "K", 10},
    };

    public Card(string value_, string suit_)
    {
        value = value_;
        suit = suit_;
    }

    public int Value()
    {
        bool faceCard = blackjackValueLookup.ContainsKey(value);
        return faceCard ? blackjackValueLookup[value] : Int32.Parse(value);
    }
}
