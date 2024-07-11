using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class VanController : MonoBehaviour
{
    //  ジャンプのパワーを設定できるようにする
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _forwordSpeed;

    private PlayerInput _playerInput;   //  input system
    private Rigidbody _rb;              //  物理演算用

    private Vector3 _moveDirection;     //  移動量
    private bool _isJumpNow;            //  ジャンプ中ならば true

    // Start is called before the first frame update
    void Start()
    {
        //  同じオブジェクトにあるはずのコンポーネントを取得する
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput == null || _rb == null || _isJumpNow) return;
        var moveDir = _playerInput.actions["Move"].ReadValue<Vector2>();
        _moveDirection = transform.TransformDirection(new Vector3(0, 0, moveDir.y * _forwordSpeed));
        transform.Rotate(0, moveDir.x, 0);
        
    }

    private void FixedUpdate()
    {
        if(_playerInput == null || _rb == null || _isJumpNow) return;
        _moveDirection.y = _rb.velocity.y;
        _rb.velocity = _moveDirection;
        //  ジャンプボタンを押していたらジャンプする
        if (_playerInput.actions["Jump"].triggered)
        {
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isJumpNow = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //  ジャンプ中
        if (_isJumpNow)
        {
            //  地面と接触していたら
            if(collision.gameObject.CompareTag("Ground"))
            {
                _isJumpNow = false;
            }
        }
    }
}
