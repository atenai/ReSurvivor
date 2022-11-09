using UnityEngine;
using UnityEngine.UI;

public class FadeGameOver : MonoBehaviour
{
    float alfa = 0.0f;
    [SerializeField] float fadeSpeed = 0.4f;
    [SerializeField] Player3D player;

    void Update()
    {
        if (player.isGameOverTrigger)
        {
            GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, alfa);
            alfa += Time.deltaTime * fadeSpeed;
        }
    }
}
