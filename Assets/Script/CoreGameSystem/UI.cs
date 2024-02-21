using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static UI singletonInstance = null;

    //リロード画像
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [SerializeField] GameObject imageReload;
    float RotateSpeed = -500.0f;

    //弾数
    [SerializeField] Text textMagazine;

    //カウントダウン
    [SerializeField] TextMeshProUGUI timerTMP;
    [SerializeField] int minute = 10;
    [SerializeField] float seconds = 0.0f;
    /// <summary>
    /// totalTImeは秒で集計されている
    /// </summary>
    float totalTime = 0.0f;

    //HP
    [SerializeField] Slider sliderHP;
    int hp;

    //ダメージ画像
    [SerializeField] public Image imageDamage;

    //ヒール画像
    [SerializeField] public Image imageHeal;

    //操作方法テキスト
    [SerializeField] GameObject textOperation;

    [SerializeField] Button shotButton;
    [SerializeField] Button reloadButton;
    [SerializeField] Button jumpButton;

    //広告
    [SerializeField] AdsInterstitial adsInterstitial;

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    void Start()
    {
        StartImageReload();

        StartTextMagazine();

        //ダメージ画像
        imageDamage.color = Color.clear;

        //ヒール画像
        imageHeal.color = Color.clear;

        StartOperation();

        StartAndroidInputButton();

        StartHP();
    }

    void LateUpdate()
    {
        LateUpdateImageReload();

        LateUpdateTextMagazine();

        LateUpdateTimerSystem();

        LateUpdateHP();
    }

    void StartImageReload()
    {
        imageReload.GetComponent<Image>().color = reloadColor;
    }

    void LateUpdateImageReload()
    {
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, RotateSpeed * Time.deltaTime);

        if (Player.singletonInstance.IsReloadTimeActive == true)
        {
            if (reloadColor.a <= 1)
            {
                reloadColor.a += Time.deltaTime * 2.0f;
                imageReload.GetComponent<Image>().color = reloadColor;
            }
        }

        if (Player.singletonInstance.IsReloadTimeActive == false)
        {
            if (reloadColor.a >= 0)
            {
                reloadColor.a -= Time.deltaTime * 2.0f;
                imageReload.GetComponent<Image>().color = reloadColor;
            }
        }
    }

    void StartTextMagazine()
    {
        textMagazine.text = Player.singletonInstance.Magazine.ToString();
    }

    void LateUpdateTextMagazine()
    {
        textMagazine.text = Player.singletonInstance.Magazine.ToString();
    }

    void LateUpdateTimerSystem()
    {
        totalTime = (minute * 60) + seconds;
        totalTime = totalTime - Time.deltaTime;

        minute = (int)totalTime / 60;
        seconds = totalTime - (minute * 60);

        if (minute <= 0 && seconds <= 0.0f)
        {
            timerTMP.text = "00" + ":" + "00";
            Player.singletonInstance.isGameOverTrigger = true;
            //現在のアニメーション（"Speed"）の値を持ってくる
            float animationCurrentPlayerMoveSpeed = Player.singletonInstance.anim.GetFloat("f_CurrentPlayerMoveSpeed");
            //移動アニメーションを徐々に「立ち」状態にする
            Player.singletonInstance.anim.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed - Time.deltaTime * 1.0f);
#if UNITY_ANDROID//端末がAndroidだった場合の処理
            adsInterstitial.ShowAd();//広告表示
#endif
            StageSceneController.GameOver(Player.singletonInstance.gameOverDelay);
        }
        else
        {
            timerTMP.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }
    }

    void StartHP()
    {
        hp = Player.singletonInstance.HP;
    }

    void LateUpdateHP()
    {
        hp = Player.singletonInstance.HP;

        //hp -= 1;
        if (hp <= 0)
        {
            hp = 0;
        }

        // HPゲージに値を設定
        sliderHP.value = hp;
    }

    void StartOperation()
    {
#if UNITY_ANDROID
        textOperation.gameObject.SetActive(false);
#endif

#if UNITY_STANDALONE_WIN
        textOperation.gameObject.SetActive(true);
#endif
    }

    /// <summary>
    /// android用のタッチ操作を追加する処理
    /// </summary>
    void StartAndroidInputButton()
    {
        shotButton.onClick.AddListener(() =>
        {
            Player.singletonInstance.ShootTouchButton();
        });

        reloadButton.onClick.AddListener(() =>
        {
            Player.singletonInstance.ReloadTouchButton();
        });

        jumpButton.onClick.AddListener(() =>
        {
            Player.singletonInstance.JumpTouchButton();
        });
    }
}
