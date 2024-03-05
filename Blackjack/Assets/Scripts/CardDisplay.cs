using System;
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
    [SerializeField] float rotateSpeed;
    [SerializeField] GameInputManager inputManager;
    private BlackjackGame game;
    private List<GameObject> aliveCards = new List<GameObject>();
    private GameObject hiddenCard;
    private Vector3[] initialPositions;
    public bool isRotating{get; private set;}
    private static Dictionary<string, int> cardIndexLookup = new Dictionary<string, int>()
    {
        { "J", 9},
        { "Q", 10},
        { "K", 11},
        { "A", 12},
    };

    private void Awake()
    {
        SaveInitialPositions();
        ConnectWithGame();
        StartCoroutine(game.PlayWithHuman());
    }

    private void SaveInitialPositions()
    {
        initialPositions = new Vector3[2];
        initialPositions[0] = new Vector3(0,0,0) + playerPosition;
        initialPositions[1] = new Vector3(0,0,0) + agentPosition;
    }

    public void ResetPositions()
    {
        playerPosition = initialPositions[0];
        agentPosition = initialPositions[1];
        SaveInitialPositions();
    }

    private void ConnectWithGame()
    {
        game = FindObjectsOfType<BlackjackGame>()[0];
        game.SetDisplay(this);
    }

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
            hiddenCard = newCard;
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
        aliveCards.Add(newCard);
    }

    private IEnumerator SendCard(GameObject card, Vector3 endPosition, bool sendForward = true)
    {
        Vector3 direction = endPosition - card.transform.position;
        Func<float, float, bool> atPosition = sendForward ? ((z1, z2) => z1 < z2) : ((z1, z2) => z2 < z1);
        while(!atPosition(card.transform.position.z, endPosition.z))
        {
            card.transform.Translate(direction * speed * Time.deltaTime, Space.World);
            yield return null;
        }
        card.transform.position = endPosition;
    }

    public IEnumerator FlipFaceDownCard()
    {
        isRotating = true;
        float timer = 0.0f;
        Vector3 originalPosition = hiddenCard.transform.position;
        while(timer < rotateSpeed / 360.0f)
        {
            hiddenCard.transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime, Space.World);
            hiddenCard.transform.position = originalPosition + 0.2f * Vector3.up * Mathf.Sin(rotateSpeed*timer*Mathf.PI/180.0f);
            timer += Time.deltaTime;
            yield return null;
        }
        hiddenCard.transform.position = originalPosition;
        Vector3 flippedTransform = hiddenCard.transform.eulerAngles;
        flippedTransform.x = 270.0f;
        hiddenCard.transform.eulerAngles = flippedTransform;
        isRotating = false;
    }

    public IEnumerator ResetGame()
    {
        foreach(GameObject card in aliveCards)
        {
            StartCoroutine(SendCard(card, startPosition, false));
        }
        yield return new WaitForSeconds(0.5f);
        ResetPositions();
        StartCoroutine(game.PlayWithHuman());
    }

    public void DisplayMessage(string message)
    {
        StartCoroutine(inputManager.DisplayMessage(message));
    }
}
