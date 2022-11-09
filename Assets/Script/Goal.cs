using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    private Text GOAL_text;
    public static bool isGOAL;
    private float ResultTime;

    //サウンド
    public AudioClip GOALSound;
    private AudioSource audioSource;

    public GameObject arrow;

    private void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();

        //Textコンポーネント取得
        GOAL_text = GameObject.Find("TextGOAL").GetComponent<Text>();
        GOAL_text.text = "";

        isGOAL = false;
        ResultTime = 0.0f;
    }

    private void Update()
    {
        if (isGOAL)
        {
            //Debug.Log("ゴール当たっているよ");
            ResultTime -= Time.deltaTime;
        }

        //　制限時間が0秒以下なら何もしない
        //if (ResultTime <= -0.5f)
        //{
        //    //リザルトへ
        //    StageSceneController.GameClear();

        //}
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            arrow.SetActive(false);

            GOAL_text.text = "GOAL";

            if (isGOAL == false)
            {
                //SE再生
                //音(GOALSound)を鳴らす
                audioSource.PlayOneShot(GOALSound);
                isGOAL = true;
            }
            other.gameObject.SetActive(false);
        }
    }

}
