using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStance))]
public class PlayerMovement : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Ground Checker Position")] private Transform _groundCheck;
    [SerializeField, Label("Ground Checker Size")] private float _groundCheckDistance = 1f;

    [Space, SerializeField, Label("Jump Height")] private float _jumpHeight = 1f;

    [Space, SerializeField, Label("Can player move?")] private bool _canMove = true;
    [SerializeField, Label("Can player jump?")] private bool _canJump = true;

    //Внутренние переменные
    private float _currentSpeed = 0f;
    private Vector2 _movingDirection = Vector2.zero;
    private bool _isGrounded = true;

    //Кэшированные переменные
    private Rigidbody _rigidbody;
    private PlayerStance _playerStance;

    //Методы Моно
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerStance = GetComponent<PlayerStance>();

        //_currentSpeed = _playerStance.CurrentStanceSpeed;
        _currentSpeed = 3f;
    }

    private void Update()
    {
        _isGrounded = IsGrounded;
        _movingDirection = MovingDirection;
        if (Input.GetKey(KeyCode.Space) && _canJump) Jump(_jumpHeight);
    }

    private void FixedUpdate()
    {
        if (_canMove) Move(_movingDirection);
    }

    //Методы скрипта
    public void Move(Vector2 direction)
    {
        var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
        var nextPosition = transform.localPosition + tripleAxisDirection * _currentSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);
    }

    public void Jump(float strength)
    {
        var velocityChange = new Vector3(_rigidbody.velocity.x, strength, _rigidbody.velocity.z);
        if (_isGrounded) _rigidbody.velocity = velocityChange;
    }

    //Геттеры и сеттеры
    public Vector2 MovingDirection
    {
        get
        {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            var normalizedInputAxis = new Vector2(horizontalInput, verticalInput).normalized;

            var moveDirection = transform.forward * normalizedInputAxis.y + transform.right * normalizedInputAxis.x;
            var directionResult = new Vector2(moveDirection.x, moveDirection.z);
            return directionResult;
        }
    }

    public bool IsGrounded => Physics.Raycast(_groundCheck.position, -transform.up, _groundCheckDistance);

    public float JumpHeight
    {
        get => _jumpHeight;
        set => _jumpHeight = value;
    }

    public float CurrentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = value;
    }

    public bool CanJump
    {
        get => _canJump;
        set => _canJump = value;
    }

    public bool CanMove
    {
        get => _canJump;
        set => _canJump = value;
    }
}