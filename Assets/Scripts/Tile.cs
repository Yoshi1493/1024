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
        rectTransform.anchoredPosition = TransformPoint(coordinate);
        valueDisplay.text = value.ToString();
    }

    #region Animations

    public IEnumerator Slide((int row, int col) start, (int row, int col) end)
    {
        Vector2 startPos = TransformPoint(start);
        Vector2 endPos = TransformPoint(end);

        float currentLerpTime = 0f;
        while (currentLerpTime < SlideAnimationDuration)
        {
            yield return new WaitForEndOfFrame();
            currentLerpTime += Time.deltaTime;
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, slideAnimationCurve.Evaluate(currentLerpTime / SlideAnimationDuration));
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

    #endregion

    //convert coordinates on the board to coordinates in world space
    Vector2 TransformPoint((int row, int col) coordinate)
    {
        return new Vector2
        (
            coordinate.col * TileSize - (TileSize * 1.5f),
            coordinate.row * TileSize - (TileSize * 1.5f)
        );
    }
}