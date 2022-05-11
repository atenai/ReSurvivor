using UnityEngine;

public class GoalPlaneController : MonoBehaviour
{
    private float VelocityX => _rigid.velocity.x;

    private float _maxSpeed;
    private Rigidbody _rigid;
    private float _moveForce;
    private float _liftCoefficient;
    private Rotator _propellerRotator;

    private void Awake()
    {
        _maxSpeed = 30.0f;
        _rigid = GetComponent<Rigidbody>();
        _moveForce = 20000.0f;
        _liftCoefficient = 800.0f;
    }

    private void Start()
    {
        _propellerRotator = transform.Find("propeller").GetComponent<Rotator>();
    }

    private void Update()
    {
        if (Goal.b_GOAL)
        {
            TakeOff();
        }
    }

    private void FixedUpdate()
    {
        if (Goal.b_GOAL)
        {
            _propellerRotator.Run();
            TakeOff();
        }
    }

    private void TakeOff()
    {
        if (VelocityX < _maxSpeed)
        {
            _rigid.AddForce(_moveForce * Vector3.right);
        }

        _rigid.AddForce(VelocityX * _liftCoefficient * Vector3.up);
    }
}
