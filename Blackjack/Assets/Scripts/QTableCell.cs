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
    private float hitValue;
    private float standValue;
    private float differenceValue;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
    }

    public void ChangeColor(float value, bool isHit)
    {
        UpdateQValues(value, isHit);
        panelImage.color = GetColorFromValue(differenceValue);
    }

    public void ChangeColorDifference()
    {
        panelImage.color = GetColorFromValue(differenceValue);
    }

    public void ChangeColorAction(bool isHit)
    {
        float value = isHit ? hitValue : standValue;
        panelImage.color = GetColorFromValue(value);
    }

    public void ChangeColorPolicy()
    {
        panelImage.color = hitValue >= standValue ? positiveColor : negativeColor;
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
}
