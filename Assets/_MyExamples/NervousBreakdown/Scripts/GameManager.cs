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

        // カード枚数分のスプライトリストを作成（重複なし、ペア用に2枚ずつ）
        List<Sprite> assignList = new List<Sprite>();
        int pairCount = cardImages.Count / 2;
        int maxPair = Mathf.Min(pairCount, cardSprites.Count); // cardSprites数を超えない
        for (int i = 0; i < maxPair; i++)
        {
            assignList.Add(cardSprites[i]);
            assignList.Add(cardSprites[i]);
        }

        // シャッフル
        for (int i = 0; i < assignList.Count; i++)
        {
            Sprite temp = assignList[i];
            int randomIndex = Random.Range(i, assignList.Count);
            assignList[i] = assignList[randomIndex];
            assignList[randomIndex] = temp;
        }

        // 割り当て（assignListの数だけ割り当て、それ以降は無視）
        for (int i = 0; i < cardImages.Count; i++)
        {
            Image img = cardImages[i].GetComponent<Image>();
            if (i < assignList.Count)
                img.sprite = assignList[i];
            else
                img.enabled = false; // 余ったカードは非表示にするなど
        }
    }
}
