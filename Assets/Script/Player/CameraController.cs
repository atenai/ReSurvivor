using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    float pullBackSpeed = 5.0f;
    [SerializeField] Transform cam;

    void Start()
    {

    }

    void Update()
    {
        if (Goal.isGOAL != true)
        {
            var playerPos = Player.singletonInstance.transform.position;
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

    IEnumerator DoShake(float duration = 0.25f, float magnitude = 0.1f)
    {
        var pos = cam.transform.localPosition;

        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = pos;
    }
}
