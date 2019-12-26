using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Globals;

public class Tile : MonoBehaviour
{
    [SerializeField] AnimationCurve animationCurve;
    RectTransform rectTransform;
    Vector2 originalPosition;

    TextMeshProUGUI valueDisplay;

    public void SetValue(int _value)
    {
        valueDisplay.text = _value.ToString();
    }

    void Awake()
    {
        valueDisplay = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        gameObject.SetActive(false);
    }

    public IEnumerator Slide(Vector2 distance)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + distance * TILE_SIZE;

        float currentLerpTime = 0f;
        while (currentLerpTime < SLIDE_ANIMATION_DURATION)
        {
            float t = currentLerpTime / SLIDE_ANIMATION_DURATION;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(t));

            yield return new WaitForEndOfFrame();
            currentLerpTime += Time.deltaTime;
        }

        rectTransform.anchoredPosition = endPos;
    }

    public IEnumerator Scale(Vector3 startScale, Vector3 endScale)
    {
        float currentLerpTime = 0f;
        while (currentLerpTime < SLIDE_ANIMATION_DURATION)
        {
            float t = currentLerpTime / SLIDE_ANIMATION_DURATION;
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, animationCurve.Evaluate(t));

            yield return new WaitForEndOfFrame();
            currentLerpTime += Time.deltaTime;
        }

        rectTransform.localScale = endScale;
        ResetPosition();
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        gameObject.SetActive(false);
    }
}