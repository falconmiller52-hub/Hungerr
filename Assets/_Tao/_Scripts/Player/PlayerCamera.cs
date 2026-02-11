using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStance))]
public class PlayerCamera : MonoBehaviour
{
    //Переменные инспектора
    [SerializeField, Label("Player Camera Objects Assign")] private List<GameObject> _cameraObjects;

    [Space, SerializeField, Label("Camera Sensitivity of X and Y axis")] private Vector2 _xySensitivity = Vector2.one;
    [SerializeField, Label("Minimum and Maximum Angle of Y axis"), MinMaxSlider(-90f, 90f)] private Vector2 _minMaxYAngle = Vector2.zero;

    [Space, SerializeField, Label("Breathing Magnitude")] private float _breathingMagnitude = 0f;
    [SerializeField, Label("Breathing Speed")] private float _breathingSpeed = 0f;

    [Space, SerializeField, Label("Stepping Magnitude")] private float _steppingMagnitude = 0f;
    [SerializeField, Label("Stepping Speed")] private float _steppingSpeed = 0f;

    [Space, SerializeField, Label("Can player rotate his camera?")] private bool _canRotate = true;

    //Внутренние переменные
    private float _xRotation, _yRotation;

    //Кэшированные переменные
    private PlayerMovement _playerMovement;
    private PlayerStance _playerStance;

    //Методы Моно
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerStance = GetComponent<PlayerStance>();

        _xRotation = _cameraObjects[3].transform.localEulerAngles.x;
        _yRotation = transform.localEulerAngles.y;

        MouseToggle("-");
    }

    private void Update()
    {
        if (_canRotate) LookAt(CameraRotation);

        BreatheMove();
        StepMove();
    }

    //Методы скрипта

    private void BreatheMove()
    {
        var nextBreathePosition = Vector3.up * Mathf.Sin(Time.time * _breathingSpeed) * _breathingMagnitude;

        _cameraObjects[1].transform.localPosition = nextBreathePosition;
    }

    private void StepMove()
    {
        var playerCurrentStance = _playerStance.CurrentStance;
        var playerCurrentSpeed = _playerStance.CurrentStanceSpeed(playerCurrentStance);

        var playerMovingDirectionMagnitude = _playerMovement.MovingDirection.magnitude;

        var nextStepPosition = Vector3.up * Mathf.Sin(Time.time * _steppingSpeed * playerCurrentSpeed) * _steppingMagnitude * playerMovingDirectionMagnitude;

        _cameraObjects[2].transform.localPosition = nextStepPosition;
    }

    public void LookAt(Vector2 direction)
    {
        transform.localEulerAngles = new Vector3(0, direction.y, 0);

        var angleChange = direction.x;

        if (angleChange > 180f) angleChange -= 360f;
        angleChange = Mathf.Clamp(angleChange, _minMaxYAngle.x, _minMaxYAngle.y);

        _cameraObjects[3].transform.localEulerAngles = new Vector3(angleChange, 0, 0);
    }

    public void MouseToggle(string state = "~")
    {
        bool visible = Cursor.visible;
        CursorLockMode lockState = Cursor.lockState;

        if (state == "~")
        {
            visible = !visible;
            lockState = lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
        else if (state == "-")
        {
            visible = false;
            lockState = CursorLockMode.Locked;
        }
        else
        {
            visible = true;
            lockState = CursorLockMode.None;
        }

        Cursor.visible = visible; Cursor.lockState = lockState;
    }

   //Геттеры и сеттеры
   public Vector2 MouseAxis
    {
        get
        {
            var xAxis = Input.GetAxis("Mouse X");
            var yAxis = -Input.GetAxis("Mouse Y");

            return new Vector2(xAxis, yAxis);
        }
    }

    public Vector2 CameraRotation
    {
        get
        {
            var mouseAxis = MouseAxis;

            var yAxis = mouseAxis.x;
            var xAxis = mouseAxis.y;

            _yRotation += yAxis * _xySensitivity.x * Time.deltaTime;

            _xRotation += xAxis * _xySensitivity.y * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, _minMaxYAngle.x, _minMaxYAngle.y);

            return new Vector2(_xRotation, _yRotation);
        }
    }

    public List<GameObject> CameraObjects
    {
        get => _cameraObjects;
        set => _cameraObjects = value;
    }

    public Vector2 Sensitivity
    {
        get => _xySensitivity;
        set => _xySensitivity = value;
    }

    public Vector2 MinMaxYAngle
    {
        get => _minMaxYAngle;
        set => _minMaxYAngle = value;
    }

    public float BreathingMagnitude
    {
        get => _breathingMagnitude;
        set => _breathingMagnitude = value;
    }

    public float BreathingSpeed
    {
        get => _breathingSpeed;
        set => _breathingSpeed = value;
    }

    public float SteppingMagnitude
    {
        get => _steppingMagnitude;
        set => _steppingMagnitude = value;
    }

    public float SteppingSpeed
    {
        get => _steppingSpeed;
        set => _steppingSpeed = value;
    }

    public bool CanRotate
    {
        get => _canRotate;
        set => _canRotate = value;
    }
}