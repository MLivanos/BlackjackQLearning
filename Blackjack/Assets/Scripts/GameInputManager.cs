using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInputManager : MonoBehaviour
{
    HumanPlayer player;
    CardDisplay display;

    private void Awake()
    {
        BlackjackGame game = FindObjectsOfType<BlackjackGame>()[0];
        display = FindObjectsOfType<CardDisplay>()[0];
        game.SetOpponent(2);
        player = game.gameObject.GetComponent<HumanPlayer>();
    }

    public void SendMoveToPlayer(bool move)
    {
        player.RecieveMove(move);
    }

    public void BackToLearning()
    {
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        StartCoroutine(display.ResetGame());
    }
}
