using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI_Controller
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject startButton;
        [SerializeField] GameObject creditsButton;
        [SerializeField] GameObject quitButton;
        [SerializeField] GameObject fadeToGame;
        [SerializeField] GameObject loadingImageIcon;
        [SerializeField] GameObject buttons;

        GameObject selectedButton;

        [SerializeField] Slider slider;
        float seconds = 0.0f;

        void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);
        }

        void Update()
        {
            selectedButton = EventSystem.current.currentSelectedGameObject;
            if (selectedButton == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(startButton);
            }
        }

        public void PlayGame()
        {
            buttons.SetActive(false);
            //StartCoroutine(LoadSceneIcon("StageScene1"));
            StartCoroutine(LoadScene("StageScene1"));
        }

        public void ShowCredits()
        {
            //StartCoroutine(LoadSceneIcon("Credits"));
            StartCoroutine(LoadScene("Credits"));
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE || UNITY_ANDROID
            UnityEngine.Application.Quit();
#endif
        }

        IEnumerator LoadSceneIcon(string sceneName)
        {
            loadingImageIcon.SetActive(true);
            var load = SceneManager.LoadSceneAsync(sceneName);
            while (!load.isDone)
            {
                loadingImageIcon.transform.localPosition = new Vector3(-750 + 1500 * load.progress / 0.9f, -338.0f, 0);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator LoadScene(string sceneName)
        {
            slider.value = float.MinValue;

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;//シーン遷移を不許可

            while (async.isDone == false)
            {
                slider.value = async.progress;

                if (0.9f <= async.progress)
                {
                    slider.value = float.MaxValue;

                    yield return new WaitForEndOfFrame();

                    async.allowSceneActivation = true;//シーン遷移を許可
                }

                yield return null;
            }
        }
    }
}
