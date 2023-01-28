using UnityEngine;
using UnityEngine.UI;

public class FadeGameOver : MonoBehaviour
{
    float alfa = 0.0f;
    [SerializeField] float fadeSpeed = 0.4f;

    void Update()
    {
        if (Player.singletonInstance.isGameOverTrigger)
        {
            GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, alfa);
            alfa += Time.deltaTime * fadeSpeed;
        }
    }
}
