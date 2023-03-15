using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpening : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private BoolVariables _key;

    private void Awake()
    {
        _animator= GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_key==null && other.CompareTag("Player")) _animator.SetBool("OpenDoor", true);
        else if (_key && other.CompareTag("Player")) _animator.SetBool("OpenDoor", true);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player")) _animator.SetBool("CloseDoor", true);
    }

}
