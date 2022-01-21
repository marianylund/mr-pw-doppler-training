using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSheep : MonoBehaviour
{
    private Renderer _rend;
    [SerializeField] private Material red;
    [SerializeField] private Material green;
    [SerializeField] private Material normal;

    private void Start()
    {
        _rend = GetComponentInParent<Renderer>();
        _rend.material = normal;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER " + other.name);
        if (other.name.Contains("sheep"))
        {
            _rend.material = green;
        }
        else if (other.name.Contains("ogre"))
        {
            _rend.material = red;
        }
        else
        {
            _rend.material = normal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit ");
        _rend.material = normal;

    }
}
