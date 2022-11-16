using UnityEngine;

public class AndroidInputActive : MonoBehaviour
{
    void Awake()
    {
#if UNITY_ANDROID
        this.gameObject.SetActive(true);
#endif

#if UNITY_STANDALONE_WIN
        this.gameObject.SetActive(false);
#endif
    }
}
