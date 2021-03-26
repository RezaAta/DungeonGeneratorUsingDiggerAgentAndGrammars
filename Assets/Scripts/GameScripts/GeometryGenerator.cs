using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class GeometryGenerator : MonoBehaviour
{
    public static GeometryGenerator instance { get; private set; }


    public void BeginGenerationOfChambers(Chamber anticipatedChamber, List<Chamber> storyTellerAllNeighborChambers)
    {
        throw new System.NotImplementedException();
    }
    public void BeginGenerationOfChambers(RectangularChamber anticipatedChamber, List<Chamber> newNeighBorChambers)
    {
        foreach (RectangularChamber newNeighBorChamber in newNeighBorChambers)
        {
            newNeighBorChamber.GenerateChamber();
            newNeighBorChamber.UpdateChamberShapeMatrix();
        }
        
    }

    private void GenerateBlock(Block block)
    {
        Instantiate(block.floorPrefab, new Vector3(block.transform.position.x, block.transform.position.y, block.transform.position.z), Quaternion.identity);
    }

    public void BeginGenerationOfHallwayBetweenChambers(List<Hallway> allNewHallways)
    {
        foreach (Hallway newHallway in allNewHallways)
            this.GenerateHallway(newHallway);
    }

    private void GenerateHallway(Hallway newHallway)
    {
        //throw new System.NotImplementedException();
    }
    private void GenerateHallway(SimpleLinearHallway newHallway)
    {
        //foreach (HallwayBlock newHallwayBlock in newHallway.allBlocks)
        //{
        //    newHallwayBlock.PlaceFloorMesh();
        //}
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one GeometryGenerator!");
            return;
        }

        instance = this;
    }



}
