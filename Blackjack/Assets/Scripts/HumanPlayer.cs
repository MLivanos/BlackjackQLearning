using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : BlackjackPlayer
{
    private bool move;
    private bool moveIsSet = false;
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
        while(!moveIsSet)
        {
            yield return null;
        }
        moveIsSet = false;
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
