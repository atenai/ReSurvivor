using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    private Color _reloadColor = new Color(255.0f, 255.0f, 255.0f);　　//Area1ディレクターテキストのカラー変数

    // オブジェクトの取得
    private GameObject _imageReloadObject;


    // Start is called before the first frame update
    private void Start()
    {
        //称号のカラーを取得してアルファを０に初期化
        //エリア1
        _reloadColor = new Color(255.0f, 255.0f, 255.0f);
        _reloadColor.a = 0;


        // オブジェクトの取得
        _imageReloadObject = GameObject.Find("ReloImage");

        _imageReloadObject.GetComponent<RawImage>().color = _reloadColor;

    }

    // Update is called once per frame
    private void Update()
    {

        if (Player3D.b_ReloadTimeActive)
        {
            if (_reloadColor.a <= 1)
            {
                //_reloadColor.a += fadeInSpeed; //アルファ値を徐々に＋する
                _reloadColor.a += Time.deltaTime * 2.0f; //アルファ値を徐々に＋する

                // 画像の透明度を変える
                _imageReloadObject.GetComponent<RawImage>().color = _reloadColor; //画像の透明度を変える

            }
        }

        if (Player3D.b_ReloadTimeActive == false)
        {
            if (_reloadColor.a >= 0)
            {
                //_reloadColor.a -= fadeInSpeed; //アルファ値を徐々に-する
                _reloadColor.a -= Time.deltaTime * 2.0f; //アルファ値を徐々に-する

                // 画像の透明度を変える
                _imageReloadObject.GetComponent<RawImage>().color = _reloadColor; //画像の透明度を変える

            }
        }

    }
}
