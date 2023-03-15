using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamSwitch : MonoBehaviour
{
    [SerializeField] GameObject[] _camList;
    [SerializeField] private InputActionReference _action;
    private bool _playerNear;

    private void Awake()
    {
        _playerNear= false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerNear && _action.action.ReadValue<float>()>0.1f)
        {
            DeactivateCam();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //afficher interface 
        _playerNear= true;
    }

    private void OnTriggerExit(Collider other)
    {
        //virer interface
        _playerNear= false;
    }


    private void DeactivateCam()
    {
        if (_camList.Length > 0)
        {
            foreach (var cam in _camList)
            {
                cam.GetComponent<CamBehavior>()._isActive = false;
            }
        }
    }
}
