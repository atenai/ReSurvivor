using System;
using System.Collections;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] [Range(-100, 0)] private float patrolRangeLeft;
    [SerializeField] [Range(0, 100)] private float patrolRangeRight;
    [Range(0, 10)] public float moveSpeed;
    [SerializeField] private Transform turningTransform;

    private float _originX;
    private float Lower => _originX + patrolRangeLeft;
    private float Upper => _originX + patrolRangeRight;

    private int _dir = -1;

    private float _moveForce = 100;
    private Rigidbody _rigid;

    private void Awake()
    {
        _originX = transform.position.x;
        _rigid = GetComponent<Rigidbody>();
        _moveForce *= _rigid.mass;
        FaceForward = MakeFaceForward(turningTransform);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public IEnumerator PatrolInRange()
    {
        while (true)
        {
            if (transform.position.x < Lower)
            {
                _dir = 1;
            } else if (transform.position.x > Upper)
            {
                _dir = -1;
            }
            if (Mathf.Abs(_rigid.velocity.x) < moveSpeed)
            {
                _rigid.AddForce(Vector3.right * _dir * _moveForce);
            }
            
            FaceForward();
            yield return new WaitForFixedUpdate();
        }
    }
    
    protected Action FaceForward;

    protected Action MakeFaceForward(Transform t = null)
    {
        if (t == null) return () => {};
        void FaceForward()
        {
            t.eulerAngles = new Vector3(0, _dir * 90, 0);
        }
        return FaceForward;
    }
}
