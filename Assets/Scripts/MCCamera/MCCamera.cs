using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class MCCamera : MonoBehaviour
{
    // カメラアンカーの初期位置を保持する変数
    private Vector3 defPosition;
    // マウス入力を保持する変数（X軸とY軸の回転量）
    private float mouseInputX;
    private float mouseInputY;

    private float rotateX;
    private float rotateY;

    [Header("カメラ設定")]
    // マウス感度を保持する変数
    public float mouseSensitivity = 5f;
    // X軸の最大回転角度を保持する変数
    [SerializeField]
    private float maxRotationX = 90f;
    // ThirdPersonBack用のカメラ位置を保持する変数
    [SerializeField]
    private Vector3 thirdPersonBackPosition;
    // ThirdPersonFront用のカメラ位置を保持する変数
    [SerializeField]
    private Vector3 thirdPersonFrontPosition;
    // カメラのモードを保持する変数
    public CameraMode cameraMode = CameraMode.FirstPerson;

    [Header("Anchor(アンカー)設定")]
    // カメラアンカーを保持する変数
    [SerializeField]
    private Transform cameraAnchor;
    // Y軸回転用アンカー
    public Transform rotateYAnchor;
    // X軸回転用アンカー
    public Transform rotateXAnchor;
    // CameraのTransformを保持する変数
    [SerializeField]
    private Transform mainCamera;


    [Header("追従オブジェクト")]
    // 追従するオブジェクトを保持する変数
    [SerializeField]
    private Transform followTarget;

    // Start is called before the first frame update
    void Start()
    {
        // 各Transformが設定されていない場合は自動で取得する処理
        if(mainCamera == null)
        {
            mainCamera = this.gameObject.GetComponentInChildren<Camera>().transform;
        }
        if(cameraAnchor == null)
        {
            cameraAnchor = this.transform;
        }
        if(rotateYAnchor == null)
        {
            rotateYAnchor = cameraAnchor.Find("RotateYAnchor");
        }
        if(rotateXAnchor == null)
        {
            rotateXAnchor = rotateYAnchor.Find("RotateXAnchor");
        }

        // Cameraアンカーの初期位置を計算して保持する
        defPosition = cameraAnchor.position - followTarget.position;
        // カメラモードに応じて初期化処理を行う
        InitCameraMode();
    }

    // Update is called once per frame
    void Update()
    {
        // カメラモード切替
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(cameraMode == CameraMode.FirstPerson)
            {
                cameraMode = CameraMode.ThirdPersonBack;
            }
            else if(cameraMode == CameraMode.ThirdPersonBack)
            {
                cameraMode = CameraMode.ThirdPersonFront;
            }
            else if(cameraMode == CameraMode.ThirdPersonFront)
            {
                cameraMode = CameraMode.FirstPerson;
            }
            InitCameraMode();
        }

        // カメラアンカーの座標を更新
        // 追従オブジェクトの位置を更新
        if(followTarget != null)
        {
            cameraAnchor.position = followTarget.position + defPosition;
        }

        // カメラの回転を更新
        // マウス入力を取得
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
        
        if(cameraMode == CameraMode.FirstPerson)
        {
            FirstPersonRotation();
        }
        else if(cameraMode == CameraMode.ThirdPersonBack)
        {
            ThirdPersonBackRotation();
        }
        else if(cameraMode == CameraMode.ThirdPersonFront)
        {
            ThirdPersonFrontRotation();
        }

    }

    // Cameraの初期化処理
    private void InitCameraMode()
    {
        if(cameraMode == CameraMode.FirstPerson)
        {
            // FirstPerson用の初期化処理
            // Cameraの位置をリセットする
            rotateXAnchor.localPosition = Vector3.zero;
            rotateYAnchor.localPosition = Vector3.zero;
            mainCamera.localPosition = Vector3.zero;

            // rotateXAnchorとrotateYAnchorの回転をリセットする
            // Cameraの回転をリセットする
            rotateXAnchor.localRotation = Quaternion.LookRotation(followTarget.forward);
            rotateYAnchor.localRotation = Quaternion.LookRotation(followTarget.forward);
            mainCamera.localRotation = Quaternion.identity;
        }
        else if(cameraMode == CameraMode.ThirdPersonBack)
        {
            // ThirdPersonBack用の初期化処理
            // Cameraの位置をリセットする
            rotateXAnchor.localPosition = Vector3.zero;
            rotateYAnchor.localPosition = Vector3.zero;
            mainCamera.localPosition = thirdPersonBackPosition;

            // rotateXAnchorとrotateYAnchorの回転をリセットする
            // Cameraの回転をリセットする
            mainCamera.LookAt(cameraAnchor);
        }
        else if(cameraMode == CameraMode.ThirdPersonFront)
        {
            // ThirdPersonFront用の初期化処理
            // Cameraの位置をリセットする
            rotateXAnchor.localPosition = Vector3.zero;
            rotateYAnchor.localPosition = Vector3.zero;
            mainCamera.localPosition = thirdPersonFrontPosition;

            // rotateXAnchorとrotateYAnchorの回転をリセットする
            // Cameraの回転をリセットする
            mainCamera.LookAt(cameraAnchor);
        }
    }

    // FirstPersonモードでのカメラ回転処理
    private void FirstPersonRotation()
    {
        // マウス入力に基づいて回転角度を更新
        // rotateXはmaxRotationXの角度に制限をする(Mathf.Clampを使用)
        rotateX = Mathf.Clamp(rotateX + -mouseInputY * mouseSensitivity * Time.deltaTime, -maxRotationX, maxRotationX);
        // rotateYは特に制限を設けない
        rotateY += mouseInputX * mouseSensitivity * Time.deltaTime;
        
        // 回転クォータニオンを作成
        Quaternion rotationY = Quaternion.Euler(0f, rotateY, 0f);
        Quaternion rotationX = Quaternion.Euler(rotateX, 0f, 0f);
        
        // 回転を適用
        rotateYAnchor.localRotation = rotationY;
        rotateXAnchor.localRotation = rotationX;
    }

    // ThirdPersonBackモードでのカメラ回転処理
    private void ThirdPersonBackRotation()
    {
        // マウス入力に基づいて回転角度を更新
        // rotateXはmaxRotationXの角度に制限をする(Mathf.Clampを使用)
        rotateX = Mathf.Clamp(rotateX + -mouseInputY * mouseSensitivity * Time.deltaTime, -maxRotationX, maxRotationX);
        // rotateYは特に制限を設けない
        rotateY += mouseInputX * mouseSensitivity * Time.deltaTime;
        
        // 回転クォータニオンを作成
        Quaternion rotationY = Quaternion.Euler(0f, rotateY, 0f);
        Quaternion rotationX = Quaternion.Euler(rotateX, 0f, 0f);
        
        // 回転を適用
        rotateYAnchor.localRotation = rotationY;
        rotateXAnchor.localRotation = rotationX;
    }

    // ThirdPersonFrontモードでのカメラ回転処理
    private void ThirdPersonFrontRotation()
    {
        // マウス入力に基づいて回転角度を更新
        // rotateXはmaxRotationXの角度に制限をする(Mathf.Clampを使用)
        rotateX = Mathf.Clamp(rotateX + -mouseInputY * mouseSensitivity * Time.deltaTime, -maxRotationX, maxRotationX);
        // rotateYは特に制限を設けない(FrontモードではrotateYは逆方向に回転する)
        rotateY += -mouseInputX * mouseSensitivity * Time.deltaTime;
        
        // 回転クォータニオンを作成
        Quaternion rotationY = Quaternion.Euler(0f, rotateY, 0f);
        Quaternion rotationX = Quaternion.Euler(rotateX, 0f, 0f);
        
        // 回転を適用
        rotateYAnchor.localRotation = rotationY;
        rotateXAnchor.localRotation = rotationX;
    }
}

// カメラモードの列挙型
public enum CameraMode
{
    FirstPerson,
    ThirdPersonBack,
    ThirdPersonFront,
}
