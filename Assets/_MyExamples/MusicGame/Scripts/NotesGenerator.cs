using UnityEngine;
using TMPro; // TextMeshProを使用するため追加
using System;

[Serializable]
public class ChartData
{
    public string name;
    public string format;
    public float bpm;
    public int division;
    public int offset_ms;
    public int maxLanes;
    public NoteData[] notes;
}

[Serializable]
public class NoteData
{
    public int time_ms;
    public int lane;
    public int type;
    public NoteData[] notes;
}

[System.Serializable]
public class KeyInputData
{
    public KeyCode keyCode; // 押すキー
    public GameObject lightLine; // キーを押したときに光るラインのGameObject
    public Collider inputCollider; // 押した瞬間だけOnにするCollider
}

public class NotesGenerator : MonoBehaviour
{
    public static NotesGenerator Instance { get; private set; } // シングルトンパターン

    [Header("Music & Score Files")]
    public AudioClip musicClip; // 音源ファイル(mp3)を指定
    public TextAsset scoreJson; // 譜面ファイル(json)を指定
    public ChartData chartData; // Jsonから変換した譜面データ

    public GameObject notePrefab; // ノートのプレハブを保持する変数
    public Transform noteParent; // ノートの親オブジェクトを指定する変数
    public Vector3 noteVelocity = new Vector3(0, 0, -10f); // ノートの移動速度（物理演算用）
    public KeyInputData[] keyInputs; // キー入力とオブジェクトの関連付け
    
    // スコア関連の変数
    [Header("Score Settings")]
    public TextMeshProUGUI scoreTextTMP; // スコアを表示するTextMeshPro
    public int currentScore = 0; // 現在のスコア

    [Header("Music Start Settings")]
    public float musicStartDelay = 1.0f; // 音楽再生前の待機時間（秒）
    private AudioSource audioSource;

    private float timer = 0f; // タイマー用の変数
    private float interval = 1f; // 生成間隔（1秒）
    private float playTime = 0f; // 譜面再生用の経過時間
    private int noteIndex = 0; // 次に出すノートのインデックス

    [Header("Note Timing Correction")]
    [Range(0.98f, 1.02f)]
    public float timeMsCorrection = 1.0f;

    void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ゲームスタート時にすべてのラインおよびコライダーを非表示にする
        foreach (KeyInputData keyInput in keyInputs)
        {
            keyInput.lightLine?.SetActive(false);
            
            if (keyInput.inputCollider != null)
            {
                keyInput.inputCollider.enabled = false;
                keyInput.inputCollider.isTrigger = true; // Triggerに設定
            }
        }
        
        // スコア表示を初期化
        UpdateScoreDisplay();
        
        // JsonファイルをChartDataオブジェクトに変換
        if (scoreJson != null)
        {
            chartData = JsonUtility.FromJson<ChartData>(scoreJson.text);
            Debug.Log($"譜面データ読み込み: {chartData.name}, BPM: {chartData.bpm}, Notes: {chartData.notes.Length}");
            // time_ms昇順でソート
            if (chartData.notes != null)
            {
                Array.Sort(chartData.notes, (a, b) => a.time_ms.CompareTo(b.time_ms));
            }
        }
        // AudioSource自動取得/生成
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.clip = musicClip;
        // 譜面再生コルーチン開始
        if (chartData != null && chartData.notes != null)
        {
            StartCoroutine(PlayChartCoroutine());
            StartCoroutine(PlayMusicCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // キー入力処理のみ
        HandleKeyInput();
    }

    private System.Collections.IEnumerator PlayChartCoroutine()
    {
        float startTime = Time.time;
        for (int i = 0; i < chartData.notes.Length; i++)
        {
            float waitTime = (chartData.notes[i].time_ms * timeMsCorrection / 1000f) - (Time.time - startTime);
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);
            int laneId = chartData.notes[i].lane;
            GenerateNote(laneId);
        }
    }

    private System.Collections.IEnumerator PlayMusicCoroutine()
    {
        yield return new WaitForSeconds(musicStartDelay);
        if (audioSource != null && musicClip != null)
        {
            audioSource.Play();
        }
    }
    
    void GenerateNote(int id)
    {
        // noteParentの子オブジェクトがあるかチェック
        if (noteParent != null && noteParent.childCount > 0 && id >= 0 && id < noteParent.childCount)
        {
            // 指定idの子オブジェクトを選択
            Transform randomChild = noteParent.GetChild(id);
            
            // ノートプレハブを生成
            if (notePrefab != null)
            {
                GameObject note = Instantiate(notePrefab, randomChild.position, randomChild.rotation);
                
                // ノートにNoteスクリプトを追加
                note.AddComponent<Note>();
                
                // ノートにTriggerColliderを追加（衝突検出用）
                if (note.GetComponent<Collider>() == null)
                {
                    BoxCollider noteCollider = note.AddComponent<BoxCollider>();
                    noteCollider.isTrigger = true;
                }
                
                // 生成したノートにRigidbodyがあれば初速を与える
                Rigidbody noteRb = note.GetComponent<Rigidbody>();
                if (noteRb != null)
                    noteRb.linearVelocity = noteVelocity;
                
                // 10秒後にノートオブジェクトを削除
                Destroy(note, 10f);
            }
        }
    }
    
    void HandleKeyInput()
    {
        foreach (KeyInputData keyInput in keyInputs)
        {
            if (Input.GetKeyDown(keyInput.keyCode))
            {
                // ラインを光らせる
                keyInput.lightLine?.SetActive(true);
                
                // Colliderを一瞬だけOnにする
                if (keyInput.inputCollider != null)
                    StartCoroutine(ActivateColliderBriefly(keyInput.inputCollider));
            }
            
            if (Input.GetKeyUp(keyInput.keyCode))
            {
                // ラインを消す
                keyInput.lightLine?.SetActive(false);
            }
        }
    }
    
    System.Collections.IEnumerator ActivateColliderBriefly(Collider collider)
    {
        collider.enabled = true;
        yield return new WaitForSeconds(0.1f); // 0.1秒間だけアクティブ
        collider.enabled = false;
    }
    
    // スコア加算メソッド
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    // スコア表示を更新
    void UpdateScoreDisplay()
    {
        string scoreDisplayText = "Score: " + currentScore.ToString();
        
        // TextMeshProでスコアを表示
        if (scoreTextTMP != null)
        {
            scoreTextTMP.text = scoreDisplayText;
        }
    }
}
