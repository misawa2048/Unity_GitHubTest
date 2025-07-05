using UnityEngine;
using UnityEngine.UI; // Image用

public class GameManager : MonoBehaviour
{
    // Inspectorで設定するカードスプライト群
    public Sprite[] cardSprites;
    // カードの裏面スプライト
    public Sprite cardBackSprite;
    // 空のカードスプライト
    public Sprite emptyCardSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignRandomCardSprites();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // カード画像をランダムに割り当てる
    public void AssignRandomCardSprites()
    {
        // cardSpritesがInspectorで設定されているかチェック
        if (cardSprites == null || cardSprites.Length == 0)
        {
            Debug.LogError("カードスプライトが設定されていません。GameManagerのInspectorでcardSpritesを設定してください。");
            return;
        }

        // CardStageオブジェクトを探す
        GameObject cardStage = GameObject.Find("CardStage");
        if (cardStage == null)
        {
            Debug.LogError("CardStageオブジェクトが見つかりません。");
            return;
        }

        // カード枚数
        int cardCount = cardStage.transform.childCount;
        // 2枚1組で必要なペア数
        int pairCount = cardCount / 2;
        // 必要なスプライトをランダムに選ぶ
        Sprite[] selectedSprites = new Sprite[pairCount];
        System.Collections.Generic.List<int> usedIndices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            int idx;
            do {
                idx = Random.Range(0, cardSprites.Length);
            } while (usedIndices.Contains(idx) && usedIndices.Count < cardSprites.Length);
            usedIndices.Add(idx);
            selectedSprites[i] = cardSprites[idx];
        }
        // 2枚1組でリスト化
        System.Collections.Generic.List<Sprite> cardList = new System.Collections.Generic.List<Sprite>();
        foreach (var s in selectedSprites)
        {
            cardList.Add(s);
            cardList.Add(s);
        }
        // 奇数の場合は空カード
        if (cardList.Count < cardCount)
        {
            cardList.Add(emptyCardSprite);
        }
        // シャッフル
        for (int i = 0; i < cardList.Count; i++)
        {
            int j = Random.Range(i, cardList.Count);
            var tmp = cardList[i];
            cardList[i] = cardList[j];
            cardList[j] = tmp;
        }
        // 割り当て
        int n = 0;
        foreach (Transform child in cardStage.transform)
        {
            // Imageコンポーネントがあれば割り当て
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = cardList[n];
            }
            // SpriteRendererの場合
            else
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = cardList[n];
                }
            }
            n++;
        }
    }
}
