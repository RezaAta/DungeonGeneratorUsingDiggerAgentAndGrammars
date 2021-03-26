using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualControler : Master
{
    private Rigidbody rigidBody;

    void Start()
    {
        if (this.rigidBody == null)
            rigidBody = this.gameObject.GetComponent<Rigidbody>();

    }
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        float moveInXAxis = Input.GetAxis("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveInZAxis = Input.GetAxis("Vertical");

        //Use the two store floats to create a new Vector2 variable movement.
        Vector3 movement = new Vector3(moveInXAxis,0, moveInZAxis);

        this.gameObject.transform.position = this.gameObject.transform.position + movement * this.gameObject.GetComponent<AgentAttributes>().movementSpeed;
        
        this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("AxisX", moveInXAxis);
        this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("AxisZ", moveInZAxis);
       
    }

}
