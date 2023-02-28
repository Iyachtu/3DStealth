using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStateMode { IDLE, MOVING, RUNNING, JUMPING, FALLING, SNEAKING}
public class PlayerMovement : MonoBehaviour
{
    private PlayerStateMode _currentState;
    [SerializeField] private float _moveSpeed, _runSpeed, _sneakSpeed, _currentSpeed, _jumpForce;
    private Animator _playerAnimator;
    private Rigidbody _rb;
    private Vector2 _pMovement;
    [SerializeField] private InputActionReference _moveInput, _jumpInput, _runInput, _sneakInput;
    private bool _isgrounded, _isjumping;

    [SerializeField] private float _rotationSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _raycastLength;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private GameObject _kneeCast, _feetCast;
    [SerializeField] float _stepHeight = 0.45f;
    [SerializeField] float _stepSmooth = 0.5f;

    [SerializeField] GameObject[] _raycastArray;

    private void Awake()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        _isgrounded = true;
        _kneeCast.transform.position = new Vector3 (_feetCast.transform.position.x, _feetCast.transform.position.y+_stepHeight,_feetCast.transform.position.z);
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
        CheckGround();
        OnStateUpdate();
        Debug.Log(_currentState);
    }

    private void FixedUpdate()
    {
        if (_pMovement.magnitude >0.1f)
        {
            Move();
            //Climb();
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    private void GetInput()
    {
        _pMovement = _moveInput.action.ReadValue<Vector2>();
        //Debug.Log(_pMovement.magnitude);
    }

    private void Move()
    {
        float test = _rb.velocity.y;
        float targetAngle = Mathf.Atan2(_pMovement.x, _pMovement.y) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        //if (_isgrounded==false) moveDir = Quaternion.Euler(0, targetAngle, 0) * new Vector3(0, _rb.velocity.y, 1);
        //Debug.Log(moveDir);
        _rb.velocity = moveDir.normalized * _currentSpeed;
        _rb.velocity = new Vector3(_rb.velocity.x, test, _rb.velocity.z);
    }

    private void Climb()
    {
        RaycastHit hitFeet;
        if (Physics.Raycast(_feetCast.transform.position, transform.TransformDirection(Vector3.forward), out hitFeet,0.35f)) 
        {
            RaycastHit hitKnee;
            if (!Physics.Raycast(_kneeCast.transform.position, transform.TransformDirection(Vector3.forward), out hitKnee, 0.45f))
            {
                _rb.position -= new Vector3(0, -_stepSmooth, 0);
            }
        }

        RaycastHit hitFeetplus45;
        if (Physics.Raycast(_feetCast.transform.position, transform.TransformDirection(1.5f,0,1), out hitFeetplus45, 0.1f))
        {
            RaycastHit hitKneeplus45;
            if (!Physics.Raycast(_kneeCast.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitKneeplus45, 0.2f))
            {
                _rb.position -= new Vector3(0, -_stepSmooth, 0);
            }
        }

        RaycastHit hitFeetmin45;
        if (Physics.Raycast(_feetCast.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitFeetmin45, 0.1f))
        {
            RaycastHit hitKneemin45;
            if (!Physics.Raycast(_kneeCast.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitKneemin45, 0.2f))
            {
                _rb.position -= new Vector3(0, -_stepSmooth, 0);
            }
        }


    }

    private void Jump()
    {
        _isjumping= true;
        _rb.AddForce(transform.up * _jumpForce,ForceMode.Impulse);
        Debug.Log("jump");
    }

    private void CheckGround()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit,_raycastLength);
        if (hit.collider != null && hit.collider.CompareTag("Ground")) { _isgrounded = true; _isjumping = false; }
        else if (hit.collider == null) _isgrounded = false;
        Debug.Log(_isgrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3 (0,1,0);
        Gizmos.DrawRay(transform.position, direction);
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
                _playerAnimator.SetBool("isJumping", true);
                Jump();
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
                _playerAnimator.SetBool("isJumping", false);
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
                if (_isgrounded == false) TransitionToState(PlayerStateMode.FALLING);
                else if (_pMovement.magnitude > 0.1f) TransitionToState(PlayerStateMode.MOVING);
                else if (_jumpInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.JUMPING);
                break;
            case PlayerStateMode.RUNNING:
                _currentSpeed = _runSpeed;
                if (_isgrounded==false) TransitionToState(PlayerStateMode.FALLING);
                else if (_runInput.action.ReadValue<float>() <0.5f) TransitionToState(PlayerStateMode.MOVING);
                else if (_jumpInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.JUMPING);
                break;
            case PlayerStateMode.FALLING:
                if (_isgrounded)
                {
                    if (_pMovement.magnitude > 0.1f) TransitionToState(PlayerStateMode.MOVING);
                    else TransitionToState(PlayerStateMode.IDLE);
                }
                break;
            case PlayerStateMode.MOVING:
                _currentSpeed = _moveSpeed;
                if (_isgrounded == false) TransitionToState(PlayerStateMode.FALLING);
                else if (_sneakInput.action.ReadValue<float>()>=0.5f) TransitionToState(PlayerStateMode.SNEAKING);
                else if (_jumpInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.JUMPING);
                else if (_runInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.RUNNING);
                break;
            case PlayerStateMode.SNEAKING:
                _currentSpeed = _sneakSpeed;
                if (_isgrounded ==false) TransitionToState(PlayerStateMode.FALLING);
                else if (_sneakInput.action.ReadValue<float>() < 0.5f) TransitionToState(PlayerStateMode.MOVING);
                break;
            case PlayerStateMode.JUMPING:
                if (_isgrounded == false && _isjumping == false) TransitionToState(PlayerStateMode.FALLING);
                else if (_isgrounded)
                {
                    if (_pMovement.magnitude <= 0.1f) TransitionToState(PlayerStateMode.IDLE);
                    else if (_pMovement.magnitude > 0.1f) TransitionToState(PlayerStateMode.MOVING);
                }
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

    private void OnCollisionStay(Collision collision)
    {
        //if(collision.collider.CompareTag("Ground")) _isgrounded= true;
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.collider.CompareTag("Ground")) _isgrounded= false;
    }

}
