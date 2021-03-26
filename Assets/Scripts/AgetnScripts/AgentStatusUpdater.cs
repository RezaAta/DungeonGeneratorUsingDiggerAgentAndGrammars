using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentStatusUpdater : MonoBehaviour
{
    public List<string> stateLog;
    public List<GameObject> locationLog = new List<GameObject>();

    public void UpdateAgentLocation()
    {

    }
    public void SendLocationUpdateToOtherComponents()
    {

    }
    public void UpdateAgentLocations()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        this.locationLog = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateAgentLocation();
    }

    public void UpdateLastLocation(Collider collider)
    {
        if (this.locationLog.Count == 0)
            this.locationLog.Add(collider.gameObject.transform.parent.gameObject);

        
        else if (this.locationLog[0] != collider.gameObject.transform.parent.gameObject)
            this.locationLog.Insert(0,collider.gameObject.transform.parent.gameObject);

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.transform.parent.GetComponent<Hallway>() != null | other.gameObject.transform.parent.GetComponent<Chamber>() != null)
            this.UpdateLastLocation(other);

        if (other.gameObject.GetComponent<HallwayBlock>() != null)
        {
            other.gameObject.transform.parent.GetComponent<SimpleLinearHallway>().AgentEnters(this.gameObject.GetComponent<HumanoidAgent>());
            //if (other.gameObject.transform.parent.GetComponent<Hallway>().hasBeenEntered == false)
            //{
            //    Hallway thisHallway = other.gameObject.transform.parent.GetComponent<Hallway>();
            //    foreach (Chamber anticipatedChamber in thisHallway.connectedChambers)
            //    {
            //        if (anticipatedChamber.areNeighborsDescribed == false)
            //            GrandGenerator.instance.BeginNeighborGeneration(this.gameObject.GetComponent<Agent>(), anticipatedChamber);

            //    }
            //}
        }

        //if (other.gameObject.transform.parent.GetComponent<Chamber>() != null && other.gameObject.transform.parent != locationLog.First())
        //{
        //    this.gameObject.GetComponent<AgentAttributes>().UpdateHealth();
        //}
    }
}
