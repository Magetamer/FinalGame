using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCont : PhysicsBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        desiredx = 0;
        if (Input.GetAxis("Horizontal") > 0) desiredx = 5;
        if (Input.GetAxis("Horizontal") < 0) desiredx = -5;

        if (Input.GetButton("Jump") && grounded) velocity.y= 10.5f;
    }
}
