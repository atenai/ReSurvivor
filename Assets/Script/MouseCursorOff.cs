using UnityEngine;

public class MouseCursorOff : MonoBehaviour
{

    private void Awake()
    {
        //マウスカーソルを消す
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        //Escapeキーでマウスカーソルを出す
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;

        }
    }
}
