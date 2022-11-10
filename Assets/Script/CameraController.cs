using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    GameObject player;
    GameObject goalPlane;
    float pullBackSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pullBackSpeed = 5.0f;
    }

    void Update()
    {
        if (!Goal.isGOAL)
        {
            var playerPos = player.transform.position;
            var cameraParentPosX = playerPos.x + 5.0f;
            this.transform.position = new Vector3(cameraParentPosX, 3.0f, -10.0f);
        }
        else
        {
            GoalCameraMove();
        }
    }

    void GoalCameraMove()
    {
        var position = this.transform.position;
        this.transform.position = new Vector3(position.x, position.y, position.z - pullBackSpeed * Time.deltaTime);
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    //スクリプトの修正が必要
    private IEnumerator DoShake(float duration, float magnitude)
    {
        var pos = transform.localPosition;

        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = pos;
    }
}
