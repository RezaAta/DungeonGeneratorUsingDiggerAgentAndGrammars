using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class GameSystem : MonoBehaviour
{
    
    public static GameSystem instance { get; private set; }

    public Chamber chamberPrefab;
    public NormalChamber normalChamberPrefab;
    public TreasureChamber treasureChamberPrefab;
    public MercenaryCamp mercenaryChamberPrefab;
    public TeleportationChamber teleportationChamberPrefab;
    public SafeHouseChamber safeHouseChamberPrefab;
    public BossChamber bossChamberPrefab;

    public NPC bossPrefab;
    public NPC mobPrefab;
    public Camp campPrefab;
    public Chest chestPrefab;
    public TeleportationAltar teleportationAltarPrefab;
    public Portal portalPrefab;

    public Hallway hallwayPrefab;
    public SimpleLinearHallway linearHallwayPrefab;

    public HumanoidAgent agentPrefab;
    public HumanoidAgent defaultAgentPrefab;
    
    public List<Chamber> allChambers = new List<Chamber>();
    public List<Hallway> allHallways = new List<Hallway>();
    public List<HumanoidAgent> allAgents = new List<HumanoidAgent>();

    public SafeHouseChamber safeHouse;
    public RectangularChamber startingChamber;
    public RectangularChamber hallwayFromSafeHouseToStartingChamber;
    public HumanoidAgent defaultAgent;

    public ChamberBlock chamberBlockPrefab;


    public List<NavMeshBuildSettings> allAgentNavMeshTypes = new List<NavMeshBuildSettings>();

    void Awake()
    {
        
        if (instance != null)
        {
            Debug.LogError("There is more than one GameSystem!");
            return;
        }

        instance = this;

    }


    void Start()
    {

        GrandGenerator.instance.GenerateStartingChambers();
    }


}
