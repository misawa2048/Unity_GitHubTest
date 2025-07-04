using System.Collections;
using UnityEngine;

namespace CardAnimRec
{
    public class AnimCardOne : MonoBehaviour
    {
        [SerializeField,Tooltip("表面のスプライト格納場所")] SpriteRenderer m_frontSpriteRenderer;
        [SerializeField,Tooltip("アニメーション格納場所")] Animator m_animator;
        [SerializeField,Tooltip("カード天地変更場所")] Transform m_cardPivot;

        private void Awake()
        {
            m_animator.SetBool("toStart", false);
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// カード表面のスプライトを設定する
        /// </summary>
        /// <param name="sprite"></param>
        public void SetCardSprite(Sprite sprite)
        {
            m_frontSpriteRenderer.sprite = sprite;
        }

        /// <summary>
        /// カードの天地を設定する
        /// </summary>
        /// <param name="isUpsideDown"></param>
        public void SetUpsideDown(bool isUpsideDown)
        {
            m_cardPivot.localRotation = isUpsideDown ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(90, 180, 0);
        }

        /// <summary>
        /// カードを退場させる
        /// </summary>
        public void ToExit()
        {
            m_animator.SetBool("toExit", true); // 退場アニメーションへ遷移
        }

        /// <summary>
        /// カードを投げるアニメーションを再生する(自動消去あり)
        /// </summary>
        /// <param name="isAutoDelete">trueなら自動で消す</param>
        public void ThrowCard(bool isAutoDelete, float delay=10f)
        {
            if (isAutoDelete)
            {
                StartCoroutine(waitAndDeleteCo(delay));
            }
            m_animator.SetBool("toExit", false);
            m_animator.SetBool("toStart", true);
            m_animator.Play("Wait_1sec"); // 1秒待つアニメーション(からの流れ)を再生
        }

        IEnumerator waitAndDeleteCo(float delay)
        {
            yield return new WaitForSeconds(delay);
            m_animator.SetBool("toExit", true);
            Destroy(gameObject, 2f); // 2秒後にカードを削除
        }
    }
}

