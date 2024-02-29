using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : BlackjackPlayer
{
    public bool move{get; private set;}
    public bool moveIsSet{get; private set;}

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
        moveIsSet = false;
        while(!moveIsSet)
        {
            yield return null;
        }
    }

    public void RecieveMove(bool recievedMove)
    {
        move = recievedMove;
        moveIsSet = true;
    }
}
