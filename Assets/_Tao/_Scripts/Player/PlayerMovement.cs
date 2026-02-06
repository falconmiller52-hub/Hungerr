using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
public class PlayerMovement : MonoBehaviour
{
    //Переменные инспектора
    [Space, SerializeField, Label("Jump Height")] float _jumpHeight = 1f;
    [SerializeField, Label("Player Gravity")] float _gravity = 1f;

    [Space, SerializeField, Label("Can player move?")] bool _canMove = true;
    [SerializeField, Label("Can player jump?")] bool _canJump = true;

    //Внутренние переменные
    Vector2 _playerMovingDirection = Vector2.zero;
    float _currentSpeed;
    bool _isGrounded = false;

    //Кэшированные переменные
    Rigidbody _rigidbody;
    ConstantForce _constantForce;

    //Кэширование и задание переменным значения при запуске
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _constantForce = GetComponent<ConstantForce>();

        _constantForce.relativeForce = new Vector3(0, -_gravity, 0);
        _currentSpeed = 3.5f;
    }

    //Постоянное обновление игры
    private void Update()
    {
        _playerMovingDirection = PlayerMovingDirection; //Присваиваю направление ходьбы переменной
        if (Input.GetKeyDown(KeyCode.Space) && _canJump) Jump(_jumpHeight); //Прыгаю (игрок в воздухе). Хуевая реализация, ибо физика должна находиться в FixedUpdate()
    }

    //Фиксированное обновление игры
    private void FixedUpdate()
    {
        if (_canMove) Move(_playerMovingDirection); //Двигаю игрока
    }

    //Триггеры для проверки пола
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Floor")) _isGrounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Floor")) _isGrounded = false;
    }

    //no fuck (получаю направление движения игрока с инпутами)
    public Vector2 PlayerMovingDirection {
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

    //Двигаю игрока
    public void Move(Vector2 direction)
    {
        var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
        var nextPosition = transform.localPosition + tripleAxisDirection * _currentSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);
    }

    //Подкидываю в воздух и даю кулдаун прыжку
    public void Jump(float strength)
    {
        var jumpHeightVector = new Vector3(0, strength, 0);

        if (_isGrounded) _rigidbody.AddForce(jumpHeightVector, ForceMode.Impulse);
    }

    //Геттеры и сеттеры
    public bool IsGrounded => _isGrounded;

    public float JumpHeight
    {
        get => _jumpHeight;
        set => _jumpHeight = value;
    }

    public float Gravity
    {
        get => _gravity;
        set 
        {
            _gravity = value;
            _constantForce.relativeForce = new Vector3(0, -_gravity, 0);
        }
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