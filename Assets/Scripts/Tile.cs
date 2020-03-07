using System.Collections;
using UnityEngine;
using TMPro;
using static Globals;

public class Tile : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AnimationCurve slideAnimationCurve;
    [SerializeField] TextMeshProUGUI valueDisplay;
    [SerializeField] RectTransform rectTransform;

    const int TileSize = 180;   //game tile width & height (px)

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
        while (currentLerpTime < SlideAnimationDuration)
        {
            float t = currentLerpTime / SlideAnimationDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, slideAnimationCurve.Evaluate(t));

            yield return new WaitForEndOfFrame();
            currentLerpTime += Time.deltaTime;
        }

        rectTransform.anchoredPosition = endPos;
    }

    public IEnumerator Shrink(float initialDelay = SlideAnimationDuration)
    {
        yield return new WaitForSeconds(initialDelay);

        anim.enabled = true;
        anim.SetTrigger("Shrink");
        yield return new WaitUntil(() => rectTransform.localScale == Vector3.zero);

        Destroy(gameObject);
    }

    Vector2 BoardToWorldSpace((int row, int col) coordinate)
    {
        Vector2 result;
        result.x = coordinate.col * TileSize - (TileSize * 1.5f);
        result.y = coordinate.row * TileSize - (TileSize * 1.5f);

        return result;
    }
}