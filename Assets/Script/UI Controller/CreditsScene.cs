using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI_Controller
{
    public class CreditsScene : MonoBehaviour
    {
        public GameObject creditsFirstButton;
        public GameObject thanksButton;
        public GameObject staffButton;
        public GameObject licenceButton;
        public GameObject returnButton;
        public GameObject thanksPanel;
        public GameObject staffPanel;
        public GameObject licencePanel;

        private GameObject _selectedButton;
    
        // Start is called before the first frame update
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(creditsFirstButton);
            thanksPanel.SetActive(false);
            staffPanel.SetActive(false);
            licencePanel.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            _selectedButton = EventSystem.current.currentSelectedGameObject;

            if(_selectedButton == thanksButton)
            {
                thanksPanel.SetActive(true);
                staffPanel.SetActive(false);
                licencePanel.SetActive(false);
            }
            else if(_selectedButton == staffButton)
            {
                thanksPanel.SetActive(false);
                staffPanel.SetActive(true);
                licencePanel.SetActive(false);
            }
            else if(_selectedButton == licenceButton)
            {
                thanksPanel.SetActive(false);
                staffPanel.SetActive(false);
                licencePanel.SetActive(true);
            }
            else if(_selectedButton == returnButton)
            {
                thanksPanel.SetActive(false);
                staffPanel.SetActive(false);
                licencePanel.SetActive(false);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(thanksButton);
            }
        }

        public void Return()
        {
            SceneManager.LoadScene("Title");
        }
    }
}
