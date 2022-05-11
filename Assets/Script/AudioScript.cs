using UnityEngine;
using UnityEngine.Serialization;

public class AudioScript : MonoBehaviour
{

    [FormerlySerializedAs("getSE")] public AudioClip getSe;
    private AudioSource _aud;

    private bool _bSe = true;

    // Start is called before the first frame update
    private void Start()
    {
        _aud = GetComponent<AudioSource>();
        _bSe = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_bSe)
        {
            _aud.PlayOneShot(getSe);
            _bSe = false;
        }
    }
}
