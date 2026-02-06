using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
public class PlayerMovement : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Jump Height")] float _jumpHeight = 1f;
    [SerializeField, Label("Player Gravity")] float _gravity = 1f;
    
    //Внутренние переменные
    Vector2 _playerMovingDirection = Vector2.zero;
    float _currentSpeed;

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
        if (Input.GetKeyDown(KeyCode.Space)) Jump(_jumpHeight); //Прыгаю (игрок в воздухе). Хуевая реализация, ибо физика должна находиться в FixedUpdate()
    }

    //Фиксированное обновление игры
    private void FixedUpdate()
    {
        Move(_playerMovingDirection); //Двигаю игрока
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

    //Подкидываю в воздух
    public void Jump(float strength)
    {
        var jumpHeightVector = new Vector3(0, strength, 0);
        _rigidbody.AddForce(jumpHeightVector, ForceMode.VelocityChange);
    }

    //Геттеры и сеттеры
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
}