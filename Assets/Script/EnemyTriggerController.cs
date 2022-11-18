using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerController : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyGameObjects;

    bool isOneHit = false;

    void Awake()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        SetActiveAll(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isOneHit == false)
        {
            isOneHit = true;
            SetActiveAll(true);
        }
    }

    void SetActiveAll(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }

        foreach (var enemyGameObject in enemyGameObjects)
        {
            enemyGameObject.SetActive(value);
        }
    }
}
