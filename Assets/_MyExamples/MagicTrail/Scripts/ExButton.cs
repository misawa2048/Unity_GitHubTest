using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ExButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public MagicCircleCtrl magicCircleCtrl; // MagicCircleCtrlへの参照
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // MagicCircleCtrlが設定されていない場合、自動で検索
        if (magicCircleCtrl == null)
        {
            magicCircleCtrl = FindFirstObjectByType<MagicCircleCtrl>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // マウスがボタンに入った時の処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("マウスがボタンに入りました");
        
        // MagicCircleCtrlのOnButtonEnter()を呼び出す
        if (magicCircleCtrl != null)
        {
            magicCircleCtrl.OnButtonEnter(this.gameObject);
        }
    }

    // マウスがボタンから出た時の処理
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("マウスがボタンから出ました");
        
        // MagicCircleCtrlのOnButtonExit()を呼び出す
        if (magicCircleCtrl != null)
        {
            magicCircleCtrl.OnButtonExit(this.gameObject);
        }
    }
    
    // マウスボタンが押された時の処理
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        
        // MagicCircleCtrlのOnButtonPressed()を呼び出す
        if (magicCircleCtrl != null)
        {
            magicCircleCtrl.OnButtonPressed(this.gameObject);
        }
    }
    
    // マウスボタンが離された時の処理
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        
        // MagicCircleCtrlのOnButtonReleased()を呼び出す
        if (magicCircleCtrl != null)
        {
            magicCircleCtrl.OnButtonReleased(this.gameObject);
        }
    }
}
