using UnityEngine;

public class Firstaidkit : MonoBehaviour
{
    private int _hp;

    // Start is called before the first frame update
    private void Start()
    {
        //HP = 100;
        _hp = GameObject.Find("Player").GetComponent<Player3D>().GetPlayerHP();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player") && HP < 100)
    //    {
    //        Destroy(this.gameObject);//このオブジェクトを削除
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        _hp = GameObject.Find("Player").GetComponent<Player3D>().GetPlayerHP();

        if (other.CompareTag("Player") && _hp < 100)
        {
            Destroy(gameObject);//このオブジェクトを削除
        }
    }

}
