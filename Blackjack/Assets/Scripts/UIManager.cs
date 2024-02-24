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
    [SerializeField] GameObject valueShowingStateLabels;
    [SerializeField] GameObject valueValueStateLabels;

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

    public void SetStateSpace(int state)
    {
        switch (state)
        {
            case 0:
                SetValueValueStateSpace();
                break;
            case 1:
                SetValueShowingStateSpace();
                break;
            case 2:
                SetValueStateSpace();
                break;
            default:
                break;
        }
    }

    public void ToggleEpsilon(bool isActive)
    {
        epsilonSelection.SetActive(isActive);
        epsilonDecaySelection.SetActive(isActive);
        minEpsilonSelection.SetActive(isActive);
        if (!isActive)
        {
            game.SetEpsilon("0.0");
            game.SetMinEpsilon("0.0");
        }
        else
        {
            game.SetEpsilon(GetValueFromText(epsilonSelection));
            game.SetMinEpsilon(GetValueFromText(minEpsilonSelection));
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

    private void SetValueShowingStateSpace()
    {
        valueShowingStateLabels.SetActive(true);
        valueValueStateLabels.SetActive(false);
    }

    private void SetValueValueStateSpace()
    {
        valueShowingStateLabels.SetActive(false);
        valueValueStateLabels.SetActive(true);
    }

    private void SetValueStateSpace()
    {
        valueShowingStateLabels.SetActive(false);
        valueValueStateLabels.SetActive(false);
    }

    private string GetValueFromText(GameObject selection)
    {
        return selection.GetComponentInChildren<TMP_InputField>().text;
    }
}
