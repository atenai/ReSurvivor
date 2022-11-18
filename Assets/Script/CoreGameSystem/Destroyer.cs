using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float duration = 5.0f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
