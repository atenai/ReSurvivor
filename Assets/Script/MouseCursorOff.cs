using UnityEngine;

public class MouseCursorOff : MonoBehaviour
{
    void Awake()
    {
#if UNITY_STANDALONE_WIN//端末がPCだった場合の処理
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif //終了
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
