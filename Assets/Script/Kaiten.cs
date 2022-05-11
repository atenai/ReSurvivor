using UnityEngine;

public class Kaiten : MonoBehaviour
{
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

        GetComponent<RectTransform>().transform.Rotate(0.0f, 0.0f, -5.0f);

    }
}
