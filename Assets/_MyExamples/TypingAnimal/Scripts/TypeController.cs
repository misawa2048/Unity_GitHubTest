using UnityEngine;
using TMPro;

public class TypeController : MonoBehaviour
{
    public TextMeshProUGUI exampleText; // ExampleText (TMP)の参照
    public TMP_InputField inputField; // InputField (TMP)の参照
    public TextMeshProUGUI scoreText; // ScoreText (TMP)の参照
    private string[] animalNames = { "Cat", "Dog", "Elephant", "Lion", "Tiger", "Bear", "Fox", "Wolf", "Rabbit", "Deer" }; // 動物の英語名リスト
    private int score = 0; // 得点

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ランダムな動物名を表示
        if (exampleText != null)
        {
            exampleText.text = GetRandomAnimalName();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 入力フィールドのテキストがExampleTextと一致するか確認
        if (inputField != null && exampleText != null && inputField.text == exampleText.text)
        {
            score++;
            Debug.Log("得点: " + score);

            // 新しい動物名を表示
            exampleText.text = GetRandomAnimalName();

            // 入力フィールドをクリア
            inputField.text = "";
        }

        // 得点をScoreTextに表示
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private string GetRandomAnimalName()
    {
        int index = Random.Range(0, animalNames.Length);
        return animalNames[index];
    }
}
