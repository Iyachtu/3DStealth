using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2 states
// regarde en mouvement sur son axe de rotation (avec un paramètre en ° serialized)
// a vu le joueur et le fixe
// si fixé plus de X secondes (serialized), déclanche l'alarm -> perdu

public enum CameraState {Move, Stop, Lock}

public class CamBehavior : MonoBehaviour
{
    [SerializeField] private BoolVariables _isHidden;
    [SerializeField] private float _hiddenRayDistance;
    [SerializeField] private float _startRotationAngle, _endRotationAngle;
    [SerializeField] private float _rotationTime;
    [SerializeField] private float _stopTime;
    [SerializeField] private float _alarmTimer;
    [SerializeField] private float _xRotation;
    private bool _playerLock = false;
    private bool _toMove = false;
    private bool _moveForward = true;
    private float _targetAngle;
    private float t;
    private CameraState _currentState;
    [SerializeField] public bool _isActive;
    private Transform _playerTransform = null;
    private float _lockCounter;
    [SerializeField] private LayerMask _rayLayerMask;


    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(_xRotation, _startRotationAngle,0);
        _currentState = CameraState.Stop;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (_isActive) Behavior();
    }

    private void Behavior()
    {
        //si lock, garder le focus sur le joueur
        if (_currentState== CameraState.Lock) 
        {
            if (_playerTransform!= null)
            {
                if(PlayerRayTest())
                {
                    transform.LookAt(_playerTransform.position);
                    _lockCounter += Time.fixedDeltaTime;
                    if (_lockCounter > _alarmTimer) Debug.Log("Ding Ding c'est l'alarme");
                }
                else _currentState = CameraState.Move;
            }
            else _currentState = CameraState.Move;
        }
        //si state stop, démarrer la coroutine
        else if (_currentState== CameraState.Stop && _toMove==false)
        {
            StartCoroutine(CameraStop());
        }
        //si state move, call camerarotation
        else if (_currentState==CameraState.Move)
        {
            CameraRotation();
        }
        
    }

    private void CameraRotation()
    {
        //test la current rotation vs target
        //si arrivé, faire demi-tour, sinon, faire la rotation
        if ( 1-t <0.0005f)
        {
            _moveForward = !_moveForward;
            _currentState= CameraState.Stop;
            _toMove=false;
            _targetAngle = -_targetAngle;
            t = 0;
        }
        else
        {
            if(_moveForward==false)
            {
                _targetAngle = Mathf.LerpAngle(_endRotationAngle, _startRotationAngle, t);
            }
            else if (_moveForward)
            {
                _targetAngle = Mathf.LerpAngle(_startRotationAngle, _endRotationAngle, t);
            }
            t = t + (Time.deltaTime/_rotationTime);
            transform.eulerAngles = new Vector3(_xRotation,_targetAngle, 0);
        }


    }

    IEnumerator CameraStop()
    {
        _toMove = true;
        yield return new WaitForSeconds(_stopTime);
        _currentState= CameraState.Move;
    }

    private bool PlayerRayTest()
    {
            RaycastHit hit;
            Vector3 rayDir = _playerTransform.position - transform.position;
            if(Physics.Raycast(transform.position, rayDir, out hit, Mathf.Infinity, _rayLayerMask))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("vu");
                    _currentState= CameraState.Lock;
                    return true;
                }
            }
            Debug.Log("pas vu");
            return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ça collide avec le joueur");
            _playerTransform = other.transform;
            _currentState = CameraState.Lock;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _currentState = CameraState.Move;
            _playerTransform = null;
            _lockCounter = 0;
        }
    }
}
