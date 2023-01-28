using UnityEngine;

public class Firstaidkit : MonoBehaviour
{
    int hp;

    void Start()
    {
        hp = GameObject.Find("Player").GetComponent<Player>().GetPlayerHP();
    }

    void OnTriggerEnter(Collider other)
    {
        hp = GameObject.Find("Player").GetComponent<Player>().GetPlayerHP();

        if (other.CompareTag("Player") && hp < 100)
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }

}
