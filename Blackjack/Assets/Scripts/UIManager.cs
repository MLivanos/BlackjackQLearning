using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] BlackjackGame game;
    [SerializeField] GameObject gammaSelection;
    [SerializeField] GameObject alphaSelection;
    [SerializeField] GameObject epsilonSelection;
    [SerializeField] GameObject epsilonDecaySelection;
    [SerializeField] GameObject minEpsilonSelection;

    public void SetEquation(int equation)
    {
        switch (equation)
        {
            case 0:
                SimpleUpdate();
                break;
            case 1:
                LearningRateUpdate();
                break;
            case 2:
                FullUpdate();
                break;
            default:
                break;
        }
    }

    private void SimpleUpdate()
    {
        alphaSelection.SetActive(false);
        gammaSelection.SetActive(false);
        game.SetAlpha("1.0");
        game.SetGamma("1.0");
    }

    private void LearningRateUpdate()
    {
        alphaSelection.SetActive(true);
        gammaSelection.SetActive(false);
        game.SetGamma(GetValueFromText(gammaSelection));
        game.SetAlpha("1.0");
    }

     private void FullUpdate()
    {
        alphaSelection.SetActive(true);
        gammaSelection.SetActive(true);
        game.SetGamma(GetValueFromText(gammaSelection));
        game.SetAlpha(GetValueFromText(alphaSelection));
    }

    private string GetValueFromText(GameObject selection)
    {
        return selection.GetComponentInChildren<TMP_InputField>().text;
    }
}
