using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStateMode { IDLE, MOVING, RUNNING, JUMPING, FALLING, SNEAKING}
public class PlayerMovement : MonoBehaviour
{
    private PlayerStateMode _currentState;
    [SerializeField] private float _moveSpeed, _runSpeed, _sneakSpeed, _currentSpeed;
    private Animator _playerAnimator;
    private Rigidbody _rb;
    private Vector2 _pMovement;
    [SerializeField] private InputActionReference _moveInput, _jumpInput, _runInput, _sneakInput;

    [SerializeField] private float _rotationSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Transform _camera;

    private void Awake()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(PlayerStateMode.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (_pMovement.magnitude >0.1f)
        {
            Move();
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    private void GetInput()
    {
        _pMovement = _moveInput.action.ReadValue<Vector2>();
    }

    private void Move()
    {
        float targetAngle = Mathf.Atan2(_pMovement.x, _pMovement.y) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        _rb.velocity = moveDir.normalized * _currentSpeed;
    }

    private void Jump()
    {

    }

    private void OnStateEnter()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;
            case PlayerStateMode.RUNNING:
                _playerAnimator.SetBool("isRunning", true);
                break;
            case PlayerStateMode.FALLING:
                _playerAnimator.SetBool("isFalling", true);
                break;
            case PlayerStateMode.MOVING:
                break;
            case PlayerStateMode.SNEAKING:
                _playerAnimator.SetBool("isSneaking", true);
                break;
            case PlayerStateMode.JUMPING:
                _playerAnimator.SetBool("isSneaking", true);
                break;
            default:
                break;
        }
    }

    private void OnStateExit()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;
            case PlayerStateMode.RUNNING:
                _playerAnimator.SetBool("isRunning", false);
                break;
            case PlayerStateMode.FALLING:
                _playerAnimator.SetBool("isFalling", false);
                break;
            case PlayerStateMode.MOVING:
                break;
            case PlayerStateMode.SNEAKING:
                _playerAnimator.SetBool("isSneaking", false);
                break;
            case PlayerStateMode.JUMPING:
                _playerAnimator.SetBool("isSneaking", false);
                break;
            default:
                break;
        }
    }

    private void OnStateUpdate()
    {
        switch (_currentState)
        {
            case PlayerStateMode.IDLE:
                break;
            case PlayerStateMode.RUNNING:
                _currentSpeed = _runSpeed;
                break;
            case PlayerStateMode.FALLING:
                break;
            case PlayerStateMode.MOVING:
                _currentSpeed = _moveSpeed;
                break;
            case PlayerStateMode.SNEAKING:
                _currentSpeed = _sneakSpeed;
                break;
            case PlayerStateMode.JUMPING:
                break;
            default:
                break;
        }
    }

    private void TransitionToState(PlayerStateMode toState)
    {
        OnStateExit();
        _currentState = toState;
        OnStateEnter();
    }

}
