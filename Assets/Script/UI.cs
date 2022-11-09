using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    Color reloadColor = new Color(255.0f, 255.0f, 255.0f, 0.0f);
    [SerializeField] GameObject imageReload;
    [SerializeField] Player3D player;
    float RotateSpeed = -500.0f;

    [SerializeField] Text textMagazine;

    void Start()
    {
        imageReload.GetComponent<Image>().color = reloadColor;
        textMagazine.text = player.magazine.ToString();
    }

    void LateUpdate()
    {
        imageReload.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, RotateSpeed* Time.deltaTime);

        if (player.isReloadTimeActive == true)
        {
            if (reloadColor.a <= 1)
            {
                reloadColor.a += Time.deltaTime * 2.0f; //アルファ値を徐々に＋する
                imageReload.GetComponent<Image>().color = reloadColor; //画像の透明度を変える
            }
        }

        if (player.isReloadTimeActive == false)
        {
            if (reloadColor.a >= 0)
            {
                reloadColor.a -= Time.deltaTime * 2.0f; //アルファ値を徐々に-する
                imageReload.GetComponent<Image>().color = reloadColor; //画像の透明度を変える
            }
        }

        textMagazine.text = player.magazine.ToString();
    }
}
