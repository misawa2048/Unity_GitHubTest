using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 神経衰弱のカードの動作を制御するスクリプト。
/// </summary>
public class NervousBreakdownCard : MonoBehaviour, IPointerClickHandler
{
    // カードの表のスプライト
    private Sprite frontSprite;
    // カードの裏のスプライト（初期値）
    private Sprite backSprite;
    // 現在表示中のスプライト
    private Sprite currentSprite;
    // スプライトを表示するImageコンポーネント
    private Image image;
    // カードが表かどうか
    private bool isFaceUp = false;

    void Awake()
    {
        image = GetComponent<Image>();
        backSprite = image.sprite; // 初期値としてImageのスプライトを裏のスプライトに設定
    }

    /// <summary>
    /// カードのスプライトを割り当てます。
    /// </summary>
    /// <param name="front">表のスプライト</param>
    /// <param name="back">裏のスプライト</param>
    public void SetSprites(Sprite front)
    {
        frontSprite = front;
        ShowBack();
    }

    /// <summary>
    /// カードがクリックされたときに呼ばれ、表のスプライトを表示します。
    /// </summary>
    /// <param name="eventData">クリックイベントデータ</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        ShowFront();
    }

    /// <summary>
    /// カードの表のスプライトを表示します。
    /// </summary>
    public void ShowFront()
    {
        if (image != null && frontSprite != null)
        {
            image.sprite = frontSprite;
            currentSprite = frontSprite;
            isFaceUp = true;
        }
    }

    /// <summary>
    /// カードの裏のスプライト（初期値）を表示します。
    /// </summary>
    public void ShowBack()
    {
        if (image != null && backSprite != null)
        {
            image.sprite = backSprite;
            currentSprite = backSprite;
            isFaceUp = false;
        }
    }

    /// <summary>
    /// 現在カードが表かどうかを取得します。
    /// </summary>
    public bool IsFaceUp()
    {
        return isFaceUp;
    }
}
