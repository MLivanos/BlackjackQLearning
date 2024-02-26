using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTableCell : MonoBehaviour
{
    [SerializeField] private Color32 negativeColor = new Color(1.0f, 0.65f, 0.0f, 1.0f);
    [SerializeField] private Color32 positiveColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
    private Image panelImage;

    private void Awake()
    {
        panelImage = GetComponent<Image>();
    }

    public void ChangeColor(float value)
    {
        panelImage.color = GetColorFromValue(value);
    }

    private Color32 GetColorFromValue(float value)
    {
        Mathf.Clamp(value, -1.0f, 1.0f);
        value = (value + 1.0f) / 2;
        Color32 valueColor = Color.Lerp(negativeColor, positiveColor, value);
        return valueColor;
    }
}
