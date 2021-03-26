using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hallway : MonoBehaviour
{
    public List<Tuple<GameObject,HallwayBlock>> itemsAndTheirBlock = new List<Tuple<GameObject, HallwayBlock>>();//first row contains all static objects, 2nd row contains consumables and intractables, 3rd row contains agents.
    public bool hasBeenEntered = false;

    public List<Chamber> connectedChambers = new List<Chamber>();

    public void SetFirstChamber(Chamber firstChamber)
    {
        if (this.connectedChambers.Count == 0)
        {
            this.connectedChambers = new List<Chamber>();
            this.connectedChambers.Add(firstChamber);
        }

        else
            this.connectedChambers.Insert(0,firstChamber);
    }

    public void GenerateHallway()
    {
        StartCoroutine(BeginHallwayGeneration());
    }
    public IEnumerator BeginHallwayGeneration()
    {
        yield return new WaitForFixedUpdate();
        //this.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public virtual void AgentEnters(HumanoidAgent agent)
    {
        
    }

    public void Awake()
    {
        //if (this.GetComponent<NavMeshSurface>() == null)
        //{
        //    this.gameObject.AddComponent<NavMeshSurface>();
        //    this.gameObject.GetComponent<NavMeshSurface>().collectObjects = UnityEngine.AI.CollectObjects.All; 
        //}
    }

}
