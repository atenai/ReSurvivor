using UnityEngine;

public class GoalPlaneController : MonoBehaviour
{
    float VelocityX => rigid.velocity.x;

    float maxSpeed = 30.0f;
    Rigidbody rigid;
    float moveForce = 20000.0f;
    float liftCoefficient = 800.0f;
    Rotator propellerRotator;

    void Awake()
    {
        rigid = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        propellerRotator = transform.Find("propeller").GetComponent<Rotator>();
    }

    void Update()
    {
        if (Goal.isGOAL)
        {
            TakeOff();
        }
    }

    void FixedUpdate()
    {
        if (Goal.isGOAL)
        {
            propellerRotator.Run();
            TakeOff();
        }
    }

    void TakeOff()
    {
        if (VelocityX < maxSpeed)
        {
            rigid.AddForce(moveForce * Vector3.right);
        }

        rigid.AddForce(VelocityX * liftCoefficient * Vector3.up);
    }
}
