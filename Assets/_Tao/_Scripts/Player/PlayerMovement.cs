using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Label("Jump Height")] float jumpHeight = 1f;
    [SerializeField, Label("Player Gravity")] float gravity = 1f;

    float _currentSpeed;
    Rigidbody _rigidbody;
    ConstantForce _constantForce;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _constantForce = GetComponent<ConstantForce>();

        CurrentSpeed = 3.5f;
    }

    private void FixedUpdate()
    {
        var playerMovingDirection = PlayerMovingDirection();
        Move(playerMovingDirection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(JumpHeight);
        }
    }
        
    public Vector2 PlayerMovingDirection()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");
        var normalizedInputAxis = new Vector2(horizontalInput, verticalInput).normalized;

        var moveDirection = transform.forward * normalizedInputAxis.y + transform.right * normalizedInputAxis.x;
        var directionResult = new Vector2(moveDirection.x, moveDirection.z);
        return directionResult;
    }

    public void Move(Vector2 direction)
    {
        var tripleAxisDirection = new Vector3(direction.x, 0, direction.y);
        var nextPosition = transform.localPosition + tripleAxisDirection * _currentSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(nextPosition);
    }

    public void Jump(float strength)
    {
        var jumpHeightVector = new Vector3(0, strength, 0) * Time.fixedDeltaTime;
        _rigidbody.AddForce(jumpHeightVector, ForceMode.VelocityChange);
    }

    public float JumpHeight
    {
        get { return jumpHeight; }
        set { jumpHeight = value; }
    }

    public float Gravity
    {
        get { return gravity; }
        set 
        { 
            gravity = value;
            _constantForce.relativeForce = new Vector3(0, -gravity, 0);
        }
    }

    public float CurrentSpeed
    {
        get { return _currentSpeed; }
        set { _currentSpeed = value; }
    }
}