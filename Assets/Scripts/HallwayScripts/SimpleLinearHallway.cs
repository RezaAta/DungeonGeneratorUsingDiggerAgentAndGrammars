using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLinearHallway : Hallway
{
    public HallwayBlock hallwayBlockPrefab;
    public LinkedList<HallwayBlock> allBlocks = new LinkedList<HallwayBlock>();
    public HallwayBlock firstBlock;
    public HallwayBlock lastBlock;
    public int length;

    public override void AgentEnters(HumanoidAgent agent)
    {
        if (this.hasBeenEntered == false)
        {
            if (this.connectedChambers.Count == 2)
            {
                if (this.lastBlock.ConnectedChamber.areNeighborsDescribed == false)
                {
                    GrandGenerator.instance.BeginNeighborGeneration(agent, this.lastBlock.ConnectedChamber);
                }

            }
            else
            {
                foreach (Chamber anticipatedChamber in this.connectedChambers)
                {
                    if (anticipatedChamber.areNeighborsDescribed == false)
                        GrandGenerator.instance.BeginNeighborGeneration(agent, anticipatedChamber);

                }
            }
        }

    }

 
}
