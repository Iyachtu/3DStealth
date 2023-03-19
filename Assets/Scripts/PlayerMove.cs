using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStateMode { IDLE, MOVING, RUNNING, JUMPING, FALLING, SNEAKING}
public class PlayerMove : MonoBehaviour
{
    [SerializeField] BoolVariables _isHidden;

    private PlayerStateMode _currentState;
    [SerializeField] private float _moveSpeed, _runSpeed, _sneakSpeed, _currentSpeed, _jumpForce;
    [SerializeField] private Animator _playerAnimator;
    private Rigidbody _rb;
    private Vector2 _pMovement;
    [SerializeField] public InputActionReference _moveInput, _jumpInput, _runInput, _sneakInput, _actionInput;
    private bool _isgrounded, _isJumping;

    [SerializeField] private float _rotationSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _raycastLength;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private GameObject _kneeCast, _feetCast;
    [SerializeField] float _stepHeight = 0.45f;
    [SerializeField] float _stepSmooth = 0.5f;

    [SerializeField] Transform _groundChecker;
    [SerializeField] Vector3 _boxDimension;

    private RaycastHit _slopeHit;
    private bool _onSlope;
    private Vector3 _slopeMoveDir;
    [SerializeField] float _slopeMaxAngle;

    private void Awake()
    {
        _isHidden.value = false;
        //_playerAnimator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
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
        if (!_isJumping) CheckGround();
        OnStateUpdate();
        _slopeMoveDir = Vector3.ProjectOnPlane(_pMovement, _slopeHit.normal);
    }

    private void FixedUpdate()
    {
        if (_pMovement.magnitude >0.1f)
        {
            _onSlope = OnSlope();
            if (_onSlope)
            {
                float angle = AngleCalc();
                SlopeMove(angle);
                //Move();
            }
            else if (!_onSlope)
            {
                Move();
                Climb();
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.25f))
        {
            if (_slopeHit.normal != Vector3.up) return true;
            else return false;
        }

        return false;
    }

    private float AngleCalc()
    {
        float angle=0;
        //angle = Mathf.Abs( Vector3.Angle(_slopeHit.normal, Vector3.forward) - 90);
        angle = Mathf.Abs( Vector3.Angle(_slopeHit.normal, Vector3.up));
        return angle;
    }

    private void GetInput()
    {
        _pMovement = _moveInput.action.ReadValue<Vector2>();
        _playerAnimator.SetFloat("DirX", _pMovement.x);
        _playerAnimator.SetFloat("DirY", _pMovement.y);
        //Debug.Log(_pMovement.magnitude);
    }

    private void Move()
    {
        float test = _rb.velocity.y;
        float targetAngle = Mathf.Atan2(_pMovement.x, _pMovement.y) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        _rb.velocity = moveDir.normalized * _currentSpeed;
        _rb.velocity = new Vector3(_rb.velocity.x, test, _rb.velocity.z);
    }

    private void SlopeMove(float angle)
    {
        if (Mathf.Sign(_pMovement.y) == Mathf.Sign(_slopeHit.normal.z))
        {
            if (angle <= _slopeMaxAngle)
            {

            }
            else
            {
            }
                Move();
        }
        else
        {
            if (angle <= _slopeMaxAngle)
            {

            }
            else
            {
            }
            Move();
        }
        
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
        _isJumping = true;
        _rb.AddForce(transform.up * _jumpForce,ForceMode.Impulse);
        StartCoroutine(JumpTimer());
    }

    private void CheckGround()
    {
        Collider[] groundcolliders = Physics.OverlapBox(_groundChecker.position, _boxDimension/2, Quaternion.identity, _groundMask);

        _isgrounded = groundcolliders.Length > 0;

        if (_isgrounded)
        {
            _playerAnimator.SetBool("isGrounded", true);
            Debug.Log("touche le sol");
        }
        else _playerAnimator.SetBool("isGrounded", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3 (0,-0.5f,0);
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawWireCube(_groundChecker.position, _boxDimension);
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
                _playerAnimator.SetBool("isMoving", true);
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
                _playerAnimator.SetBool("isMoving", false);
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
                _rb.velocity = Vector3.zero;
                if (_isgrounded == false) TransitionToState(PlayerStateMode.FALLING);
                else if (_pMovement.magnitude > 0.1f) TransitionToState(PlayerStateMode.MOVING);
                else if (_jumpInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.JUMPING);
                break;
            case PlayerStateMode.RUNNING:
                _currentSpeed = _runSpeed;
                if (_isgrounded==false) TransitionToState(PlayerStateMode.FALLING);
                else if (_runInput.action.ReadValue<float>() <0.5f || _pMovement.magnitude<0.1f) TransitionToState(PlayerStateMode.MOVING);
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
                else if (_pMovement.magnitude < 0.1f) TransitionToState(PlayerStateMode.IDLE);
                else if (_sneakInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.SNEAKING);
                else if (_jumpInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.JUMPING);
                else if (_runInput.action.ReadValue<float>() >= 0.5f) TransitionToState(PlayerStateMode.RUNNING);
                break;
            case PlayerStateMode.SNEAKING:
                _currentSpeed = _sneakSpeed;
                if (_isgrounded ==false) TransitionToState(PlayerStateMode.FALLING);
                else if (_sneakInput.action.ReadValue<float>() < 0.5f || _pMovement.magnitude <0.1f) TransitionToState(PlayerStateMode.MOVING);
                break;
            case PlayerStateMode.JUMPING:
                if (_isgrounded)
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
    }

    private void OnCollisionExit(Collision collision)
    {
    }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(0.01f);
        _isJumping = false;
    }
}
