using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum SpottingState { Spotting, Looking, Running }

public class StaticEnemyBehavior : MonoBehaviour
{
    private NavMeshAgent _navAgent;
    private PatrolState _currentState;

    private Transform _playerTransform = null;
    private float _lockCounter = 0; //variable pour mesurer le temps pendant lequel le guarde a vu le joueur
    [SerializeField] private float _runTimer;
    [SerializeField] private LayerMask _rayLayerMask;
    private Transform _lastSeenPlayer;

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _currentState = PatrolState.Spotting;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Debug.Log(_currentState);
        Behavior();
    }

    private void Behavior()
    {
        if (_currentState == PatrolState.Looking)
        {
            if (_playerTransform != null)
            {
                transform.LookAt(_lastSeenPlayer);
                _navAgent.SetDestination(_lastSeenPlayer.position);

                if (PlayerRayTest())
                {
                    _lockCounter += Time.fixedDeltaTime;
                    if (_lockCounter >= _runTimer) _currentState = PatrolState.Running;
                    //il a vu le joueur, il va à cette position rapidement
                    //s'il le voit plus de X second, il lui courre dessus pour le choper
                    //sinon, il va à la dernière position de détection

                }
                else _currentState = PatrolState.Looking;
            }
            else _currentState = PatrolState.Spotting;
        }
        else if (_currentState == PatrolState.Running)
        {
            Debug.Log("perdu, la patrouille t'a vu");
        }
        else if (_currentState == PatrolState.Spotting)
        {
            
        }

    }

    private void Move()
    {
        //if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_waypoints[_currentWP].position.x, _waypoints[_currentWP].position.z)) <= _WPdistanceTolerance)
        //{
        //    _currentWP++;
        //    if (_currentWP == _waypoints.Length) _currentWP = 0;
        //    _navAgent.SetDestination(_waypoints[_currentWP].position);
        //}
        //else _navAgent.SetDestination(_waypoints[_currentWP].position);

    }

    private bool PlayerRayTest()
    {
        RaycastHit hit;
        Vector3 rayDir = _playerTransform.position - transform.position;
        if (Physics.Raycast(transform.position, rayDir, out hit, Mathf.Infinity, _rayLayerMask))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                _currentState = PatrolState.Looking;
                _lastSeenPlayer = hit.collider.transform;
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _lastSeenPlayer = _playerTransform = other.transform;
            _currentState = PatrolState.Looking;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _currentState = PatrolState.Spotting;
            _playerTransform = null;
            _lockCounter = 0;
        }
    }
}
