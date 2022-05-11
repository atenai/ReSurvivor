using UnityEngine;

public class TankShellController : MonoBehaviour
{
    private int _attack;

    // Start is called before the first frame update
    private void Start()
    {
        //攻撃力
        _attack = 100;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                //プレイヤーのHPを減らす
                collision.GetComponent<Player3D>().SetPlayerDamage(_attack);
                Destroy(gameObject);
                break;

            case "Kabe":
                Destroy(gameObject);
                break;
        }
    }
}
