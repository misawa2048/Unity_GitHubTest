using UnityEngine;
using UnityEngine.UI;
// ConcentrationGame, Card などのスクリプトが同じアセンブリ内にあればusingは不要ですが、
// 名前空間がある場合はここにusingを追加してください。
// 例: using YourNamespace;

/// <summary>
/// 神経衰弱ゲーム全体を管理するクラス
/// ・ゲームの説明表示
/// ・リセット/開始ボタン
/// ・ConcentrationGameの制御
/// </summary>
public class CardGame : MonoBehaviour
{
    [Header("UI参照")]
    [Tooltip("説明テキスト")] public Text descriptionText;
    [Tooltip("リセットボタン")] public Button resetButton;
    [Tooltip("ConcentrationGame本体")] public ConcentrationGame concentrationGame;

    // Start is called before the first frame update
    void Start()
    {
        // ゲームの説明を表示
        if (descriptionText != null)
        {
            descriptionText.text = "神経衰弱ゲームへようこそ！\n同じ数字のカードを2枚ずつめくってペアを揃えよう。";
        }
        // リセットボタンにイベントを登録
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClicked);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// リセットボタンが押されたときの処理
    /// </summary>
    void OnResetButtonClicked()
    {
        // ConcentrationGameの子オブジェクト(カード)を全て削除
        foreach (Transform child in concentrationGame.cardParent)
        {
            Destroy(child.gameObject);
        }
        // ConcentrationGameのStart()を再実行して新しいゲームを開始
        concentrationGame.Start();
    }
}
