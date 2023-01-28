using UnityEngine;
using UnityEngine.Serialization;

public class Zirai : MonoBehaviour
{

    //爆発エフェクトのプレファブ
    [FormerlySerializedAs("ZiraiEffectPrefab")] public GameObject ziraiEffectPrefab;
    [FormerlySerializedAs("ZiraiEffectDestroyTime")] public float ziraiEffectDestroyTime;

    //煙エフェクトのプレファブ
    [FormerlySerializedAs("ZiraiSmokeEffectPrefab")] public GameObject ziraiSmokeEffectPrefab;
    [FormerlySerializedAs("ZiraiSmokeEffectDestroyTime")] public float ziraiSmokeEffectDestroyTime;

    //地雷のSE
    [FormerlySerializedAs("ZiraiSEPrefab")] public GameObject ziraiSePrefab;
    [FormerlySerializedAs("ZiraiSE_Endtime")] public float ziraiSeEndtime;

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
            Player.singletonInstance.SetPlayerDamage(50);
            Destroy(gameObject);//このオブジェクトを削除
        }
    }
}
