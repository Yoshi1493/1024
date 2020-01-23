using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Globals;

public class Tile : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AnimationCurve slideAnimationCurve;
    [SerializeField] TextMeshProUGUI valueDisplay;
    [SerializeField] RectTransform rectTransform;

    public void Initialize((int row, int col) coordinate, int value)
    {
        //set anchored position
        rectTransform.anchoredPosition = BoardToWorldSpace(coordinate);

        //set value
        SetValue(value);
    }

    public void SetValue(int value)
    {
        valueDisplay.text = value.ToString();
        gameObject.SetActive(value != 0);
    }

    public IEnumerator Slide((int row, int col) start, (int row, int col) end)
    {
        Vector2 startPos = BoardToWorldSpace(start);
        Vector2 endPos = BoardToWorldSpace(end);

        float currentLerpTime = 0f;
        while (currentLerpTime < SLIDE_ANIMATION_DURATION)
        {
            float t = currentLerpTime / SLIDE_ANIMATION_DURATION;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, slideAnimationCurve.Evaluate(t));

            yield return new WaitForEndOfFrame();
            currentLerpTime += Time.deltaTime;
        }

        rectTransform.anchoredPosition = endPos;
    }

    public void Shrink()
    {
        anim.SetTrigger("Shrink");
        Destroy(gameObject, SLIDE_ANIMATION_DURATION + anim.GetCurrentAnimatorStateInfo(0).length);
    }

    Vector2 BoardToWorldSpace((int row, int col) coordinate)
    {
        Vector2 result;
        result.x = coordinate.col * TILE_SIZE - (TILE_SIZE * 1.5f);
        result.y = coordinate.row * TILE_SIZE - (TILE_SIZE * 1.5f);

        return result;
    }
}