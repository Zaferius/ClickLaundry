using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CODE_Swipe : MonoBehaviour
{
    // Paste those variables to GameManager
    
    //-- Mechanic Variables
    
    // touch start position, touch end position, swipe direction
    private Vector2 _startPos, _endPos, _direction; 
    
    // to calculate swipe time to control throw force in Z direction
    private float _touchTimeStart, _touchTimeFinish, _timeInterval; 

    // to control throw force in X and Y directions
    public float throwForceInXAndY = 1f; 
    
    // to control throw force in Z direction
    public float throwForceInZ = 50f;

    public GameObject player;
    private Rigidbody _playerRb;
    //-- Mechanic Variables

    private void Start()
    {
        _playerRb = player.GetComponent<Rigidbody>();
    }

    private void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchTimeStart = Time.time;
            _startPos = Input.mousePosition;
        }
        
        if (Input.GetMouseButtonUp(0)) 
        {
            
            // marking time when you release it
            _touchTimeFinish = Time.time;

            // calculate swipe time interval 
            _timeInterval = _touchTimeFinish - _touchTimeStart;

            // getting release finger position
            _endPos = Input.mousePosition;

            // calculating swipe direction in 2D space
            _direction = _startPos - _endPos;

            // add force to balls rigidbody in 3D space depending on swipe time, direction and throw forces
            _playerRb.AddForce(-_direction.x * throwForceInXAndY, -_direction.y * throwForceInXAndY, 0f);
        }
    }
}
