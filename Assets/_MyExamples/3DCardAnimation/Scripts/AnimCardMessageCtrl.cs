using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardAnimRec
{

    public class AnimCardMessageCtrl : MonoBehaviour
    {
        /// <summary>
        /// カードメッセージ情報
        /// </summary>
        [System.Serializable]
        public class CardMessInfo
        {
            [Tooltip("カード名")] public string name;
            [Tooltip("メッセージ")] public string message;
            [Tooltip("画像")] public Sprite sprite;
            public CardMessInfo(string _name, string _message, Sprite _sprite)
            {
                name = _name;
                message = _message;
                sprite = _sprite;
            }
        }

        /// <summary>
        /// ○○運情報
        /// </summary>
        [System.Serializable]
        public class  RoundInfo
        {
            public string name;
            public RoundInfo(string _name)
            {
                name = _name;
            }
        }

        [SerializeField, Tooltip("デバッグメッセージ表示")] bool m_isDebug = true;
        [SerializeField, Tooltip("カードスタート位置＆角度")] Transform m_startPos;
        [SerializeField, Tooltip("カードPrefab")] GameObject m_cardPrefab;
        [SerializeField, Tooltip("タイトル")] TMPro.TMP_Text m_titleTxt;
        [SerializeField, Tooltip("メッセージ")] TMPro.TMP_Text m_messageTxt;
        [SerializeField, Tooltip("名前text")] TMPro.TMP_Text m_nameTxt;
        [SerializeField, Tooltip("ガチャ回数")] RoundInfo[] m_roundInfoArr;
        //[SerializeField, Tooltip("ガチャ間隔")] float m_waitSec = 8;
        //[SerializeField, Tooltip("リザルト間隔")] float m_resultSec = 0.2f;
        [SerializeField, Tooltip("ガチャで使われるカード一覧")] CardMessInfo[] m_cardMesArr;
        [SerializeField, Tooltip("選択されたカード一覧")] List<CardMessInfo> m_selectedCardList;
        Color m_titleColor;
        Color m_messageColor;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // タイトルとメッセージの色を保存して透明にする
            m_titleColor = m_titleTxt.color;
            m_messageColor = m_messageTxt.color;
            m_titleTxt.color = Color.clear;
            m_messageTxt.color = Color.clear;

            StartCoroutine(cardThrowCo());
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// カードを投げる
        /// </summary>
        /// <returns></returns>
        IEnumerator cardThrowCo()
        {
            List<CardMessInfo> cardList = new List<CardMessInfo>(m_cardMesArr);
            m_selectedCardList = new List<CardMessInfo>();
            yield return new WaitForSeconds(0.5f);

            // m_selectNum回カードを投げる
            for (int round = 0; round < m_roundInfoArr.Length; round++)
            {
                int idx = Random.Range(0, cardList.Count); // ランダムにカードを選択
                GameObject cardObj = Instantiate(m_cardPrefab, m_startPos); // カードを生成
                cardObj.transform.localScale = Vector3.one; // カードの位置を設定
                bool isUpsideDown = Random.Range(0, 2) == 0; // カードの天地をランダムに設定
                AnimCardOne animCardOne = cardObj.GetComponent<AnimCardOne>(); // カードのスクリプトを取得
                animCardOne.SetCardSprite(cardList[idx].sprite); // カードのスプライトを設定
                animCardOne.SetUpsideDown(isUpsideDown); // カードの天地を設定
                m_selectedCardList.Add(cardList[idx]); // 選択したカードを選択リストに追加
                cardList.RemoveAt(idx); // カードリストから選択したカードを削除
                animCardOne.ThrowCard(false); // カードを投げるアニメーションを再生(消さない)

                yield return new WaitForSeconds(1f); // 1秒待つ
                StartCoroutine(fadeMessageCo(true, m_titleTxt, m_roundInfoArr[round].name, m_titleColor, round, idx)); // タイトルをフェードイン

                yield return new WaitForSeconds(3f); // 3秒待つ(アニメーションで落ちてくる時間も含まれる)

                StartCoroutine(fadeMessageCo(true, m_messageTxt, m_selectedCardList[round].message, m_messageColor, round, idx)); // メッセージをフェードイン

                yield return StartCoroutine(waitAndDeleteCardCo(round, idx, animCardOne)); // カード待機＆クリックで削除

                StartCoroutine(fadeMessageCo(false, m_titleTxt, m_roundInfoArr[round].name, m_titleColor, round, idx)); // タイトルをフェードアウト
                StartCoroutine(fadeMessageCo(false, m_messageTxt, m_selectedCardList[round].message, m_messageColor, round, idx)); // メッセージをフェードアウト
            }
        }

        /// <summary>
        /// カード待機＆クリックで削除
        /// </summary>
        /// <param name="_round">何番目のラウンド(シーン)か</param>
        /// <param name="_idx">何番目のカードか</param>
        /// <param name="animCardOne"></param>
        /// <returns></returns>
        IEnumerator waitAndDeleteCardCo(int _round, int _idx, AnimCardOne animCardOne)
        {
            bool isClicked = false;
            while (!isClicked)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isClicked = true;
                }
                yield return null;
            }
            animCardOne.ToExit();
        }

        IEnumerator fadeMessageCo(bool _isFadeIn, TMPro.TMP_Text _txtObj, string _message, Color _col, int _round, int _idx)
        {
            float t = 0;
            float duration = 1; // フェード時間
            Color col = _col;

            _txtObj.text = _message; // メッセージを設定

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                col.a = _isFadeIn ? t : 1 - t;
                _txtObj.color = col;
                yield return null;
            }
        }

        /// <summary>
        /// 名前を設定する
        /// </summary>
        /// <param name="name"></param>
        public void OnSetName(string name)
        {
            m_nameTxt.text = name+"さんの";
        }

        private void OnGUI()
        {
            if (!m_isDebug)
                return;

            GUI.Label(new Rect(10, 10, 500, 20), "Hierarchy > AnimCardMessageCtrl に設定があります。");
            GUI.Label(new Rect(10, 30, 500, 20), "メッセージはAnimCardMessageCtrl > CardMesArrにあります。");
            GUI.Label(new Rect(10, 50, 500, 20), "Project > CardAnimRecorder > Prefabs > AnimCard がカード単体の雛形です");
            GUI.Label(new Rect(10, 70, 500, 20), "Project > CardAnimRecorder > Animations にアニメーションデータがあります");
        }

    }
}
