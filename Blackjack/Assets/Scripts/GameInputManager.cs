using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameInputManager : MonoBehaviour
{
    [SerializeField] TMP_Text displayMessage;
    [SerializeField] CardDisplay display;
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

    public void BackToLearning()
    {
        SceneManager.LoadScene(0);
    }

    public void Reset()
    {
        StartCoroutine(display.ResetGame());
    }

    public IEnumerator DisplayMessage(string message)
    {
        displayMessage.text = message;
        Color textColor = displayMessage.color;
        float alpha = 0.0f;
        float transparencySpeed = 2.0f;
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime * transparencySpeed;
            textColor.a = alpha;
            displayMessage.color = textColor;
            yield return null;
        }
        yield return new WaitForSeconds(3.0f);
        while(alpha > 0.0f)
        {
            alpha -= Time.deltaTime * transparencySpeed;
            textColor.a = alpha;
            displayMessage.color = textColor;
            yield return null;
        }
    }
}
