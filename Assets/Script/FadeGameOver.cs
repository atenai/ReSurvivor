using UnityEngine;
using UnityEngine.UI;

public class FadeGameOver : MonoBehaviour
{
    private float _alfa;
    //private float _speed = 0.0275f;
    private float _red, _green, _blue;

    // Start is called before the first frame update
    private void Start()
    {
        _alfa = 0.0f;
        _red = GetComponent<Image>().color.r;
        _green = GetComponent<Image>().color.g;
        _blue = GetComponent<Image>().color.b;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Player3D.b_GameOverTrigger)
        {
            GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
            _alfa += Time.deltaTime * 0.4f;

        }
    }
}
