using UnityEngine;
using UnityEngine.UI;

public class FlushController : MonoBehaviour
{
    private Image _img;

    private bool _bCameraDamageEffect;

    // Start is called before the first frame update
    private void Start()
    {
        _img = GetComponent<Image>();
        _img.color = Color.clear;

        //_bCameraDamageEffect = GameObject.Find("Player").GetComponent<Player3D>().b_DamageEffect;
    }

    // Update is called once per frame
    private void Update()
    {

        //_bCameraDamageEffect = GameObject.Find("Player").GetComponent<Player3D>().b_DamageEffect;

        Debug.Log(_bCameraDamageEffect);

        if (_bCameraDamageEffect)
        {
            //Debug.Log("PlayerDamageTure");
            _img.color = new Color(0.5f, 0f, 0f, 0.5f);

        }

        if (_bCameraDamageEffect == false)
        {
            _img.color = Color.Lerp(_img.color, Color.clear, Time.deltaTime);
            //Debug.Log("PlayerDamageFalse");
        }
    }
}
