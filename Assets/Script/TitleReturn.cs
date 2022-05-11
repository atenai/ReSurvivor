using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleReturn : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            //ステージ１シーンへ
            SceneManager.LoadScene("Title");
        }
    }
}
