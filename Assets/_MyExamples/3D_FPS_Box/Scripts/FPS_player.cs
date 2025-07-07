using UnityEngine;

public class FPS_player : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度
    public float rotSpeed = 90f; // 回転速度
    public Rigidbody rb; // プレイヤーのRigidbody
    public Transform mainCamera; // メインカメラのTransform

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Rigidbodyの参照を取得
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // メインカメラの参照を取得
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの移動処理
        float moveX = Input.GetAxis("Horizontal"); // 左右移動
        float moveZ = Input.GetAxis("Vertical");   // 前後移動

        Vector3 moveDirection = transform.TransformDirection(new Vector3(moveX, 0, moveZ).normalized);
        rb.linearVelocity = moveDirection * moveSpeed;

        // カメラの追従処理
        Vector3 cameraPosition = transform.position - transform.forward * 5f + Vector3.up * 2f;
        mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPosition, Time.deltaTime * 5f);
        mainCamera.LookAt(transform.position);

        // カーソルキーによるY軸回転処理
        float rotationY = Input.GetAxis("HorizontalArrow"); // カーソルキー左右の入力
        transform.Rotate(0, rotationY * rotSpeed * Time.deltaTime, 0);
    }
}
