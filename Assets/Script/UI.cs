using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    //リロード画像
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [SerializeField] GameObject imageReload;
    [SerializeField] Player3D player;
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

    void Start()
    {
        StartImageReload();

        StartTextMagazine();
    }

    void LateUpdate()
    {
        LateUpdateImageReload();

        LateUpdateTextMagazine();

        LateUpdateTimerSystem();
    }

    void StartImageReload()
    {
        imageReload.GetComponent<Image>().color = reloadColor;
    }

    void LateUpdateImageReload()
    {
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, RotateSpeed * Time.deltaTime);

        if (player.isReloadTimeActive == true)
        {
            if (reloadColor.a <= 1)
            {
                reloadColor.a += Time.deltaTime * 2.0f;
                imageReload.GetComponent<Image>().color = reloadColor;
            }
        }

        if (player.isReloadTimeActive == false)
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
        textMagazine.text = player.magazine.ToString();
    }

    void LateUpdateTextMagazine()
    {
        textMagazine.text = player.magazine.ToString();
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
            player.isGameOverTrigger = true;
            //現在のアニメーション（"Speed"）の値を持ってくる
            float animationCurrentPlayerMoveSpeed = player.anim.GetFloat("f_CurrentPlayerMoveSpeed");
            //移動アニメーションを徐々に「立ち」状態にする
            player.anim.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed - Time.deltaTime * 1.0f);
            StageSceneController.GameOver(player.GameOverDelay);
        }
        else
        {
            timerTMP.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
        }
    }
}
