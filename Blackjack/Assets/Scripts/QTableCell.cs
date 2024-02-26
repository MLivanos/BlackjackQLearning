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
        panelImage.color = GetColorFromValue(value, isHit);
    }

    private Color32 GetColorFromValue(float value, bool isHit)
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
        Mathf.Clamp(differenceValue, -1.0f, 1.0f);
        differenceValue = (differenceValue + 1.0f) / 2;
        Color32 lowColor = differenceValue < 0.5 ? negativeColor : neutralColor;
        Color32 highColor = differenceValue < 0.5 ? neutralColor : positiveColor;
        differenceValue = differenceValue < 0.5 ? differenceValue*2 : (differenceValue - 0.5f)*2;
        //Debug.Log(differenceValue);
        Color32 valueColor = Color.Lerp(lowColor, highColor, differenceValue);
        return valueColor;
    }
}
