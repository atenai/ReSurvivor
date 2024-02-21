using UnityEngine;

public class Firstaidkit : MonoBehaviour
{
    void Start()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && Player.singletonInstance.HP < 100)
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }

}
