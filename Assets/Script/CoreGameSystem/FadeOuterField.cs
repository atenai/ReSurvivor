using UnityEngine;
using UnityEngine.UI;

public class FadeOuterField : MonoBehaviour
{
    private float _alfa;
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
        if (OuterField.b_OuterField)
        {
            _bFade = true;
        }

        if (_bFade)
        {
            GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
            _alfa += Time.deltaTime;

        }

        if (1 <= _alfa)
        {
            //ステージ１シーンへ
            StageSceneController.GameOver();
            _bFade = false;
        }
    }
}
