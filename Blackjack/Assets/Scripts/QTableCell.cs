using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTableCell : MonoBehaviour
{
    [SerializeField] private Color32 negativeColor = new Color(1.0f, 0.65f, 0.0f, 1.0f);
    [SerializeField] private Color32 neutralColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color32 positiveColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
    private Image panelImage;
    private float[] hitValueHistory;
    private float[] standValueHistory;
    private float[] differenceValueHistory;
    int historyIndex;
    private float hitValue;
    private float standValue;
    private float differenceValue;

    private void Awake()
    {
        InitializeHistory();
        panelImage = GetComponent<Image>();
    }

    private void InitializeHistory()
    {
        hitValueHistory = new float[100];
        standValueHistory = new float[100];
        differenceValueHistory = new float[100];
        historyIndex = 0;
    }

    public void Clear()
    {
        InitializeHistory();
        panelImage.color = neutralColor;
    }

    public void ChangeColor(float value, bool isHit)
    {
        UpdateQValues(value, isHit);
        panelImage.color = GetColorFromValue(differenceValue);
    }

    public void ChangeColorDifference(int index=99)
    {
        panelImage.color = GetColorFromValue(differenceValueHistory[index]);
    }

    public void ChangeColorAction(bool isHit, int index=99)
    {
        float value = isHit ? hitValueHistory[index] : standValueHistory[index];
        panelImage.color = GetColorFromValue(value);
    }

    public void ChangeColorPolicy(int index=99)
    {
        panelImage.color = hitValueHistory[index] >= standValue ? positiveColor : negativeColor;
    }

    private Color32 GetColorFromValue(float value)
    {
        value = Mathf.Clamp(value, -1.0f, 1.0f);
        value = (value + 1.0f) / 2;
        Color32 lowColor = value < 0.5 ? negativeColor : neutralColor;
        Color32 highColor = value < 0.5 ? neutralColor : positiveColor;
        float lerpValue = value < 0.5 ? value*2 : (value - 0.5f)*2;
        Color32 valueColor = Color.Lerp(lowColor, highColor, lerpValue);
        return valueColor;
    }

    public void UpdateQValues(float value, bool isHit)
    {
        if(isHit)
        {
            hitValue = value;
        }
        else
        {
            standValue = value;
        }
        differenceValue = (hitValue - standValue)/2;
    }

    public void AddToHistory()
    {
        int index = Mathf.Min(historyIndex, 99);
        hitValueHistory[index] = hitValue;
        standValueHistory[index] = standValue;
        differenceValueHistory[index] = differenceValue;
        historyIndex++;
    }
}
