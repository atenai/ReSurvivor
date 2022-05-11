using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerController : MonoBehaviour
{
    public List<GameObject> enemyGameObjects;

    private void Awake()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        SetActiveAll(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetActiveAll(true);
        }
    }

    private void SetActiveAll(bool value)
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
