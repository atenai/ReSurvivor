using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

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

    //private void OnCollisionEnter2D(Collision2D hit)
    //{
    //    if (hit.gameObject.CompareTag("Tank") || hit.gameObject.CompareTag("Kabe") || hit.gameObject.CompareTag("Enemy"))
    //    {
    //        Destroy(this.gameObject);//このオブジェクトを削除
    //    }
    //}
}
