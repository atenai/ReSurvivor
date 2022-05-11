using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI_Controller
{
    public class GameAfterScene : MonoBehaviour
    {
        public GameObject retryButton;
        public GameObject menuButton;
        public GameObject clearScreenFirstButton;

        private GameObject _selectedButton;

        private bool _allowSceneActivation;
    
        // Start is called before the first frame update
        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(clearScreenFirstButton);
            StartCoroutine(LoadScene());
        }

        // Update is called once per frame
        private void Update()
        {
            _selectedButton = EventSystem.current.currentSelectedGameObject;
            if(_selectedButton == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(clearScreenFirstButton);
            }
        }

        public void Retry()
        {
            _allowSceneActivation = true;
        }
    
        public void Return()
        {
            SceneManager.LoadScene("Title");
        }

        private IEnumerator LoadScene(string sceneName)
        {
            //Begin to load the Scene you specify
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    //Wait to you press the space key to activate the Scene
                    if (_allowSceneActivation)
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LoadScene()
        {
            return LoadScene(StageSceneController.ActiveScene.name);
        }
    }
}
