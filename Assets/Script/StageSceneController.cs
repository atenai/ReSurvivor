using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneController : MonoBehaviour
{
    private static StageSceneController _instance;
    public static StageSceneController Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null) _instance = value;
            else if (_instance == value) { }
            else Destroy(_instance);
        }
    }
    
    public static Scene ActiveScene => SceneManager.GetActiveScene();

    [SerializeField] private GameObject coreObject;
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private GameObject gameClearObject;
    public static bool IsBossAlive { get; set; }
    private static readonly List<GameObject> RootGameObjects = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
        // 最大fps制限
        Application.targetFrameRate = 120;

        IsBossAlive = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameOverObject.SetActive(false);
        
        ActiveScene.GetRootGameObjects(RootGameObjects);
        RootGameObjects.Remove(coreObject);
    }

    public static void GameOver(float delay = 0)
    {
        Instance.StartCoroutine(ShowSubUI(Instance.gameOverObject, delay));
    }
    
    public static void GameClear(float delay = 0)
    {
        Instance.StartCoroutine(ShowSubUI(Instance.gameClearObject, delay));
    }

    private static IEnumerator ShowSubUI(GameObject ui, float delay = 0)
    {

        yield return new WaitForSeconds(delay);
        ui.SetActive(true);
        foreach (var o in RootGameObjects.Where(o => !(o == null) && o != ui))
        {
            o.SetActive(false);
        }
    }
    
    

    public static void ReloadCurrentScene()
    {
        SceneManager.LoadScene(ActiveScene.buildIndex);
    }
}
