using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Portal : InteractableObject
{
    public Portal otherPortal;

    public NavMeshLinkData linkToOtherPortal;
    public bool linkIsGenerated;
    public bool linkIsActive;

    private void Awake()
    {
        if (this.GetComponent<BoxCollider>() == null)
        {
            this.gameObject.AddComponent<BoxCollider>();
            this.GetComponent<BoxCollider>().isTrigger = true;
        }    
    }

    public override void Interact(GameObject interactor)
    {
        interactor.gameObject.transform.position = this.otherPortal.transform.position;
    }
    
    public void GenerateNavLinkTo(Portal portal)
    {
        linkToOtherPortal.agentTypeID = MapGenerator.instance.navMeshHandler.agentTypeID;
        linkToOtherPortal.area = MapGenerator.instance.navMeshHandler.defaultArea;
        linkToOtherPortal.startPosition = this.transform.position;
        linkToOtherPortal.endPosition = this.otherPortal.transform.position;
        linkToOtherPortal.bidirectional = true;

        NavMesh.AddLink(linkToOtherPortal);
        linkIsGenerated = true;
        linkIsActive = true;

        otherPortal.linkIsGenerated = true;
        otherPortal.linkIsActive = true;
        otherPortal.linkToOtherPortal = this.linkToOtherPortal;
    }
}
