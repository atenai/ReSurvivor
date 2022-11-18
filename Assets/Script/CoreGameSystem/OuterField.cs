using UnityEngine;
using UnityEngine.UI;

public class OuterField : MonoBehaviour
{
    //テキスト
    private Text OuterField_text;

    //サウンド
    public AudioClip GOALSound;
    private AudioSource audioSource;

    //
    public static bool b_OuterField;

    // Start is called before the first frame update
    private void Start()
    {

        //Componentを取得
        audioSource = GetComponent<AudioSource>();

        //Textコンポーネント取得
        OuterField_text = GameObject.Find("TextOuterField").GetComponent<Text>();
        //テキストに文字を代入
        OuterField_text.text = "";

    }

    // Update is called once per frame
    private void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
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
