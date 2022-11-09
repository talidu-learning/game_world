using System;
using BuildingSystem;
using UnityEngine;

namespace Interactables
{
    public class Socket : MonoBehaviour
    {
        private CustomTransformer _customTF;

        private void Awake()
        {
            _customTF = GetComponent<CustomTransformer>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Socket"))
            {
                Debug.Log("Enable");
                _customTF.EntersSocket();
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Socket"))
            {
                Debug.Log("Disable");
                _customTF.LeavesSocket();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Socket"))
            {
                Debug.Log("Enable");
                _customTF.EntersSocket();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Socket"))
            {
                Debug.Log("Disable");
                _customTF.LeavesSocket();
            }
        }
    }
}