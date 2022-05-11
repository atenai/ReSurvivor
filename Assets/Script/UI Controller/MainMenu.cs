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

        private GameObject _selectedButton;

        public static bool BFade;

        private float _alfa;
        private float _speed = 0.025f;
        private float _red, _green, _blue;
    
        // Start is called before the first frame update
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(startButton);

            _alfa = 0.0f;
            BFade = false;
            _red = fadeToGame.GetComponent<Image>().color.r;
            _green = fadeToGame.GetComponent<Image>().color.g;
            _blue = fadeToGame.GetComponent<Image>().color.b;
        }

        // Update is called once per frame
        private void Update()
        {
            _selectedButton = EventSystem.current.currentSelectedGameObject;
            if(_selectedButton == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(startButton);
            }

            if (BFade)
            {
                fadeToGame.GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
                _alfa += _speed;
            }
            if (_alfa >= 1)
            {
                //ステージ１シーンへ
                SceneManager.LoadScene("StageScene1");
                BFade = false;
            }
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
