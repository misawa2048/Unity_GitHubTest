using System.Collections;
using UnityEngine;
namespace CardAnimRec
{
    public class Card3DForRecord : MonoBehaviour
    {
        [SerializeField] Rigidbody m_targetRB;
        [SerializeField] float m_throwForce = 1000;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            anim_throw();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void anim_throw() { StartCoroutine(throwCo());   }
        void anim_open()  { StartCoroutine(openAnimCo());  }
        void anim_close() { StartCoroutine(closeAnimCo()); }

        IEnumerator throwCo()
        {
            yield return new WaitForSeconds(0.5f);
            m_targetRB.AddForce(m_targetRB.transform.forward * m_throwForce, ForceMode.Impulse);
        }
        IEnumerator openAnimCo()
        {
            m_targetRB.transform.localEulerAngles = new Vector3(0, 0, 180);
            m_targetRB.isKinematic = true;
            yield return new WaitForSeconds(0.5f);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                m_targetRB.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(180, 0, t));
                yield return null;
            }
        }
        IEnumerator closeAnimCo()
        {
            m_targetRB.transform.localEulerAngles = new Vector3(0, 0, 0);
            m_targetRB.isKinematic = true;
            yield return new WaitForSeconds(0.5f);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                m_targetRB.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 180, t));
                yield return null;
            }
        }
    }
}
