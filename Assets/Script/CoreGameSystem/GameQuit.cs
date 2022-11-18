using UnityEditor;
using UnityEngine;

public class GameQuit : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        //Escapeキーでゲーム終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();//ゲーム終了
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
                              UnityEngine.Application.Quit();
        #endif
    }
}
