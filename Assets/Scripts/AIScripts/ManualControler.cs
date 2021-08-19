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
        float moveInXAxis = Input.GetAxis("Horizontal");

        float moveInZAxis = Input.GetAxis("Vertical");

        bool attacking = Input.GetMouseButton(0);

        Vector3 movement = new Vector3(moveInXAxis,0, moveInZAxis).normalized;

        float cameraYRotation = Camera.main.transform.rotation.eulerAngles.y;

        movement = Quaternion.AngleAxis(cameraYRotation, Vector3.up)* movement;

        this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("AxisX", moveInXAxis);
        this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("AxisZ", moveInZAxis);
        
        if (moveInXAxis != 0 || moveInZAxis != 0)
        {
            Move(movement);
        }
        else
            this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);

        if (attacking == true)
        {
            this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetTrigger("Attacking");
        }
        //Debug.Log(this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.GetFloat("Speed"));
        
    }

    private void Move(Vector3 movement)
    {
        this.gameObject.transform.position = this.gameObject.transform.position + movement* this.gameObject.GetComponent<AgentAttributes>().movementSpeed * Time.deltaTime;
        this.gameObject.GetComponent<HumanoidAgent>().actorBody.animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);

        this.gameObject.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
    }
}
