using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("カード画像のスプライトリスト")]
    public List<Sprite> cardSprites; // ペア用に2枚ずつ用意しておく

    [Header("カードを並べる親オブジェクト(CardStage)")]
    public Transform cardStage;

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
}
