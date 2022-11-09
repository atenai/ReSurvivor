using UnityEngine;

public class UIRotate : MonoBehaviour
{   
    void LateUpdate()
    {
        this.GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);
    }
}
