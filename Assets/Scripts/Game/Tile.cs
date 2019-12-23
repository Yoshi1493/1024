using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Globals;

public class Tile : MonoBehaviour
{
    TextMeshProUGUI valueDisplay;
    int value;

    Vector2 originalSpawnPosition;
    [SerializeField] AnimationCurve movementCurve;

    void Awake()
    {
        valueDisplay = GetComponentInChildren<TextMeshProUGUI>();
        originalSpawnPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public void SetValue(int _value)
    {
        value = _value;
        valueDisplay.text = value.ToString();
    }

    public IEnumerator Slide(SlideDirection slideDirection, int spaces)
    {
        yield return null;
    }
}