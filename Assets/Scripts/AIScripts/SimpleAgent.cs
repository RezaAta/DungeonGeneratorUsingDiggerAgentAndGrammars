using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class AStarNode
{
    public Chamber chamber;
    public float totalCost;
    public float realCost;
    public float estimatedCostToGoal;
    public AStarNode previous;
}

public class SimpleAgent : Master
{
    List<GameObject> travelQueue = new List<GameObject>();
    public void FindPath(Vector3 destination)
    {

    }
    public bool FindPath(GameObject target)
    {
        if (target.GetComponent<Chamber>() != null)
        {
            //TODO:implement A* on Chambers
            this.travelQueue = new List<GameObject>(UseAStarPathFinding(agent.statusUpdater.locationLog.Last().GetComponent<Chamber>(), target.GetComponent<Chamber>()));
            return true;
        }
        else if (target.GetComponent<Hallway>() != null)
        {
            Debug.Log("eh... not implemented...");
        }
        else if (target.GetComponent<Block>())
        {
            if (this.FindPath(target.transform.parent.gameObject))
                this.gameObject.GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
        }
        return false;
    }

    private List<GameObject> UseAStarPathFinding(Chamber startingPoint, Chamber target)
    {

        List<Chamber> currentMapOfChambers = new List<Chamber>(GameSystem.instance.allChambers);

        List<AStarNode> openSet = new List<AStarNode>();

        float currentCostValue = 0;


        AStarNode startingNode = new AStarNode();
        startingNode.chamber = startingPoint;
        startingNode.totalCost = 0f;
        startingNode.realCost = 0f;
        startingNode.estimatedCostToGoal = EstimateRemainingDistance(startingPoint, target);
        startingNode.previous = null;

        openSet.Add(startingNode);

        List<AStarNode> closedSet = new List<AStarNode>();

        List<AStarNode> chosenNodes = new List<AStarNode>();


        while (openSet.Any())
        {
            int bestChamberIndex = 0;

            for (int i = 0; i < openSet.Count(); i++)
                if (openSet[i].totalCost < openSet[bestChamberIndex].totalCost)
                {
                    bestChamberIndex = i;
                    if (openSet[i].estimatedCostToGoal == 0f)
                        break;
                }

            AStarNode current = openSet[bestChamberIndex];

            chosenNodes = new List<AStarNode>();
            
            AStarNode thisNode = current;
            
            while (thisNode.previous != null)
            {
                chosenNodes.Insert(0,thisNode);
                thisNode = thisNode.previous;
            }
            

            if (current.chamber == target)
            {
                Debug.Log("A* Done Successfully");
                break;
            }

            currentCostValue += current.realCost;

            closedSet.Add(current);
            openSet.RemoveAt(bestChamberIndex);
            foreach (Chamber neighbor in current.chamber.neighborChambers)
            {
                if (!openSet.Exists(t => t.chamber == neighbor) && currentMapOfChambers.Exists(t => t == neighbor))
                {
                    SimpleLinearHallway hallwayBetweenChambers = null;

                    foreach (SimpleLinearHallway hallway in neighbor.connectedHallways)
                        if (hallway.connectedChambers.Exists(chamber => chamber == current.chamber))
                            hallwayBetweenChambers = hallway;

                    if (hallwayBetweenChambers == null)
                        Debug.Log("hallwayBetweenChambers == null");

                    AStarNode newNode = new AStarNode();

                    newNode.chamber = neighbor;
                    newNode.realCost =  currentCostValue + hallwayBetweenChambers.length;
                    newNode.estimatedCostToGoal = EstimateRemainingDistance(neighbor , target);
                    newNode.totalCost = newNode.estimatedCostToGoal + newNode.realCost;
                    newNode.previous = current;

                    openSet.Add(newNode);
                }
            }
        }

        //if (startingPoint.GetComponent<Chamber>() != null)
        //{
        //    List<Hallway> adjacentHallways = startingPoint.GetComponent<Chamber>().connectedHallways;
        //    foreach (Hallway hallway in adjacentHallways)
        //    {
        //        CalculateCost(hallway,target);
        //    }
        //}
        List<GameObject> foundPath = new List<GameObject>();

        foreach (var node in chosenNodes)
        {
            foundPath.Add(node.chamber.gameObject);
        }

        return foundPath;
    }

    private float EstimateRemainingDistance(Chamber startingPoint, Chamber target)
    {
        Vector3 distance = target.transform.position - startingPoint.transform.position;

        return distance.magnitude;
    }
    private void FixedUpdate()
    {
        if (agent.destinationChamber != null && agent.destinationChamber.transform.position != agent.navMeshAgent.destination && agent.navMeshAgent.isStopped && !travelQueue.Any())
        {
            if (this.FindPath(agent.destinationChamber.gameObject))
            {
                Debug.Log("moving to target");
            }
        }

        if (this.travelQueue.Any() && agent.navMeshAgent.isStopped)
        {
            agent.navMeshAgent.destination = this.travelQueue.First().transform.position;
            this.travelQueue.RemoveAt(0);
            agent.navMeshAgent.acceleration = 100;
            agent.navMeshAgent.isStopped = false;
        }

        if (agent.transform.position == agent.navMeshAgent.destination && agent.navMeshAgent.isStopped == false)
        {
            agent.navMeshAgent.isStopped = true;

            if (agent.statusUpdater.locationLog.First().GetComponent<Chamber>() == agent.destinationChamber)
            {
                Debug.Log("destination reached.");
                agent.destinationChamber = null;
            }
        }

        if (agent.navMeshAgent.isOnOffMeshLink)
            agent.navMeshAgent.CompleteOffMeshLink();
    }
}
