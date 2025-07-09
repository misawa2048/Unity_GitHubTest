using UnityEngine;

public class Note : MonoBehaviour
{
    private bool hasBeenHit = false; // 重複ヒットを防ぐためのフラグ
    
    void OnTriggerEnter(Collider other)
    {
        // inputColliderとの衝突を検出
        if (!hasBeenHit && other.enabled)
        {
            // NotesGeneratorのインスタンスを取得してスコアを加算
            if (NotesGenerator.Instance != null)
            {
                NotesGenerator.Instance.AddScore(100); // 100ポイント加算
                hasBeenHit = true;
                
                // ノートを削除
                Destroy(gameObject);
            }
        }
    }
}
