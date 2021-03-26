using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    
    public static MapGenerator instance { get; private set; }

    public SimpleDigger simpleDiggerPrefab;

    public SimpleDigger.Direction preferredDirection = SimpleDigger.Direction.Right;

    public NavMeshSurface navMeshHandler;

    public void GenerateAnticipatedChamberNeighbors(Chamber anticipatedChamber, List<Hallway> allNewHallways)
    {
        StartCoroutine(BeginGenerateAnticipatedChamberNeighborsCoroutine(anticipatedChamber, allNewHallways));
    }

    public IEnumerator BeginGenerateAnticipatedChamberNeighborsCoroutine(Chamber anticipatedChamber, List<Hallway> allNewHallways)
    {
        foreach (SimpleLinearHallway newHallway in allNewHallways)
        {
            SimpleDigger digger = Instantiate(simpleDiggerPrefab);

            if (newHallway.connectedChambers.Count > 2)
            {
                List<Chamber> endChambers = newHallway.connectedChambers;
                Debug.LogError("can't dig hallway to multiple hallways!");
                continue;
                //digger.StartDigging(startChamber,endChambers,newHallway,this.preferredDirection);

            }
            else
            {
                if (newHallway.connectedChambers[0] is RectangularChamber && newHallway.connectedChambers[1] is RectangularChamber)
                {
                    RectangularChamber startChamber = newHallway.connectedChambers[0] as RectangularChamber;
                    RectangularChamber endChambers = newHallway.connectedChambers[1] as RectangularChamber;

                    if (startChamber.isGenerated == true && endChambers.isGenerated == false)
                        digger.StartDigging(startChamber, endChambers, newHallway, this.preferredDirection);

                    else if (startChamber.isGenerated == false)
                        Debug.LogError("can't dig hallway from a chamber that has not been generated!");

                    else if (startChamber.isGenerated == true & endChambers.isGenerated == true)
                        digger.DigHallwayBetweenExistingChambers(startChamber, endChambers, newHallway);
                    
                }
            }
            Destroy(digger.gameObject);
            GameSystem.instance.allHallways.Add(newHallway);
            yield return new WaitForFixedUpdate();
        }
        anticipatedChamber.areNeighborsGenerated = true;

    }
    public void GenerateAnticipatedChamberNeighbors(Chamber anticipatedChamber)
    {

    }


    public void PlaceAllNewChambersNearAnticipatedChamber(Chamber anticipatedChamber,List<Chamber> allNewChambers)
    {

    }

    //public void PlaceAllNewChambersNearAnticipatedChamber(RectangularChamber anticipatedChamber, List<RectangularChamber> allNewChambers)
    //{
    //    PlaceChamberLinear(anticipatedChamber, allNewChambers);
    //}

    //private void PlaceChamberLinear(RectangularChamber anticipatedChamber, List<RectangularChamber> allNewChambers)
    //{
    //    Vector3 anticipatedChamberCoordinates = anticipatedChamber.gameObject.transform.position;

    //    foreach (Chamber neighborChamber in anticipatedChamber.neighborChambers)
    //    {
    //        if (neighborChamber.isGenerated == false)
    //        {
    //            int x, y, z;
    //            anticipatedChamber.position
    //            Vector3 neighborChamberPosition = new Vector3(x,y,z);
    //            neighborChamber.gameObject.transform.SetPositionAndRotation();
    //        }
    //    }
    //}


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one MapGenerator!");
            return;
        }

        instance = this;
    }


}
