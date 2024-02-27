using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTable
{
    protected float[,,] qTable;

    protected abstract void CreateTable();

    public abstract float GetEntry(List<Card> cards, Card showing, int action);

    public abstract void SetEntry(List<Card> cards, Card showing, int action, float qValue);

    public QTable()
    {
        CreateTable();
    }
    public bool GetAction(List<Card> cards, Card showing)
    {
        return GetEntry(cards, showing, 0) >= GetEntry(cards, showing, 1);
    }

    public float GetBestQValue(List<Card> cards, Card showing)
    {
        return Mathf.Max(GetEntry(cards, showing, 0), GetEntry(cards, showing, 1));
    }

    public int GetValue(List<Card> cards)
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

    public virtual int GetShowingIndex(Card showing)
    {
        return 0;
    }

    public void Seed(float initialQValue)
    {
        for(int i=0; i<10; i++)
        {
            for(int j=0; j<qTable.GetLength(1); j++)
            {
                qTable[i,j,0] = initialQValue;
                qTable[i,j,1] = -1 * initialQValue; 
            }
        }
    }
}

public class ValueShowingTable: QTable
{
    protected override void CreateTable()
    {
        // Value, showing card, action
        qTable = new float[22,10,2];
    }

    public override float GetEntry(List<Card> cards, Card showing, int action)
    {
        int valueIndex = GetValue(cards);
        int showingIndex = showing.Value() - 2;
        return qTable[valueIndex,showingIndex,action];
    }

    public override void SetEntry(List<Card> cards, Card showing, int action, float qValue)
    {
        int valueIndex = GetValue(cards);
        int showingIndex = showing.Value() - 2;
        qTable[valueIndex,showingIndex,action] = qValue;
    }

    public override int GetShowingIndex(Card showing)
    {
        return showing.Value() - 2;
    }
}

public class ValueCardShowingTable: QTable
{
    protected override void CreateTable()
    {
        // Value, showing card, action
        qTable = new float[22,13,2];
    }

    public override int GetShowingIndex(Card showing)
    {
        int showingIndex;
        switch (showing.value)
        {
            case "J":
                showingIndex = 9;
                break;
            case "Q":
                showingIndex = 10;
                break;
            case "K":
                showingIndex = 11;
                break;
            case "A":
                showingIndex = 12;
                break;
            default:
                showingIndex = showing.Value() - 2;
                break;
        }
        return showingIndex;
    }

    public override float GetEntry(List<Card> cards, Card showing, int action)
    {
        int valueIndex = GetValue(cards);
        int showingIndex = GetShowingIndex(showing);
        return qTable[valueIndex,showingIndex,action];
    }

    public override void SetEntry(List<Card> cards, Card showing, int action, float qValue)
    {
        int valueIndex = GetValue(cards);
        int showingIndex = GetShowingIndex(showing);
        qTable[valueIndex,showingIndex,action] = qValue;
    }
}

public class ValueQTable : QTable
{
    protected override void CreateTable()
    {
        // Value, unused, action
        qTable = new float[22,1,2];
    }

    public override float GetEntry(List<Card> cards, Card showing, int action)
    {
        int valueIndex = GetValue(cards);
        return qTable[valueIndex,0,action];
    }

    public override void SetEntry(List<Card> cards, Card showing, int action, float qValue)
    {
        int valueIndex = GetValue(cards);
        qTable[valueIndex,0,action] = qValue;
    }
}
