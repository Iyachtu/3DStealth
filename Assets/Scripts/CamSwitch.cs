using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamSwitch : MonoBehaviour
{
    [SerializeField] GameObject[] _camList;
    private bool _playerNear;
    [SerializeField] private string _text;
    [SerializeField] private TMP_Text _textBox;
    [SerializeField] private GameObject _interface, _player;

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
        if (_playerNear && _player.GetComponent<PlayerMove>()._actionInput.action.ReadValue<float>()>0.1f)
        {
            DeactivateCam();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _textBox.text = _text;
            _interface.SetActive(true);
            _playerNear= true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("ça sort du trigger");
            _interface.SetActive(false);
            _playerNear = false;
    }


    private void DeactivateCam()
    {
        if (_camList.Length > 0)
        {
            foreach (var cam in _camList)
            {
                cam.GetComponentInChildren<CamBehavior>()._isActive = false;
            }
        }
        _interface.SetActive(false);
    }
}
