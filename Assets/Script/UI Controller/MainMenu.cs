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
        public GameObject startButton;
        public GameObject creditsButton;
        public GameObject quitButton;
        public GameObject fadeToGame;
        public GameObject loadingImage;
        public GameObject buttons;

        GameObject selectedButton;

        //bool isFade = false;

        //float alfa = 0.0f;
        //[SerializeField] float speed = 0.025f;
        //float red, green, blue;
    
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);

            //red = fadeToGame.GetComponent<Image>().color.r;
            //green = fadeToGame.GetComponent<Image>().color.g;
            //blue = fadeToGame.GetComponent<Image>().color.b;
        }

        private void Update()
        {
            selectedButton = EventSystem.current.currentSelectedGameObject;
            if(selectedButton == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(startButton);
            }

            //if (isFade)
            //{
            //    fadeToGame.GetComponent<Image>().color = new Color(red, green, blue, alfa);
            //    alfa += speed * Time.deltaTime;
            //}
            //if (alfa >= 1)
            //{
            //    //ステージ１シーンへ
            //    //SceneManager.LoadScene("StageScene1");
            //    isFade = false;
            //}
        }

        public void PlayGame()
        {
            buttons.SetActive(false);
            loadingImage.SetActive(true);
            StartCoroutine(LoadSceneBackground("StageScene1"));
        }

        public void ShowCredits()
        {
            SceneManager.LoadScene("Credits");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                              UnityEngine.Application.Quit();
#endif
        }

        private IEnumerator LoadSceneBackground(string sceneName)
        {
            var load = SceneManager.LoadSceneAsync(sceneName);
            while (!load.isDone)
            {
                loadingImage.transform.localPosition = new Vector3(-750 + 1500 * load.progress / 0.9f, -338.0f, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
