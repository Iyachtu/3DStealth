using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialStarter : MonoBehaviour
{
    [SerializeField] private int _dialNum;
    [SerializeField] private GameObject _dialManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _dialManager.GetComponent<VN>().LoadQueue(_dialNum);
            Destroy(gameObject);
        }
    }
}
