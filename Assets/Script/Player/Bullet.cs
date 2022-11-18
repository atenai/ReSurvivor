using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Boss") || collision.gameObject.CompareTag("Tank") || collision.CompareTag("Kabe") || collision.CompareTag("Enemy") || collision.CompareTag("technical"))
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Tank") || collision.gameObject.CompareTag("Kabe") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("technical"))
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }
}
