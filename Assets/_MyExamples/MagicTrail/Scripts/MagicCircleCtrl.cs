using UnityEngine;

public class MagicCircleCtrl : MonoBehaviour
{
    [Header("Button State")]
    public bool isButtonPressed = false; // ボタンを押しているかどうかのフラグ
    
    [Header("Trail Object")]
    public GameObject trailObject; // 軌跡を描くためのGameObject
    [Range(0f, 10f)]
    public float trailDepthOffset = 0.5f; // 軌跡オブジェクトの奥行オフセット値
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // ボタンが押された時に呼ばれる関数
    public void OnButtonPressed(GameObject buttonObject = null)
    {
        isButtonPressed = true;
        if (buttonObject != null)
        {
            Debug.Log($"ボタンが押されました: {buttonObject.name}");
            
            // trailObjectをボタンの位置まで移動
            if (trailObject != null)
            {
                Vector3 buttonPosition = buttonObject.transform.position;
                Vector3 trailPosition = new Vector3(buttonPosition.x, buttonPosition.y, buttonPosition.z + trailDepthOffset);
                trailObject.transform.position = trailPosition;
                Debug.Log($"trailObjectを{buttonObject.name}の位置に移動しました（奥行+{trailDepthOffset}）");
            }
        }
        else
        {
            Debug.Log("ボタンが押されました");
        }
    }
    
    // ボタンが離された時に呼ばれる関数
    public void OnButtonReleased(GameObject buttonObject = null)
    {
        isButtonPressed = false;
        if (buttonObject != null)
        {
            Debug.Log($"ボタンが離されました: {buttonObject.name}");
        }
        else
        {
            Debug.Log("ボタンが離されました");
        }
    }
    
    // ボタンにマウスが入った時に呼ばれる関数
    public void OnButtonEnter(GameObject buttonObject = null)
    {
        if (buttonObject != null)
        {
            Debug.Log($"ボタンにマウスが入りました: {buttonObject.name}");
            
            // ボタンが押されたままマウスが入った場合、trailObjectを移動
            if (isButtonPressed && trailObject != null)
            {
                Vector3 buttonPosition = buttonObject.transform.position;
                Vector3 trailPosition = new Vector3(buttonPosition.x, buttonPosition.y, buttonPosition.z + trailDepthOffset);
                trailObject.transform.position = trailPosition;
                Debug.Log($"ボタンが押されたままマウスが入ったため、trailObjectを{buttonObject.name}の位置に移動しました（奥行+{trailDepthOffset}）");
            }
        }
        else
        {
            Debug.Log("ボタンにマウスが入りました");
        }
    }
    
    // ボタンからマウスが出た時に呼ばれる関数
    public void OnButtonExit(GameObject buttonObject = null)
    {
        if (buttonObject != null)
        {
            Debug.Log($"ボタンからマウスが出ました: {buttonObject.name}");
        }
        else
        {
            Debug.Log("ボタンからマウスが出ました");
        }
    }
    
    // マウス位置をワールド座標に変換してtrailObjectを移動
    public void MoveTrailObjectToMousePosition()
    {
        if (trailObject != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
            trailObject.transform.position = worldPosition;
            Debug.Log($"trailObjectをマウス位置に移動しました: {worldPosition}");
        }
    }
}
