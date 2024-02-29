using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInputManager : MonoBehaviour
{
    HumanPlayer player;

    private void Awake()
    {
        BlackjackGame game = FindObjectsOfType<BlackjackGame>()[0];
        game.SetOpponent(2);
        player = game.gameObject.GetComponent<HumanPlayer>();
    }

    public void SendMoveToPlayer(bool move)
    {
        player.RecieveMove(move);
    }

    public void NewHand()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToLearning()
    {
        SceneManager.LoadScene(0);
    }
}
