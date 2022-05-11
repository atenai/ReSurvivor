using UnityEngine;
using UnityEngine.UI;

public class PressAnyKey : MonoBehaviour
{
    private bool _bStart;

    private Text _pressAnyKeyText;


    private float _alfa;//テキスト・α値
    private const float MAX = 1.0f;
    private const float MIN = 0.0f;
    private bool _bAlfa;
    private float _plasAlfa = 0.015f;




    // Start is called before the first frame update
    private void Start()
    {
        _bStart = false;

        //Textコンポーネント取得
        _pressAnyKeyText = GameObject.Find("Press Any Key").GetComponent<Text>();
        //direct_text.text = "エリア0";
        //
        //テキストカラー初期化
        _pressAnyKeyText.color = new Color(0.0f, 255.0f, 255.0f, _alfa);

        _plasAlfa = 0.015f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _bStart = true;
        }

        if (_bStart == false)
        {
            _pressAnyKeyText.text = "Press Any Key";
        }
        else
        {
            _pressAnyKeyText.text = "";
        }
        //
        if (_alfa >= MAX) _bAlfa = true;
        if (_alfa <= MIN) _bAlfa = false;

        if (_bAlfa) _alfa -= _plasAlfa;
        else
        {
            _alfa += _plasAlfa;
        }
        _pressAnyKeyText.color = new Color(255.0f, 255.0f, 0.0f, _alfa);
    }
}
