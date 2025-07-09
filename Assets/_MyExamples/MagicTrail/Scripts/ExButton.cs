using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ExButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
        public UnityEvent<PointerEventData> m_ExEnterEvent;
        public UnityEvent<PointerEventData> m_ExExitEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // マウスがボタンに入った時の処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("マウスがボタンに入りました");
        m_ExEnterEvent.Invoke(eventData); // イベントを呼び出す
    }

    // マウスがボタンから出た時の処理
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("マウスがボタンから出ました");
        m_ExExitEvent.Invoke(eventData); // イベントを呼び出す       
    }
}