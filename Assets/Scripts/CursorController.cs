using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    // カーソルがロックされているかどうかを管理するフラグ変数
    private bool isCursorLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        // ゲーム開始時にカーソルのロック状態を設定する
        SetCursorLock(isCursorLocked);
    }

    // Update is called once per frame
    void Update()
    {
        // ESCキーが押されたらカーソルのロック状態を切り替える
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // フラグ変数を反転させる
            isCursorLocked = !isCursorLocked;
            // カーソルのロック状態を設定する
            SetCursorLock(isCursorLocked);
        }
    }

    // カーソルのロック状態を設定する関数
    // 引数lockedがtrueの場合はカーソルをロックし、falseの場合はカーソルを解放する
    private void SetCursorLock(bool locked)
    {
        if(locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        // カーソルの可視状態を設定する
        Cursor.visible = !locked;
    }
}
