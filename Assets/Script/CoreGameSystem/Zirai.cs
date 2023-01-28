using UnityEngine;
using UnityEngine.Serialization;

public class Zirai : MonoBehaviour
{

    //爆発エフェクトのプレファブ
    [FormerlySerializedAs("ZiraiEffectPrefab")] public GameObject ziraiEffectPrefab;
    //private Vector3 RightEffectPosition;
    [FormerlySerializedAs("ZiraiEffectDestroyTime")] public float ziraiEffectDestroyTime;

    //煙エフェクトのプレファブ
    [FormerlySerializedAs("ZiraiSmokeEffectPrefab")] public GameObject ziraiSmokeEffectPrefab;
    //private Vector3 RightEffectPosition;
    [FormerlySerializedAs("ZiraiSmokeEffectDestroyTime")] public float ziraiSmokeEffectDestroyTime;

    //弾発射のSE
    [FormerlySerializedAs("ZiraiSEPrefab")] public GameObject ziraiSePrefab;
    [FormerlySerializedAs("ZiraiSE_Endtime")] public float ziraiSeEndtime;

    private GameObject Player { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SEオブジェクトを生成する
            var se = Instantiate(ziraiSePrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(se, ziraiSeEndtime);//SEをSE_Endtime後削除
            //エフェクトオブジェクトを生成する	
            var effect = Instantiate(ziraiEffectPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(effect, ziraiEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
            //エフェクトオブジェクトを生成する	
            var smokeEffect = Instantiate(ziraiSmokeEffectPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(smokeEffect, ziraiSmokeEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
            Player.GetComponent<Player>().SetPlayerDamage(50);
            Destroy(gameObject);//このオブジェクトを削除
        }
    }
}
