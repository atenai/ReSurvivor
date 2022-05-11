using System.Collections;
using UnityEngine;

public class Tank : MonoBehaviour {
    public GameObject tankShellPrefab;

    private int _hp;
    public int Hp {
        get {
            return _hp;
        }
        protected set {
            _hp = value;
            if (Hp <= 0) {
                Destroy(gameObject);
            }
        }
    }

    private float _roundInterval;
    private float _thrust;
    private Vector3 _relativePosition;
    private GameObject _player;
    private float _activeRange, _withdrawalRange;
    private float _maxSpeed;
    private Rigidbody2D _rigid2D;
    private float _velocityX, _moveForce, _turnThreshold;
    private Vector3 _distanceX, _direction, _localScale;

    // Start is called before the first frame update
    private void Start() {
        //耐久値
        Hp = 7;
        //射撃間隔
        _roundInterval = 2.5f;
        //銃弾の推力（速度を決める）
        _thrust = 350.0f;
        //発射口の相対位置
        _relativePosition = new Vector3(-1.5f, 0.0f, 0.0f);
        //離脱範囲
        _withdrawalRange = 7.0f;
        //移動速度
        _maxSpeed = 1.0f;
        //推力（加速度を決める）
        _moveForce = 10.0f;
        _turnThreshold = 0.1f;

        _ = StartCoroutine(FireTimer());

        _player = GameObject.FindGameObjectWithTag("Player");

        _rigid2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        if (_player != null) {
            //プレイヤーへの水平距離ベクター
            _distanceX = Vector3.Project(_player.transform.position - transform.position, Vector3.right);
            //プレイヤーへの方向ベクター
            _direction = _distanceX / _distanceX.magnitude;
            //水平速度
            _velocityX = _rigid2D.velocity.x;
            //localScale（向き調整用）
            _localScale = transform.localScale;

            //プレイヤーの方へ向く
            if (_direction.x * _localScale.x > 0) {
                transform.localScale = new Vector3(-_localScale.x, _localScale.y, _localScale.z);
            }

            //距離が近い
            if (_distanceX.magnitude > _withdrawalRange + _turnThreshold) {
                //最大速度に達していない
                if (_velocityX * _direction.x < _maxSpeed) {
                    //プレイヤーの方へ推力をかける
                    _rigid2D.AddForce(_direction * _moveForce);
                }
            }
            //距離が遠い
            else if (_distanceX.magnitude < _withdrawalRange - _turnThreshold) {
                if (_velocityX * -_direction.x < _maxSpeed) {
                    _rigid2D.AddForce(-_direction * _moveForce);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //プレイヤーの銃弾に当たる
        if (collision.gameObject.CompareTag("Bullet")) {
            Hp -= 1;
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator FireTimer() {
        while (true) {
            //銃弾生成
            var shell = Instantiate(tankShellPrefab);
            //プレイヤーへ飛ばす
            shell.GetComponent<Rigidbody2D>().AddForce(_direction * _thrust);
            shell.transform.position = transform.position + new Vector3(
                _relativePosition.x * -_direction.x, _relativePosition.y, _relativePosition.z);
            yield return new WaitForSeconds(_roundInterval);
        }
    }
}
