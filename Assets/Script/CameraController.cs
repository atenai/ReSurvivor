using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _goalPlane;
    private float _pullBackSpeed;
    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _pullBackSpeed = 5.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Goal.b_GOAL)
        {
            var tmp = _player.transform.position;
            //if (Player3D.b_rot == true)
            //{
            var x = tmp.x + 5.0f;

            /*
            if (StageSceneController.isBossAlive)
            {
                x = Mathf.Min(x, 205);
            }
            */
            transform.position = new Vector3(x, 3.0f, -10.0f);
            //}
            //else if(Player3D.b_rot == false)
            //{
            //    this.transform.position = new Vector3(tmp.x - 5.0f, 3.0f, -10.0f);
            //}
        }
        else
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y, position.z - _pullBackSpeed * Time.deltaTime);
        }

    }
}
