using UnityEngine;
using UnityEngine.UI;

public class FadeGoal : MonoBehaviour
{
    private float _alfa;
    //private float _speed = 0.007f;
    private float _red, _green, _blue;

    private bool _bFade;

    // Start is called before the first frame update
    private void Start()
    {
        _alfa = 0.0f;
        _bFade = false;
        _red = GetComponent<Image>().color.r;
        _green = GetComponent<Image>().color.g;
        _blue = GetComponent<Image>().color.b;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Goal.b_GOAL)
        {
            _bFade = true;
        }

        if (_bFade)
        {
            GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
            _alfa += Time.deltaTime;

        }

        if (_alfa >= 1)
        {
            //ステージ１シーンへ
            StageSceneController.GameClear();
            _bFade = false;
        }
    }
}
