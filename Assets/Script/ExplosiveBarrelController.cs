using UnityEngine;

public class ExplosiveBarrelController : MonoBehaviour
{
    private Player3D _player;
    
    public GameObject explosionEffect;

    public float radius = 5f;
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        AreaOfEffect();
        Destroy(gameObject);
    }

    private void AreaOfEffect()
    {
        var player = GameObject.Find("Player").GetComponent<Player3D>();
        
        var colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(var nearbyObject in colliders)
        {
            if(nearbyObject != null)
            {
                if(nearbyObject.CompareTag("Player"))
                {
                    //Debug.Log("Player in Range");
                    Player3D.b_DamageEffect = true;
                    player.SetPlayerDamage(50);
                }
                if(nearbyObject.CompareTag("Enemy"))
                {
                    //Debug.Log("Enemy in Range");
                    Destroy(GameObject.FindWithTag("Enemy"));
                }
            }
        }
    }
}
