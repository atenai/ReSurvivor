using UnityEngine;
using UnityEngine.UI;

public class MagazineUI : MonoBehaviour
{
    private Text _MagazineUIText;

    private int _MagazineUINum;

    // Start is called before the first frame update
    private void Start()
    {
        _MagazineUINum = 0;//スコア初期化
        //Textコンポーネント取得
        _MagazineUIText = GetComponent<Text>();
        //テキストの文字入力
        _MagazineUIText.text = _MagazineUINum.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        _MagazineUINum = Player3D.Magazine;
        //テキストの文字入力
        _MagazineUIText.text = _MagazineUINum.ToString();
    }
}
