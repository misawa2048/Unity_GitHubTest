using UnityEngine;
using UnityEngine.UI; // Image用

public class GameManager : MonoBehaviour
{
    // Inspectorで設定するカードスプライト群
    public Sprite[] cardSprites;

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

        // CardStageの子オブジェクト（CardImage群）を取得
        foreach (Transform child in cardStage.transform)
        {
            // Imageコンポーネントがあれば割り当て
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = cardSprites[Random.Range(0, cardSprites.Length)];
            }
            // SpriteRendererの場合
            else
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = cardSprites[Random.Range(0, cardSprites.Length)];
                }
            }
        }
    }
}
