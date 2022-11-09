using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _goalPlane;
    private float _pullBackSpeed;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _pullBackSpeed = 5.0f;
    }

    private void Update()
    {
        if (!Goal.isGOAL)
        {
            var tmp = _player.transform.position;
            var x = tmp.x + 5.0f;
            transform.position = new Vector3(x, 3.0f, -10.0f);
        }
        else
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, position.z - _pullBackSpeed * Time.deltaTime);
        }

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
