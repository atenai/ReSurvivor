using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundSceneLoader : MonoBehaviour
{
    public GameObject loadingImage;
    public GameObject hideDuringLoading;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadBackground(sceneName));
    }

    private IEnumerator LoadBackground(string sceneName)
    {
        var load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone)
        {
            loadingImage.transform.localPosition = new Vector3(-750 + 1500 * load.progress / 0.9f, -338.0f, 0);
            yield return new WaitForEndOfFrame();
        }
    }
}
