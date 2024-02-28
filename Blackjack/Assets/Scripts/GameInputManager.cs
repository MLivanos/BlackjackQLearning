using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    HumanPlayer player;

    private void Awake()
    {
        BlackjackGame game = FindObjectsOfType<BlackjackGame>()[0];
        game.SetOpponent(2);
        player = (HumanPlayer)game.GetPlayer2();
    }

    public void SendMoveToPlayer(bool move)
    {
        player.RecieveMove(move);
    }
}
