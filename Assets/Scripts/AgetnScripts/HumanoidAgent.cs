using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanoidAgent : MonoBehaviour
{
    public AgentStatusUpdater statusUpdater;
    public AgentAttributes attributes;
    public Rigidbody agentRigidBody;
    public Intelligence intelligence;
    public ActorBody actorBody;//contains collider, material, mesh
    public BoxCollider logicalCollider;

    public NavMeshAgent navMeshAgent;

    public Chamber destinationChamber;

    //private void FixedUpdate()
    //{
    //    if (destinationChamber != null && destinationChamber.transform.position != navMeshAgent.destination)
    //    {   
    //        navMeshAgent.destination = destinationChamber.transform.position;
    //        navMeshAgent.acceleration = 100;
    //        navMeshAgent.isStopped = false;

    //        if (navMeshAgent.isOnOffMeshLink)
    //            navMeshAgent.CompleteOffMeshLink();
    //    }
        
    //}

    public void SetIntelligence<newIntelligenceType>()where newIntelligenceType : Intelligence
    {
        Destroy(this.intelligence);
        this.intelligence = this.gameObject.AddComponent<newIntelligenceType>();
    }
    
    void Awake()
    {
        if (this.statusUpdater == null)
            this.statusUpdater = this.gameObject.AddComponent<AgentStatusUpdater>();

        if (this.attributes == null)
            this.attributes = this.gameObject.AddComponent<AgentAttributes>();

        if (this.agentRigidBody == null)
        {
            this.agentRigidBody = this.gameObject.AddComponent<Rigidbody>();
            this.agentRigidBody.isKinematic = true;
        }

        if (this.intelligence == null)
            this.intelligence = this.gameObject.AddComponent<Intelligence>();

        if (this.actorBody == null)
            Debug.LogError("agent has no physical body.");
        else
        { 
            this.actorBody.gameObject.transform.parent = this.gameObject.transform;
            this.actorBody.agent = this;
        }

        if (this.logicalCollider == null)
        {
            this.logicalCollider = this.gameObject.AddComponent<BoxCollider>();
            this.logicalCollider.size = new Vector3(1.5f, 3f, 1f);
            this.logicalCollider.center = this.transform.position + new Vector3(0,1,0);
            this.logicalCollider.isTrigger = true;
        }

        if (this.navMeshAgent == null)
        {
            this.navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();

            //var agentTypeID = NavMesh.CreateSettings().agentTypeID;
            //ref NavMeshBuildSettings buildSet = ref NavMesh.GetSettingsByID(agentTypeID);

            this.navMeshAgent.height = this.actorBody.GetComponent<SkinnedMeshRenderer>().bounds.extents.y * 2;
            this.navMeshAgent.radius = Math.Max(this.actorBody.GetComponent<SkinnedMeshRenderer>().bounds.extents.x, this.actorBody.GetComponent<SkinnedMeshRenderer>().bounds.extents.z) * 2;
            this.navMeshAgent.speed = this.attributes.movementSpeed;
            this.navMeshAgent.autoTraverseOffMeshLink = true;
            this.navMeshAgent.isStopped = true;
            
        }
    }
   

}
