using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayValue : MonoBehaviour
{
    private BlackjackGame game;
    private TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponentInChildren<TMP_InputField>() as TMP_InputField;
    }

    public void SetValue(float newValue)
    {
        SetValue(newValue.ToString("0.000"));
    }

    public void SetValue(string newValue)
    {
        inputField.text = newValue;
    }

    public float GetValue()
    {
        return float.Parse(inputField.text);
    }
}
