using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    private float _alfa;
    private float _speed = 0.025f;
    private float _red, _green, _blue;

    public static bool BFade;

    //サウンド
    [FormerlySerializedAs("StartSound")] public AudioClip startSound;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        //Componentを取得
        _audioSource = GetComponent<AudioSource>();

        _alfa = 0.0f;
        BFade = false;
        _red = GetComponent<Image>().color.r;
        _green = GetComponent<Image>().color.g;
        _blue = GetComponent<Image>().color.b;
    }

    // Update is called once per frame
    private void Update()
    {

        if (BFade)
        {
            GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
            _alfa += _speed;
        }


        if (_alfa >= 1)
        {
            
            //ステージ１シーンへ
            SceneManager.LoadScene("StageScene1");
            BFade = false;
        }
    }
}
