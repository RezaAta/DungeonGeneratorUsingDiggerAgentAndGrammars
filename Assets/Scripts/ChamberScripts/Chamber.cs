using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class Chamber : MonoBehaviour
{

    public List<GameObject> chamberObjects;//first row contains all static objects, 2nd row contains consumables and intractables, 3rd row contains agents.  
    public string type;
    public bool areNeighborsDescribed = false;
    public bool areNeighborsGenerated = false;
    public bool isGenerated = false;
    public List<Chamber> neighborChambers;
    public List<Hallway> connectedHallways;

    public Hallway GetConnectingHallway(Chamber thatChamber)
    {
        var commoHallways = this.connectedHallways.Intersect(thatChamber.connectedHallways);
        if (commoHallways.Count() == 0)
            return null;
        
        else if (commoHallways.Count()>1)
        {
            Debug.LogError("2 hallways connecting 2 chambers?? madness...");
            return null;
        }
        else
            return commoHallways.First();
    }

    private void ReCenterChamberPosition()
    {
        if (this.chamberObjects.Count != 0)
        {
            Vector3 sumOfAllPositions = new Vector3(0,0,0);
            Vector3 RepositionVector = new Vector3(0,0,0);

            for (int i = 0; i < this.chamberObjects.Count; i++)
                sumOfAllPositions = sumOfAllPositions + (this.chamberObjects[i].transform.position);
            
            RepositionVector = this.transform.position - sumOfAllPositions / this.chamberObjects.Count;

            for (int i = 0; i < this.chamberObjects.Count; i++)
                this.chamberObjects[i].transform.position += RepositionVector;

            
        }
    }


    public virtual void GenerateChamber()
    {
        StartCoroutine(BeginChamberGeneration());
    }
    public virtual IEnumerator BeginChamberGeneration()
    {
        yield return new WaitForFixedUpdate();
        //this.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();

    }

    public void Awake()
    {
        //if (this.GetComponent<NavMeshSurface>() == null)
        //{
        //    this.gameObject.AddComponent<NavMeshSurface>();
        //    this.gameObject.GetComponent<NavMeshSurface>().collectObjects = UnityEngine.AI.CollectObjects.All;
        //}
    }
    // Start is called before the first frame update
    void Start()
    {
        this.ReCenterChamberPosition();
    }
}
