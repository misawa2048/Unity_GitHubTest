using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    public GameObject notePrefab; // ノートのプレハブを保持する変数
    public Transform noteParent; // ノートの親オブジェクトを指定する変数
    public Vector3 noteVelocity = new Vector3(0, 0, -10f); // ノートの移動速度（物理演算用）
    
    private float timer = 0f; // タイマー用の変数
    private float interval = 1f; // 生成間隔（1秒）

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        // 1秒経過したらノートを生成
        if (timer >= interval)
        {
            GenerateNote();
            timer = 0f; // タイマーをリセット
        }
    }
    
    void GenerateNote()
    {
        // noteParentの子オブジェクトがあるかチェック
        if (noteParent != null && noteParent.childCount > 0)
        {
            // ランダムな子オブジェクトを選択
            int randomIndex = Random.Range(0, noteParent.childCount);
            Transform randomChild = noteParent.GetChild(randomIndex);
            
            // ノートプレハブを生成
            if (notePrefab != null)
            {
                GameObject note = Instantiate(notePrefab, randomChild.position, randomChild.rotation);
                
                // 生成したノートにRigidbodyがあれば初速を与える
                Rigidbody noteRb = note.GetComponent<Rigidbody>();
                if (noteRb != null)
                {
                    noteRb.linearVelocity = noteVelocity;
                }
            }
        }
    }
}
