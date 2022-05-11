using UnityEngine;

public class PlayerRayCast : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        var rayPositionX = gameObject.transform.position.x;
        var rayPositionY = gameObject.transform.position.y;
        var rayPositionZ = gameObject.transform.position.z;
        var v3RayPosition = new Vector3(rayPositionX, rayPositionY + 1.4f, rayPositionZ);

        var ray = new Ray(v3RayPosition, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200.0f))
        {//何かがレイに触れた場合 
            if (hit.collider.tag == "Enemy")
            {//敵を見つけた際
                Debug.Log("ソルジャーをレイで見つけた");
                // 衝突したオブジェクトを消す
                //Destroy(hit.collider.gameObject);

            }
            else if (hit.collider.tag == "Boss")
            {//敵を見つけた際
                Debug.Log("ボスをレイで見つけた");
                // 衝突したオブジェクトを消す
                //Destroy(hit.collider.gameObject);

            }
            else if (hit.collider.tag == "technical")
            {//敵を見つけた際
                Debug.Log("テクニカルをレイで見つけた");
                // 衝突したオブジェクトを消す
                //Destroy(hit.collider.gameObject);

            }
        }
        Debug.DrawRay(ray.origin, ray.direction * 200, Color.red, 1);

    }
}
