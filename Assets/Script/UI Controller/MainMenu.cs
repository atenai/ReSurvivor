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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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
            //loadingImage.SetActive(true);
            //StartCoroutine(LoadSceneIcon("StageScene1"));
            StartCoroutine(LoadScene("StageScene1"));
        }

        public void ShowCredits()
        {
            //SceneManager.LoadScene("Credits");
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
            var load = SceneManager.LoadSceneAsync(sceneName);
            while (!load.isDone)
            {
                loadingImageIcon.transform.localPosition = new Vector3(-750 + 1500 * load.progress / 0.9f, -338.0f, 0);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator LoadScene(string sceneName)
        {
            slider.value = 0.0f;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;//シーン遷移を不許可

            while (slider.value < 1.0f)
            {
                seconds = seconds + (Time.deltaTime * 0.1f);

                //sliderBarがmax以下の時の計算
                if (slider.value < 0.9f)
                {
                    //Debug.Log("<color=yellow>seconds : " + this.seconds + "</color>");
                    slider.value = seconds;
                }

                //sliderBarがmaxになった時の計算
                if (0.9f <= slider.value)
                {
                    //Debug.Log("<color=green>seconds : " + this.seconds + "</color>");
                    slider.value = 0.9f;
                }

                //シーン遷移と終了処理
                if (0.9f <= asyncLoad.progress)
                {
                    slider.value = 1.0f;

                    yield return null;

                    asyncLoad.allowSceneActivation = true;//シーン遷移を許可
                }
            }
        }
    }
}
