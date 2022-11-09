using UnityEngine;

public class ScreenSetting : MonoBehaviour
{

    private void Awake()
    {

#if UNITY_ANDROID//端末がAndroidだった場合の処理

#endif //終了

#if UNITY_IOS//端末がiPhoneだった場合の処理
        
#endif //終了

#if UNITY_STANDALONE//端末がPCだった場合の処理

        Screen.SetResolution(1920, 1080, true, 60);

#endif //終了

#if UNITY_EDITOR//Unityエディター上での処理

        Screen.SetResolution(1920, 1080, true, 60);

#endif //終了
    }

}
