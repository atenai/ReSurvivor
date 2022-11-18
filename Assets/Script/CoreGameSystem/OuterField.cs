using UnityEngine;
using UnityEngine.UI;

public class OuterField : MonoBehaviour
{
    //テキスト
    Text OuterField_text;

    //サウンド
    public AudioClip GOALSound;
    AudioSource audioSource;

    public static bool b_OuterField;

    void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();

        //Textコンポーネント取得
        OuterField_text = GameObject.Find("TextOuterField").GetComponent<Text>();
        //テキストに文字を代入
        OuterField_text.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("フィールド外");

            b_OuterField = true;

            //テキストに文字を代入
            OuterField_text.text = "エリア外";

            //SE再生
            //音(GOALSound)を鳴らす
            audioSource.PlayOneShot(GOALSound);

            //ゲームオーバー
            StageSceneController.GameOver();
        }
    }
}
