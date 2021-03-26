using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandGenerator : MonoBehaviour
{
    public static GrandGenerator instance { get; private set; }

    public StoryTeller storyTeller;
    public MapGenerator mapGenerator;
    public GeometryGenerator geometryGenerator;

    public void BeginNeighborGeneration(HumanoidAgent agent, Chamber anticipatedChamber)
    {
        StartCoroutine(BeginNeighborGenerationCoroutine(agent,anticipatedChamber));
    }
    public IEnumerator BeginNeighborGenerationCoroutine(HumanoidAgent agent, Chamber anticipatedChamber)
    {
        this.storyTeller.DescribeNeighborChambers(agent, anticipatedChamber);
        yield return new WaitForFixedUpdate();
        this.storyTeller.DescribeAllHallways();
        yield return new WaitForFixedUpdate();
        this.mapGenerator.GenerateAnticipatedChamberNeighbors(anticipatedChamber,storyTeller.allNewHallways);
        yield return new WaitForFixedUpdate();
        foreach (Chamber newChamber in storyTeller.allNeighborChambers)
            this.storyTeller.DescribeChamberContent(agent, newChamber);

        yield return new WaitForFixedUpdate();
        //mapGenerator.PlaceAllNewChambersNearAnticipatedChamber(anticipatedChamber,storyTeller.allNeighborChambers);
        //this.geometryGenerator.BeginGenerationOfChambers(anticipatedChamber,storyTeller.allNeighborChambers);
        //this.geometryGenerator.BeginGenerationOfHallwayBetweenChambers(storyTeller.allNewHallways);

        yield return new WaitForFixedUpdate();
        this.mapGenerator.navMeshHandler.UpdateNavMesh(this.mapGenerator.navMeshHandler.navMeshData);

    }
    public void GenerateStartingChambers()
    {
        StartCoroutine("BeginStartingChambersGeneration");
    }

    public IEnumerator BeginStartingChambersGeneration()
    {
        this.CreateStartingSafeHouse();
        yield return new WaitForFixedUpdate();
        this.CreateStartingChamber();
        yield return new WaitForFixedUpdate();
        this.mapGenerator.GenerateAnticipatedChamberNeighbors(GameSystem.instance.safeHouse,storyTeller.allNewHallways);
        yield return new WaitForFixedUpdate();
        this.geometryGenerator.BeginGenerationOfChambers(GameSystem.instance.safeHouse,storyTeller.allNeighborChambers);
        yield return new WaitForFixedUpdate();
        this.geometryGenerator.BeginGenerationOfHallwayBetweenChambers(storyTeller.allNewHallways);
        yield return new WaitForFixedUpdate();
        this.mapGenerator.navMeshHandler.BuildNavMesh();
        yield return new WaitForFixedUpdate();
        this.CreateDefaultAgent(GameSystem.instance.safeHouse);
    }
    public void CreateStartingSafeHouse()
    {
        SafeHouseChamber safeHouse = Instantiate(GameSystem.instance.safeHouseChamberPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        safeHouse.sizeX = 5;
        safeHouse.sizeZ = 5;

        safeHouse.areNeighborsDescribed = true;

        safeHouse.InitializeShapeMatrixWithFloors();
        safeHouse.InstantiateAllBlocks();
        safeHouse.GenerateChamber();

        GameSystem.instance.safeHouse = safeHouse;
        GameSystem.instance.allChambers.Add(safeHouse);
    }
    public void CreateStartingChamber()
    {
        NormalChamber secondChamber = Instantiate(GameSystem.instance.normalChamberPrefab);
        
        storyTeller.allNeighborChambers.Add(secondChamber);

        secondChamber.sizeX = 5;
        secondChamber.sizeZ = 5;

        secondChamber.isGenerated = false;
        secondChamber.areNeighborsDescribed = false;

        secondChamber.neighborChambers.Add(GameSystem.instance.safeHouse);
        GameSystem.instance.safeHouse.neighborChambers.Add(secondChamber);
        storyTeller.AddHallwayBetweenChambers(secondChamber,GameSystem.instance.safeHouse,GameSystem.instance.linearHallwayPrefab);

        secondChamber.InitializeShapeMatrixWithFloors();


        GameSystem.instance.startingChamber = secondChamber;
        GameSystem.instance.allChambers.Add(secondChamber);
    }
    public void CreateDefaultAgent(SafeHouseChamber agentSafeHouse)
    {
        if (agentSafeHouse.isGenerated == false)
        {
            Debug.LogError("cant Create Agent in A non-Generated Chamber.");
        }
        else
        {
            HumanoidAgent defaultAgent = Instantiate(GameSystem.instance.defaultAgentPrefab, agentSafeHouse.transform.position, Quaternion.identity);

            agentSafeHouse.owner = defaultAgent;

            Camera.main.GetComponent<CameraFollow>().target = defaultAgent.transform;

            defaultAgent.SetIntelligence<ManualControler>();

            GameSystem.instance.defaultAgent = defaultAgent;
            GameSystem.instance.allAgents.Add(defaultAgent);

        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one GrandGenerator!");
            return;
        }

        instance = this;
    }

}
