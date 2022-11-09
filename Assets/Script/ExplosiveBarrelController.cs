using UnityEngine;

public class ExplosiveBarrelController : MonoBehaviour
{
    private Player3D player;
    
    public GameObject explosionEffect;

    public float radius = 5f;

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
                    player.isImageDamage = true;
                    player.SetPlayerDamage(50);
                }
                if(nearbyObject.CompareTag("Enemy"))
                {
                    Destroy(GameObject.FindWithTag("Enemy"));
                }
            }
        }
    }
}
