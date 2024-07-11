using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;                   //  入力（Input System）
    private CharacterController _characterController;   //  character移動
    private Animator _animator;                         //  アニメーション

    Vector3 _moveDirection = Vector3.zero;              //  移動方向

    #region インスペクターで操作できる変数
    [SerializeField] private float _gravity;            //  重力
    [SerializeField] private float _speedZ;             //  前方への移動速度
    [SerializeField] private float _speedJump;          //  ジャンプパワー
    [SerializeField] private float _rotateSpeed;        //  回転速度
    [SerializeField] private float _runRotateSpeed;     //  走っているときの回転速度
    #endregion

    //  アニメーションに速度を伝えるためのハッシュコード
    private static readonly int Speed = Animator.StringToHash("speed");
    //  コントローラーの微妙なブレをカットするための数値
    private float _runLimitSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        //  characterコントローラーの取得
        _characterController = GetComponent<CharacterController>();
        //  アニメーターの取得
        _animator = GetComponent<Animator>();
        //  Input System の取得
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        float frontSpeed = 0;
        //  接地していれば移動の処理する
        if (_characterController.isGrounded)
        {
            //  コントローラーから移動の入力を取得する
            var moveValue = _playerInput.actions["Move"].ReadValue<Vector2>();
            frontSpeed = moveValue.y;                       //  アニメーション用に保存
            //  前方の移動入力が特定の値より大きければ移動と判断する
            if (moveValue.y > _runLimitSpeed)
                _moveDirection.z = moveValue.y * _speedZ;   //  移動方向にスピードを掛ける
            else
                _moveDirection.z = 0;                       //  移動しないので０にする
            //  回転を取得（移動中とその場の回転速度を変える）
            var rotateSpeed = moveValue.x * ((moveValue.y > _runLimitSpeed) ? _runRotateSpeed : _rotateSpeed);
            //  characterの回転
            transform.Rotate(0, rotateSpeed, 0);
            //  ジャンプボタンが押されていたら
            if (_playerInput.actions["Jump"].triggered)
            {
                _moveDirection.y = _speedJump;              //  上方向に速度を設定する
            }
        }
        //  重力加速度を加算する
        _moveDirection.y -= _gravity * Time.deltaTime;
        //  ローカルの移動方向をキャラの向いている方向へ移動させる処理
        Vector3 globalDirection = transform.TransformDirection(_moveDirection);
        _characterController.Move(globalDirection * Time.deltaTime);
        //  接地判定をして地面にたどり着いていれば下への移動速度は０に設定する
        if (_characterController.isGrounded) _moveDirection.y = 0;
        Debug.Log($"Speed = {frontSpeed}");
        //  走りのアニメーションリクエスト
        _animator.SetFloat(Speed, frontSpeed);
    }
}
