using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveShaderController : MonoBehaviour
{
    public bool testMe;
    private float _dissolveValue;

    public MeshRenderer meshRenderer;

    public void Update()
    {
        if (testMe)
        {
            _dissolveValue += 1 * Time.deltaTime;
        }
        else
        {
            _dissolveValue -= 1 * Time.deltaTime;
        }
        
        meshRenderer.material.SetFloat("_DissolveScale",_dissolveValue);
    }
}
