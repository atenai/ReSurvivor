using UnityEngine;
using UnityEngine.UI;

public class Tama : MonoBehaviour
{
    private Text _tamaText;
    //トータルスコア
    private int _tamaNum;

    // Start is called before the first frame update
    private void Start()
    {
        _tamaNum = 0;//スコア初期化
        //Textコンポーネント取得
        _tamaText = GetComponent<Text>();
        //テキストの文字入力
        _tamaText.text = " " + _tamaNum;
    }

    // Update is called once per frame
    private void Update()
    {
        _tamaNum = Player3D.Magazine;
        //テキストの文字入力
        _tamaText.text = " " + _tamaNum;
    }
}
