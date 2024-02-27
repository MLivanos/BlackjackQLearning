using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject QTableCanvas;
    GameObject currentShowing;
    QTableCell[,] QTableMatrix = new QTableCell[0,0];
    private int iterationIndex = 99;
    private int viewIndex;

    private void Start()
    {
        currentShowing = valueValueStateLabels;
        InstantiateQTableCells();
    }

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
        game.SetStateSpace(state);
        InstantiateQTableCells();
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

    public void InstantiateQTableCells()
    {
        ClearQTableCells();
        int numberOfValues = 20;
        int numberOfOpponentValues = currentShowing ? currentShowing.transform.childCount : 1;
        QTableMatrix = new QTableCell[numberOfValues,numberOfOpponentValues];
        for(int i=0; i<numberOfValues; i++)
        {
            for(int j=0; j<numberOfOpponentValues; j++)
            {
                GameObject newCell = Instantiate(cellPrefab, QTableCanvas.transform);
                QTableMatrix[i,j] = newCell.GetComponent<QTableCell>();
                newCell.transform.Translate(Vector3.right * j * 20 + Vector3.down * i * 15);
            }
        }
    }

    private void ClearQTableCells()
    {
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                Destroy(QTableMatrix[i,j].gameObject);
            }
        }
    }

    public void InstantiateQLearner()
    {
        SimpleUpdate();
    }

    public void UpdateCell(int valueIndex, int opponentIndex, float value, bool action)
    {
        QTableMatrix[valueIndex,opponentIndex].ChangeColor(value, action);
    }

    public void ChangeTableView(int viewType)
    {
        viewIndex = viewType;
        switch (viewIndex)
        {
            case 0:
                ChangeTableViewDifference();
                break;
            case 1:
                ChangeTableViewAction(true);
                break;
            case 2:
                ChangeTableViewAction(false);
                break;
            case 3:
                ChangeTableViewPolicy();
                break;
            default:
                break;
        }
    }

    public void ChangeTableViewDifference()
    {
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].ChangeColorDifference(iterationIndex);
            }
        }
    }

    public void ChangeTableViewAction(bool isHit)
    {
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].ChangeColorAction(isHit,iterationIndex);
            }
        }
    }

    public void ChangeTableViewPolicy()
    {
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].ChangeColorPolicy(iterationIndex);
            }
        }
    }

    public void StoreQTable()
    {
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].AddToHistory();
            }
        }
    }

    public void SetIterationIndex(float sliderValue)
    {
        int oldIterationIndex = iterationIndex;
        iterationIndex = (int)(sliderValue * 99);
        if (iterationIndex != oldIterationIndex)
        {
            ChangeTableView(viewIndex);
        }
    }

    public void SeedQTable(float initialQValue=5.0f)
    {
        game.SeedQTable(initialQValue);
        for(int i=0; i<10; i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].ChangeColor(initialQValue,true);
                QTableMatrix[i,j].ChangeColor(-1*initialQValue,false);
            }
        }
    }

    public void ClearQTable()
    {
        game.ClearQTable();
        for(int i=0; i<QTableMatrix.GetLength(0); i++)
        {
            for(int j=0; j<QTableMatrix.GetLength(1); j++)
            {
                QTableMatrix[i,j].Clear();
            }
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
        currentShowing = valueShowingStateLabels;
    }

    private void SetValueValueStateSpace()
    {
        valueShowingStateLabels.SetActive(false);
        valueValueStateLabels.SetActive(true);
        currentShowing = valueValueStateLabels;
    }

    private void SetValueStateSpace()
    {
        valueShowingStateLabels.SetActive(false);
        valueValueStateLabels.SetActive(false);
        currentShowing = null;
    }

    private string GetValueFromText(GameObject selection)
    {
        return selection.GetComponentInChildren<TMP_InputField>().text;
    }

    public void Run()
    {
        game.Run();
    }

    public void Play()
    {
        
    }
}
