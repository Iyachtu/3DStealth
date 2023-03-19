using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardPicking : MonoBehaviour
{
    [SerializeField] private BoolVariables _item;
    private bool _playerNear;
    [SerializeField] private string _text;
    [SerializeField] private TMP_Text _textBox;
    [SerializeField] private GameObject _interface, _player;

    private void Awake()
    {
        _playerNear = false;
        _item.value = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_playerNear && _player.GetComponent<PlayerMove>()._actionInput.action.ReadValue<float>() > 0.1f)
        {
            ItemPick();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _textBox.text = _text;
            _interface.SetActive(true);
            _playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _interface.SetActive(false);
        _playerNear = false;
    }


    private void ItemPick()
    {
        _item.value = true;
        _interface.SetActive(false);
    }
}
