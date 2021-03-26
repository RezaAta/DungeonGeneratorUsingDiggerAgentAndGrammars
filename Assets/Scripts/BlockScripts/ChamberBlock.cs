using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberBlock : Block
{

    public Hallway connectedHallway = null;

    public void PlaceFloorMesh()
    {
        Instantiate(floorPrefab, this.transform.position, Quaternion.identity, this.transform);
    }


    void Awake()
    {
        base.Initialize();
        this.PlaceFloorMesh();
    }
}
