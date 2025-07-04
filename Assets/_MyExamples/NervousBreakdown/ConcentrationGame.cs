using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神経衰弱のゲーム本体を管理するクラス
/// ・カードの生成、配置
/// ・カードのめくり処理、ペア判定
/// ・ゲーム進行管理
/// </summary>
public class ConcentrationGame : MonoBehaviour
{
    [Header("カードの親オブジェクト")]
    [Tooltip("生成したカードを格納する親Transform")]
    public Transform cardParent;

    [Header("カードのプレハブ")]
    [Tooltip("カードのプレハブ。Cardスクリプトがアタッチされている必要あり")]
    public GameObject cardPrefab;

    [Header("カードの種類数（ペア数）")]
    [Tooltip("異なる数字（絵柄）の種類数。2倍がカード総数になる")]
    public int pairCount = 8;

    // ゲーム中のカードリスト
    private List<Card> cards = new List<Card>();
    // 1回のターンでめくったカード
    private List<Card> flippedCards = new List<Card>();
    // ペアが揃った数
    private int matchedPairs = 0;

    /// <summary>
    /// ゲーム開始時やリセット時に呼ばれる初期化処理
    /// </summary>
    public void Start()
    {
        // カードリスト初期化
        cards.Clear();
        flippedCards.Clear();
        matchedPairs = 0;

        // カード番号リスト作成（各数字2枚ずつ）
        List<int> numbers = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            numbers.Add(i);
            numbers.Add(i);
        }
        // シャッフル
        for (int i = 0; i < numbers.Count; i++)
        {
            int j = Random.Range(i, numbers.Count);
            int tmp = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = tmp;
        }

        // カード生成
        for (int i = 0; i < numbers.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            Card card = cardObj.GetComponent<Card>();
            card.SetNumber(numbers[i]);
            card.SetGame(this); // このゲーム本体を渡す
            cards.Add(card);
        }
    }

    /// <summary>
    /// カードがめくられたときに呼ばれる（Cardから呼び出し）
    /// </summary>
    /// <param name="card">めくられたカード</param>
    public void OnCardFlipped(Card card)
    {
        // すでに2枚めくっている場合は何もしない
        if (flippedCards.Count >= 2) return;
        // すでにめくったカードは追加しない
        if (flippedCards.Contains(card)) return;

        flippedCards.Add(card);

        // 2枚めくったら判定
        if (flippedCards.Count == 2)
        {
            if (flippedCards[0].Number == flippedCards[1].Number)
            {
                // ペア成立
                flippedCards[0].SetMatched();
                flippedCards[1].SetMatched();
                matchedPairs++;
                // 全ペア揃ったらクリア
                if (matchedPairs == pairCount)
                {
                    Debug.Log("ゲームクリア！");
                }
                flippedCards.Clear();
            }
            else
            {
                // ペア不成立: 少し待って裏返す
                card.StartCoroutine(CloseCardsAfterDelay(1.0f));
            }
        }
    }

    /// <summary>
    /// ペア不成立時、少し待ってカードを裏返すコルーチン
    /// </summary>
    private System.Collections.IEnumerator CloseCardsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var c in flippedCards)
        {
            c.Close();
        }
        flippedCards.Clear();
    }
}
