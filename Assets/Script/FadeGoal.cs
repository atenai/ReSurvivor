using UnityEngine;
using UnityEngine.UI;

public class FadeGoal : MonoBehaviour
{
    private float alfa = 0.0f ;


    private bool isFade = false;

    private void Start()
    {

    }

    private void Update()
    {
        if (Goal.b_GOAL)
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
            //ステージ１シーンへ
            StageSceneController.GameClear();
            isFade = false;
        }
    }
}
