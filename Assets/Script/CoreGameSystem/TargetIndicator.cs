using UnityEngine;
using UnityEngine.UI;

//参考サイト
//https://qiita.com/o8que/items/46e486f62bdf05c29559

[RequireComponent(typeof(RectTransform))]//RectTransformをアタッチしていなかったらアタッチする
public class TargetIndicator : MonoBehaviour
{
    [SerializeField] Transform target;//ターゲット(玉orアイテム)の座標
    [SerializeField] Image arrow;//向きの画像

    Camera mainCamera;
    RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;//メインカメラを取得
        rectTransform = this.GetComponent<RectTransform>();//このスクリプトをアタッチしているゲームオブジェクトのRectTransformを取得
    }

    void LateUpdate()
    {
        if (target == null || target.gameObject.activeSelf == false)
        {
            transform.Find("Image_Arrow").gameObject.SetActive(false);
            return;
        }
        else
        {
            transform.Find("Image_Arrow").gameObject.SetActive(true);
        }

        //ルート(Canvas)のスケール値を取得する
        float canvasScale = transform.root.localScale.z;
        var screenCenter = 0.5f * new Vector3(Screen.width, Screen.height);//スクリーンの中心点全体の長さから0.5をかけることで求める[（例)2 * 0.5 = 1]

        //(画面中心を原点(0,0)とした)ターゲットのスクリーン座標を求める
        var targetIndicatorPos = mainCamera.WorldToScreenPoint(target.position) - screenCenter;//ワールド座標　→　スクリーン座標へ

        //カメラ後方にあるターゲットのスクリーン座標は、画面外に移動する
        if (targetIndicatorPos.z < 0f)//スクリーン座標.zが0以下の時中身を実行する
        {
            targetIndicatorPos.x = -targetIndicatorPos.x;
            targetIndicatorPos.y = -targetIndicatorPos.y;

            //カメラと水平なターゲットのスクリーン座標を補正する
            //pos.y == 0fならtrue
            if (Mathf.Approximately(targetIndicatorPos.y, 0f) == true)//Mathf.Approximatelyは浮動小数点数同士が等しいかどうかを比較したい場合、両者の差がある一定値以内ならほぼ等しい
            {
                targetIndicatorPos.y = -screenCenter.y;
            }
        }

        //画面端の表示位置を調整する
        //UI座標系の値をスクリーン座標系の値に変換する
        var halfSize = 0.5f * canvasScale * rectTransform.sizeDelta;//rectTransformのサイズの半分を求める

        //Mathf.Maxは一番大きい数値を返す、Mathf.Absは絶対値を返す//つまりここは絶対値の中で最も大きい数を求める
        //何故絶対値を求めるのかというと画面端は1か-1しかない為、
        float edgeOfScreen = Mathf.Max(Mathf.Abs(targetIndicatorPos.x / (screenCenter.x - halfSize.x)), Mathf.Abs(targetIndicatorPos.y / (screenCenter.y - halfSize.y)));//画面端のテキスト表示位置を求める

        //ターゲットのスクリーン座標が画面外なら、画面端になるように調整する
        //画面端が1か-1ならtrue
        bool isOffscreen = (targetIndicatorPos.z < 0f || 1f < edgeOfScreen);
        if (isOffscreen == true)
        {
            targetIndicatorPos.x = targetIndicatorPos.x / edgeOfScreen;
            targetIndicatorPos.y = targetIndicatorPos.y / edgeOfScreen;
        }

        //スクリーン座標系の値をUI座標系の値に変換する
        rectTransform.anchoredPosition = targetIndicatorPos / canvasScale;//ターゲットのスクリーン座標をこのUIのRectTransformに入れる

        //ターゲットのスクリーン座標が画面外なら、ターゲットの方向を指す矢印を表示する
        arrow.enabled = isOffscreen;
        if (isOffscreen == true)
        {
            arrow.rectTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(-targetIndicatorPos.x, targetIndicatorPos.y) * Mathf.Rad2Deg);//矢印を回転させてあげる処理をしている
        }
    }
}