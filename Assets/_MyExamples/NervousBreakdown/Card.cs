using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 神経衰弱のカード1枚を表現するクラス
/// ・カードの表裏切り替え
/// ・クリック検知
/// ・数字表示
/// ・ペア成立/不成立の状態管理
/// </summary>
public class Card : MonoBehaviour, IPointerClickHandler
{
    [Header("カード表面のテキスト")]
    [Tooltip("カードの数字を表示するText(UI)")]
    public Text numberText;

    [Header("カードの表面オブジェクト")]
    [Tooltip("表面(数字が見える側)のGameObject。裏返す時に非表示にする")]
    public GameObject front;

    [Header("カードの裏面オブジェクト")]
    [Tooltip("裏面(数字が見えない側)のGameObject。めくる時に非表示にする")]
    public GameObject back;

    // このカードの数字
    public int Number { get; private set; }
    // このカードが既にペア成立済みか
    private bool isMatched = false;
    // ゲーム本体への参照
    private ConcentrationGame game;
    // 現在表向きか
    private bool isOpen = false;

    /// <summary>
    /// カードの数字をセットし、表面に表示する
    /// </summary>
    public void SetNumber(int number)
    {
        Number = number;
        if (numberText != null)
        {
            numberText.text = number.ToString();
        }
    }

    /// <summary>
    /// ゲーム本体の参照をセット
    /// </summary>
    public void SetGame(ConcentrationGame game)
    {
        this.game = game;
    }

    /// <summary>
    /// カードがクリックされた時の処理
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 既に表向き or ペア成立済みなら無視
        if (isOpen || isMatched) return;
        Open();
        // ゲーム本体に通知
        if (game != null)
        {
            game.OnCardFlipped(this);
        }
    }

    /// <summary>
    /// カードを表向きにする
    /// </summary>
    public void Open()
    {
        isOpen = true;
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);
    }

    /// <summary>
    /// カードを裏向きに戻す
    /// </summary>
    public void Close()
    {
        isOpen = false;
        if (front != null) front.SetActive(false);
        if (back != null) back.SetActive(true);
    }

    /// <summary>
    /// ペア成立時に呼ばれる。以降クリック不可に
    /// </summary>
    public void SetMatched()
    {
        isMatched = true;
        // ここでエフェクトや色変更なども可能
    }
}
