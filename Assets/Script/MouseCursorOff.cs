using UnityEngine;

public class MouseCursorOff : MonoBehaviour
{
    void Awake()
    {
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

#if UNITY_EDITOR//Unityエディター上での処理
    void Update()
    {
        //Cキーでマウスカーソルを出す
        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
#endif //終了
}
