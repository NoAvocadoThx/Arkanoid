using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //private
    //transform of the paddle
    private Transform _paddle;

    //public 
    public float velocity;
    public float mouseVelocity;
    public float borderLimit;

    // Start is called before the first frame update
    void Start()
    {
        //cache reference
        _paddle = GetComponent<Transform>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //check if paddle exists
        if (_paddle)
        {
            //get key movement direction
            float direction = Input.GetAxis("Horizontal");
            //get mouse movement direction
            float mouseDir = Input.GetAxis("Mouse X");
            //if any key bind to "Horizontal" is pressed
            if (direction != 0)
            {
                //move the paddle
                _paddle.position = _paddle.position + (Vector3.right * direction * velocity);
            }
            //if using mouse
            else if (mouseDir != 0)
            {
                //move the paddle
                _paddle.position = _paddle.position + (Vector3.right * mouseDir * mouseVelocity);
            }
            //prevent paddle from going out of screen
            float curXPos = _paddle.position.x;
            curXPos = Mathf.Clamp(curXPos, -borderLimit, borderLimit);
            _paddle.position = new Vector3(curXPos, _paddle.position.y, _paddle.position.z);
        }
    }
}
