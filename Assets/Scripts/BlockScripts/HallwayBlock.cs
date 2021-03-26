using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayBlock : Block
{
    //public HallwayBlock previousBlock;
    //public HallwayBlock nextBlock;

    public RectangularChamber ConnectedChamber = null;
    public void PlaceFloorMesh()
    {
        Instantiate(floorPrefab, this.transform.position, Quaternion.identity, this.transform);
    }

    void Awake()
    {
        base.Initialize();
        this.PlaceFloorMesh();
        //this.GetComponent<BoxCollider>().isTrigger = true;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("shite");
    //    if (other.gameObject.GetComponent<ActorBody>() !=null )
    //    {
    //        Agent agent = other.gameObject.GetComponent<ActorBody>().agent;
    //        Hallway parentHallway = this.gameObject.transform.parent.GetComponent<Hallway>();
    //        if (parentHallway.hasBeenEntered == false)
    //        {
    //            foreach (Chamber anticipatedChamber in parentHallway.connectedChambers)
    //            {
    //                if (anticipatedChamber.areNeighborsDescribed == false)
    //                {
    //                    GrandGenerator.instance.BeginNeighborGeneration(agent, anticipatedChamber);

    //                }
    //            }
    //        }

    //    }
    //}

}
