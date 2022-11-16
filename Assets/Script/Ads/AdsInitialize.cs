using UnityEngine;
using UnityEngine.Advertisements;

//参考サイト
//https://www.youtube.com/watch?v=tzgOTVPXC-I
//https://www.youtube.com/watch?v=gzTs3WpzhWg

//必ずADSのバージョンを4.3.0にすること！！

//広告全体の初期化
public class AdsInitialize : MonoBehaviour, IUnityAdsInitializationListener
{

    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = false;
    private string _gameId;

    //[SerializeField] AdsRewarded adsRewardedButton;
    [SerializeField] AdsInterstitial adsInterstitialButton;

    void Awake()
    {
        InitializeAds();
    }

    //広告の初期化処理
    public void InitializeAds()
    {
        //Debug.Log("<color=yellow>UnityAdsが初期化開始</color>");
        //iOSかAndroidのどちらのプラットフォームかを取得して広告IDを取得する
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        //広告の初期化処理(第一引数に広告ID, 第二引数にテストモードかどうか?, 第三引数はわからない)
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    //初期化処理が完了した際に実行する
    public void OnInitializationComplete()
    {
        //Debug.Log("<color=blue>UnityAdsが初期化の成功</color>");
        //リワード広告をロードする
        //adsRewardedButton.LoadAd();
        //インターステーショナル広告をロードする
        adsInterstitialButton.LoadAd();
    }

    //初期化処理が失敗した場合に実行する
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //Debug.Log("<color=red>UnityAdsが初期化処理に失敗</color>");
        //Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
