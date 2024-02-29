using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : BlackjackPlayer
{
    public bool move{get; private set;}
    public bool moveIsSet{get; private set;}
    private bool isWaitingForMove = false;

    protected override void Start()
    {
        base.Start();
    }
    
    public override bool Hit(List<Card> cards, Card showing)
    {
        StartCoroutine(WaitForResponse());
        return move;
    }

    private IEnumerator WaitForResponse()
    {
        isWaitingForMove = true;
        moveIsSet = false;
        while(!moveIsSet)
        {
            yield return null;
        }
        isWaitingForMove = false;
    }

    public void RecieveMove(bool recievedMove)
    {
        if(!isWaitingForMove)
        {
            return;
        }
        move = recievedMove;
        moveIsSet = true;
    }
}
