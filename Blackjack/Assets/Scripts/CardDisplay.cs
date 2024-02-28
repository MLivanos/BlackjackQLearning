using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] GameObject[] clubCards;
    [SerializeField] GameObject[] spadeCards;
    [SerializeField] GameObject[] heartCards;
    [SerializeField] GameObject[] diamondCards;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 playerPosition;
    [SerializeField] Vector3 agentPosition;
    [SerializeField] Vector3 cardOffset;
    [SerializeField] float speed;
    
    private static Dictionary<string, int> cardIndexLookup = new Dictionary<string, int>()
    {
        { "J", 9},
        { "Q", 10},
        { "K", 11},
        { "A", 12},
    };

    private int FindIndex(string valueString)
    {
        int cardIndex;
        bool isNumberCard = int.TryParse(valueString, out cardIndex);
        if (isNumberCard)
        {
            return cardIndex - 2;
        }
        return cardIndexLookup[valueString];
    }

    private GameObject FindCard(Card card)
    {
        string suit = card.suit;
        string value = card.value;
        GameObject[] cardSuitPrefab = suit == "Hearts" ? heartCards : suit == "Clubs" ? clubCards : suit=="Diamonds" ? diamondCards : spadeCards;
        return cardSuitPrefab[FindIndex(value)];
    }

    public void CreateCard(Card card, bool isFaceDown=false, bool isPlayerCard=true)
    {
        GameObject cardPrefab = FindCard(card);
        Vector3 endPosition = isPlayerCard ? playerPosition : agentPosition;
        GameObject newCard = Instantiate(cardPrefab, startPosition, cardPrefab.transform.rotation);
        if(isFaceDown)
        {
            newCard.transform.Rotate(Vector3.right * 180);
        }
        StartCoroutine(SendCard(newCard, endPosition));
        endPosition += cardOffset;
        if (isPlayerCard)
        {
            playerPosition += cardOffset;
        }
        else
        {
            agentPosition += cardOffset;
        }
    }

    private IEnumerator SendCard(GameObject card, Vector3 endPosition)
    {
        Vector3 direction = endPosition - card.transform.position;
        while(card.transform.position.z > endPosition.z)
        {
            card.transform.Translate(direction * speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}
