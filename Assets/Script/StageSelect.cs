using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    public Image stage1;
    public Image stage2;
    public Image stage3;

    int i_Num = 1;

    public AudioClip SelectSE;
    AudioSource SelectAud;

    public Image fade;

    private float _alfa;
    private float alfaDefine = 1.0f;

    private bool _bFade;
    private bool b_stage1;
    private bool b_stage2;
    private bool b_stage3;

    // Start is called before the first frame update
    void Start()
    {
        SelectAud = GetComponent<AudioSource>();
        stage1 = GameObject.Find("ImageStage1").GetComponent<Image>();//上で作ったImage型のstage1にImageStage1を見つけてImageクラスの機能を入れる
        stage2 = GameObject.Find("ImageStage2").GetComponent<Image>();//上で作ったImage型のstage2にImageStage2を見つけてImageクラスの機能を入れる
        stage3 = GameObject.Find("ImageStage3").GetComponent<Image>();//上で作ったImage型のstage3にImageStage3を見つけてImageクラスの機能を入れる

        fade = GameObject.Find("ImageFade").GetComponent<Image>();
        _alfa = 0.0f;
        _bFade = false;
        b_stage1 = false;
        b_stage2 = false;
        b_stage3 = false;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.A) && _bFade == false || Input.GetKeyDown(KeyCode.LeftArrow) && _bFade == false)//もし左矢印キーを押したら下の内容を実行
        {
            i_Num = i_Num - 1;
        }

        if (Input.GetKeyDown(KeyCode.D) && _bFade == false || Input.GetKeyDown(KeyCode.RightArrow) && _bFade == false)//もし右矢印キーを押したら下の内容を実行
        {
            i_Num = i_Num + 1;
        }

        if (i_Num == 1)//ステージ1を選択中
        {
            stage1.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);//ステージ1の色を変える
            stage2.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ2の色を変える
            stage3.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ3の色を変える

            if (Input.GetKeyDown("return") && _bFade == false || Input.GetKeyDown(KeyCode.Space) && _bFade == false)//もしエンターキーを押したら下の内容を実行
            {
                SelectAud.PlayOneShot(SelectSE);
                _bFade = true;
                b_stage1 = true;
            }
        }
        else if (i_Num == 2)//ステージ2を選択中
        {
            stage1.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ1の色を変える
            stage2.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);//ステージ2の色を変える
            stage3.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ3の色を変える

            if (Input.GetKeyDown("return") && _bFade == false || Input.GetKeyDown(KeyCode.Space) && _bFade == false)//もしエンターキーを押したら下の内容を実行
            {
                SelectAud.PlayOneShot(SelectSE);
                _bFade = true;
                b_stage2 = true;
            }
        }
        else if (i_Num == 3)//ステージ3を選択中
        {
            stage1.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ1の色を変える
            stage2.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);//ステージ2の色を変える
            stage3.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);//ステージ3の色を変える

            if (Input.GetKeyDown("return") && _bFade == false || Input.GetKeyDown(KeyCode.Space) && _bFade == false)//もしエンターキーを押したら下の内容を実行
            {
                SelectAud.PlayOneShot(SelectSE);
                _bFade = true;
                b_stage3 = true;
            }
        }
        else if (i_Num == 4)
        {
            i_Num = 1;
        }
        else if (i_Num == 0)
        {
            i_Num = 3;
        }

        if (_bFade)
        {
            fade.color = new Color(0.0f, 0.0f, 0.0f, _alfa);
            _alfa += Time.deltaTime;
        }

        //Debug.Log(_alfa);

        if (alfaDefine <= _alfa && b_stage1)
        {
            SceneManager.LoadScene("StageScene1");//ステージ1シーンに飛ぶ
            _bFade = false;
        }

        if (alfaDefine <= _alfa && b_stage2)
        {
            StageSceneController.GameClear();//ステージ2シーンに飛ぶ
            _bFade = false;
        }

        // if (alfaDefine <= _alfa && b_stage3)
        // {
        //     SceneManager.LoadScene("GameOver");//ステージ3シーンに飛ぶ
        //     _bFade = false;
        // }
    }

}
