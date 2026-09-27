using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RigidBodyを必ずアタッチするようにする
[RequireComponent(typeof(Rigidbody))]
public class MCPlayer : MonoBehaviour
{
    // Rigidbodyを保持する変数
    private Rigidbody rb;
    // 移動方向を保持する変数
    private Vector3 moveDistance;
    // 回転方向を保持する変数
    private float rotateDirection;
    // 回転を保持する変数
    private Quaternion rotateQuaternion;

    // ジャンプ入力を保持する変数
    private bool jump;
    // 現在のジャンプ回数を保持する変数
    private int jumpCount;

    [Header("プレイヤー設定")]
    // 移動速度
    [SerializeField]
    private float moveSpeed = 5f;
    // ジャンプ力
    [SerializeField]
    private float jumpForce = 5f;
    // 連続ジャンプの最大回数
    [SerializeField]
    private int jumpMaxCount = 1;

    [Header("カメラ")]
    [SerializeField]
    private MCCamera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotateQuaternion = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // 移動入力を取得
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        // ジャンプ入力を取得
        jump = Input.GetAxis("Jump") > 0;
        
        // ワールド座標系での移動方向
        Vector3 worldDistance = new Vector3(moveX, 0, moveZ);
        // ローカル座標系での移動方向
        moveDistance = transform.TransformDirection(worldDistance);

        // プレイヤーの回転をカメラのY軸回転に合わせる
        this.transform.rotation = Quaternion.LookRotation(playerCamera.rotateYAnchor.forward);
    }
    
    // 物理演算による移動処理
    private void FixedUpdate()
    {
        // Rigidbodyを使って移動
        rb.MovePosition(rb.position + moveDistance * moveSpeed * Time.fixedDeltaTime);

        // ジャンプ処理
        if (jump && jumpCount < jumpMaxCount)
        {
            rb.AddForce(this.transform.up * jumpForce, ForceMode.Impulse);
            jump = false;
            jumpCount++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 地面に接触したらジャンプ回数をリセット
        if (collision.gameObject.tag == "Ground")
        {
            jumpCount = 0;
        }
    }

}
