using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    private Text GOAL_text;
    public static bool b_GOAL;
    private float ResultTime;

    //サウンド
    public AudioClip GOALSound;
    private AudioSource audioSource;

    public GameObject arrow;

    // Start is called before the first frame update
    private void Start()
    {

        //Componentを取得
        audioSource = GetComponent<AudioSource>();

        //Textコンポーネント取得
        GOAL_text = GameObject.Find("TextGOAL").GetComponent<Text>();
        GOAL_text.text = "";

        b_GOAL = false;
        ResultTime = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (b_GOAL)
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

            if (b_GOAL == false)
            {
                //SE再生
                //音(GOALSound)を鳴らす
                audioSource.PlayOneShot(GOALSound);
                b_GOAL = true;
            }
            other.gameObject.SetActive(false);
        }
    }

}
