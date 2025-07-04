using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("カード画像のスプライトリスト")]
    public List<Sprite> cardSprites; // ペア用に2枚ずつ用意しておく

    [Header("カードを並べる親オブジェクト(CardStage)")]
    public Transform cardStage;

    private List<NervousBreakdownCard> flippedCards = new List<NervousBreakdownCard>();
    private bool isChecking = false;

    // シーン開始時に呼ばれる
    void Start()
    {
        AssignRandomSpritesToCards();
    }

    // CardStageの子(CardImage)にランダムでスプライトを割り当てる
    public void AssignRandomSpritesToCards()
    {
        if (cardStage == null)
        {
            Debug.LogError("CardStageが設定されていません");
            return;
        }

        // CardStageの子を全て取得
        List<Transform> cardImages = new List<Transform>();
        foreach (Transform child in cardStage)
        {
            if (child.GetComponent<Image>() != null)
                cardImages.Add(child);
        }

        // スプライト名の末尾の数字ごとにグループ化
        var pairDict = new Dictionary<string, List<Sprite>>();
        foreach (var sprite in cardSprites)
        {
            if (sprite == null) continue;
            string name = sprite.name;
            string num = System.Text.RegularExpressions.Regex.Match(name, @"(\\d+)$").Value;
            if (string.IsNullOrEmpty(num)) num = name; // 数字がなければ名前全体
            if (!pairDict.ContainsKey(num)) pairDict[num] = new List<Sprite>();
            pairDict[num].Add(sprite);
        }

        // 各ペアごとに1枚ずつカードリストに追加（ペア数分）
        List<Sprite> assignList = new List<Sprite>();
        foreach (var kv in pairDict)
        {
            foreach (var sprite in kv.Value)
            {
                assignList.Add(sprite);
            }
        }

        // カード枚数に合わせてリストを拡張（足りない場合はループで追加）
        while (assignList.Count < cardImages.Count)
        {
            foreach (var kv in pairDict)
            {
                foreach (var sprite in kv.Value)
                {
                    if (assignList.Count >= cardImages.Count) break;
                    assignList.Add(sprite);
                }
                if (assignList.Count >= cardImages.Count) break;
            }
        }
        // 余分な分はカット
        if (assignList.Count > cardImages.Count)
            assignList.RemoveRange(cardImages.Count, assignList.Count - cardImages.Count);

        // シャッフル
        for (int i = 0; i < assignList.Count; i++)
        {
            Sprite temp = assignList[i];
            int randomIndex = Random.Range(i, assignList.Count);
            assignList[i] = assignList[randomIndex];
            assignList[randomIndex] = temp;
        }

        // 割り当て
        for (int i = 0; i < cardImages.Count; i++)
        {
            var card = cardImages[i].GetComponent<NervousBreakdownCard>();
            if (card != null)
            {
                // カードの表スプライトを割り当てる
                card.SetSprites(assignList[i]);
                // ゲーム開始時は裏向きにする
                card.ShowBack();
            }
            else
            {
                // 互換性のためImageのみの場合は従来通り
                Image img = cardImages[i].GetComponent<Image>();
                if (img != null)
                    img.sprite = assignList[i];
            }
        }
    }

    /// <summary>
    /// カードがクリックされた時に呼ばれる
    /// </summary>
    public void OnCardClicked(NervousBreakdownCard card)
    {
        // チェック中または2枚すでにめくられている場合は処理しない
        if (isChecking || flippedCards.Count >= 2)
            return;

        flippedCards.Add(card);

        // 2枚めくられた場合の処理
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    /// <summary>
    /// 2枚のカードが一致するかチェックする
    /// </summary>
    private System.Collections.IEnumerator CheckMatch()
    {
        isChecking = true;
        
        // すべてのカードのクリックを無効化
        SetAllCardsClickable(false);

        // 少し待機（プレイヤーがカードを確認する時間）
        yield return new WaitForSeconds(1.0f);

        NervousBreakdownCard card1 = flippedCards[0];
        NervousBreakdownCard card2 = flippedCards[1];

        // スプライト名から数字を抽出して比較
        string num1 = ExtractNumberFromSpriteName(card1.GetFrontSprite().name);
        string num2 = ExtractNumberFromSpriteName(card2.GetFrontSprite().name);

        if (num1 == num2)
        {
            // 一致した場合：カードを無効化（スプライトをnullに）
            card1.DisableCard();
            card2.DisableCard();
            Debug.Log("マッチしました！");
        }
        else
        {
            // 一致しなかった場合：カードを裏返す
            card1.ShowBack();
            card2.ShowBack();
            Debug.Log("マッチしませんでした");
        }

        // リセット
        flippedCards.Clear();
        isChecking = false;
        
        // 無効化されていないカードのクリックを有効化
        SetAllCardsClickable(true);

        // ゲーム終了チェック
        CheckGameEnd();
    }

    /// <summary>
    /// スプライト名から数字を抽出する
    /// </summary>
    private string ExtractNumberFromSpriteName(string spriteName)
    {
        string num = System.Text.RegularExpressions.Regex.Match(spriteName, @"(\d+)$").Value;
        return string.IsNullOrEmpty(num) ? spriteName : num;
    }

    /// <summary>
    /// すべてのカードのクリック可能状態を設定
    /// </summary>
    private void SetAllCardsClickable(bool clickable)
    {
        foreach (Transform child in cardStage)
        {
            var card = child.GetComponent<NervousBreakdownCard>();
            if (card != null && !card.IsDisabled())
            {
                card.SetClickable(clickable);
            }
        }
    }

    /// <summary>
    /// ゲーム終了チェック
    /// </summary>
    private void CheckGameEnd()
    {
        bool allDisabled = true;
        foreach (Transform child in cardStage)
        {
            var card = child.GetComponent<NervousBreakdownCard>();
            if (card != null && !card.IsDisabled())
            {
                allDisabled = false;
                break;
            }
        }

        if (allDisabled)
        {
            Debug.Log("ゲームクリア！");
            StartCoroutine(RestartGame());
        }
    }

    /// <summary>
    /// ゲームクリア後にシーンをリロードする
    /// </summary>
    private IEnumerator RestartGame()
    {
        // 少し待機してからリロード
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
