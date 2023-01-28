using UnityEngine;

public class Firstaidkit : MonoBehaviour
{
    int hp;

    void Start()
    {
        hp = Player.singletonInstance.GetPlayerHP();
    }

    void OnTriggerEnter(Collider other)
    {
        hp = Player.singletonInstance.GetPlayerHP();

        if (other.CompareTag("Player") && hp < 100)
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }

}
