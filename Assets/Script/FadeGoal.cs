using UnityEngine;
using UnityEngine.UI;

public class FadeGoal : MonoBehaviour
{
    float alfa = 0.0f ;
    bool isFade = false;

    void Update()
    {
        if (Goal.isGOAL)
        {
            isFade = true;
        }

        if (isFade)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, alfa);
            alfa += Time.deltaTime;

        }

        if (alfa >= 1)
        {
            StageSceneController.GameClear();
            isFade = false;
        }
    }
}
