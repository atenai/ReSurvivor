using System;
using UnityEngine;
using UnityEngine.UI;

public class HpBarCtrl : MonoBehaviour
{
    private Slider _slider;

    private int _hp;
    private Player3D _player3D;

    // Start is called before the first frame update
    private void Start()
    {
        // スライダーを取得する
        _slider = GameObject.Find("HPSlider").GetComponent<Slider>();

        try
        {
            _player3D = GameObject.Find("Player").GetComponent<Player3D>();
            _hp = _player3D.GetPlayerHP();
        }
        catch (Exception)
        {

        }
    }

    

    // Update is called once per frame
    private void Update()
    {
        if(_player3D != null)
        {
            _hp = _player3D.GetPlayerHP();
        }

        //hp -= 1;
        if (_hp <= 0)
        {
            _hp = 0;
        }

        // HPゲージに値を設定
        _slider.value = _hp;
    }
}
