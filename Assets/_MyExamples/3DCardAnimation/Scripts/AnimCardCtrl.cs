using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardAnimRec
{
    public class AnimCardCtrl : MonoBehaviour
    {
        [System.Serializable]
        public class ResultInfo
        {
            public bool isUpsideDown;
            public Sprite sprite;
            public ResultInfo(bool _isUpsideDown, Sprite _sprite)
            {
                isUpsideDown = _isUpsideDown;
                sprite = _sprite;
            }
        }

        [SerializeField, Tooltip("カードスタート位置＆角度")] Transform m_startPos;
        [SerializeField, Tooltip("リザルト一番目位置＆角度")] Transform m_resStartPos;
        [SerializeField, Tooltip("リザルト最終位置＆角度")] Transform m_resEndPos;
        [SerializeField, Tooltip("カードPrefab")] GameObject m_cardPrefab;
        [SerializeField, Tooltip("ガチャ回数")] int m_selectNum = 5;
        //[SerializeField, Tooltip("ガチャ間隔")] float m_waitSec = 8;
        [SerializeField, Tooltip("リザルト間隔")] float m_resultSec = 0.2f;
        [SerializeField, Tooltip("ガチャで使われるカード一覧")] Sprite[] m_cardSprArr;
        [SerializeField, Tooltip("選択されたカード一覧")] List<ResultInfo> m_selectedCardList;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(CardThrowCo());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 500, 20), "Hierarchy > AnimCardCtrl に設定があります。");
            GUI.Label(new Rect(10, 30, 500, 20), "Project > CardAnimRecorder > Prefabs > AnimCard がカード単体の雛形です");
            GUI.Label(new Rect(10, 50, 500, 20), "Project > CardAnimRecorder > Animations にアニメーションデータがあります");
        }

        IEnumerator CardThrowCo()
        {
            List<Sprite> sprList = new List<Sprite>(m_cardSprArr);
            m_selectedCardList = new List<ResultInfo>();
            yield return new WaitForSeconds(0.5f);

            // m_selectNum回カードを投げる
            for (int i = 0; i < m_selectNum; i++)
            {
                int idx = Random.Range(0, sprList.Count); // ランダムにカードを選択
                GameObject cardObj = Instantiate(m_cardPrefab, m_startPos); // カードを生成
                cardObj.transform.position = m_startPos.position; // カードの位置を設定
                bool isUpsideDown = Random.Range(0, 2) == 0; // カードの天地をランダムに設定
                AnimCardOne animCardOne = cardObj.GetComponent<AnimCardOne>();
                animCardOne.SetCardSprite(sprList[idx]); // カードのスプライトを設定
                animCardOne.SetUpsideDown(isUpsideDown); // カードの天地を設定
                m_selectedCardList.Add(new ResultInfo(isUpsideDown, sprList[idx])); // 選択したカードを選択リストに追加
                sprList.RemoveAt(idx); // カードリストから選択したカードを削除
                animCardOne.ThrowCard(true,5f); // カードを投げるアニメーションを再生(して5秒後に退場)
                yield return new WaitForSeconds(8f); // 8秒待つ
            }

            // リザルト表示
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < m_selectedCardList.Count; i++)
            {
                float rate = (float)i / (float)(m_selectedCardList.Count - 1); // カードの表示位置を計算
                GameObject cardObj = Instantiate(m_cardPrefab, m_startPos); // カードを生成
                Vector3 pos = Vector3.Lerp(m_resStartPos.position, m_resEndPos.position, rate); // カードの位置を計算
                Quaternion rot = Quaternion.Lerp(m_resStartPos.rotation, m_resEndPos.rotation, rate); // カードの角度を計算
                cardObj.transform.position = pos; // カードの位置を設定
                cardObj.transform.rotation = rot; // カードの角度を設定
                AnimCardOne animCardOne = cardObj.GetComponent<AnimCardOne>();
                animCardOne.SetCardSprite(m_selectedCardList[i].sprite); // カードのスプライトを設定
                animCardOne.SetUpsideDown(m_selectedCardList[i].isUpsideDown); // カードの天地を設定
                animCardOne.ThrowCard(false); // カードを投げるアニメーションを再生(して消さない)
                yield return new WaitForSeconds(m_resultSec); // 0.2秒待つ
            }
        }
    }
}
