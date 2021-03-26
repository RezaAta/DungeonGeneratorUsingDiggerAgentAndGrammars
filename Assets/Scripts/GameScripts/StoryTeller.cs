using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;
//TODO: set neighbors of chambers that are before the end chamber as Generated.

public class StoryTeller : MonoBehaviour
{
    public static StoryTeller instance { get; private set; }

    public List<Chamber> allNeighborChambers;
    public List<Hallway> allNewHallways;
    public List<Tuple<HumanoidAgent, Chamber>> listOfAnticipatedAgentsAndChambers;
    public string neighborsStructureDifficulty;
    public bool deadEnd;
    private Random random = new Random();

    public void DescribeNeighborChambers(HumanoidAgent currentAgent, Chamber anticipatedCurrentChamber)
    {
        this.listOfAnticipatedAgentsAndChambers = new List<Tuple<HumanoidAgent, Chamber>>();
        this.listOfAnticipatedAgentsAndChambers.Add(Tuple.Create(currentAgent, anticipatedCurrentChamber));
        this.allNeighborChambers = new List<Chamber>();
        this.allNewHallways = new List<Hallway>();
        foreach (Tuple<HumanoidAgent, Chamber> agentAndChamber in listOfAnticipatedAgentsAndChambers)
        {
            this.DetermineChambersTypes(agentAndChamber);// i haven't considered agent parties or team ups. i should revise the code's logic.
            
            //foreach (Chamber newChamber in this.allNeighborChambers)
            //    this.DescribeChamberContent(currentAgent,newChamber);
        }
        anticipatedCurrentChamber.areNeighborsDescribed = true;
    }

    public void DetermineChambersTypes(Tuple<HumanoidAgent, Chamber> agentAndChamber)
    {
        this.DetermineDifficulty(agentAndChamber.Item1);
        Chamber PreviousChamber = agentAndChamber.Item1.statusUpdater.locationLog[1].GetComponent<Chamber>();

        //string anticipatedCurrentChamberType = agentAndChamber.Item2.type;

        if (agentAndChamber.Item2 is SafeHouseChamber)
            this.AddChambers(agentAndChamber.Item2, GameSystem.instance.treasureChamberPrefab);
        

        else if (agentAndChamber.Item2 is TreasureChamber)
            this.GenerateTreasureChamberNeighborsBasedOnDifficulty(agentAndChamber.Item2);

        else if (agentAndChamber.Item2 is BossChamber)
            this.GenerateBossChamberNeighborsBasedOnDifficulty(agentAndChamber.Item2);

        else if (agentAndChamber.Item2 is NormalChamber)
            this.GenerateNormalChamberNeighborsBasedOnDifficulty(agentAndChamber.Item2);

        else if (agentAndChamber.Item2 is MercenaryCamp)
            this.GenerateMercenaryCampNeighborsBasedOnDifficulty(agentAndChamber.Item2);

        else if (agentAndChamber.Item2 is TeleportationChamber)
        {
            int numberOfNormalChambers = random.Next(1,4);//First intended value was 1,5
            this.AddChambers(agentAndChamber.Item2, GameSystem.instance.normalChamberPrefab,numberOfNormalChambers);
        }
    }

    public void DetermineDifficulty(HumanoidAgent agent)
    {
        this.neighborsStructureDifficulty = "hard";
        int healthPotionCount = 0;
        if (agent.attributes.items.Count != 0)
        {
            foreach (GameObject item in agent.attributes.items)//I SHOULD GET ITEM COUNT FROM INVENTORY CLASS, i should implement inventory class....
                if (item.name == "healthPotion")
                    healthPotionCount += 1;
        }

        if (agent.attributes.health.current > 5 / 10 * agent.attributes.health.max)
        {
            if (Math.Abs(agent.attributes.winRatio - (-1)) > 0.00001)
            {
                if (agent.attributes.winRatio >= 60)
                    this.neighborsStructureDifficulty = "hard";
                else if (agent.attributes.winRatio < 60)
                    this.neighborsStructureDifficulty = "normal";
            }
            if (healthPotionCount > 0)
                this.neighborsStructureDifficulty = "normal";
        }
        else
        {
            if (healthPotionCount == 0)
                this.neighborsStructureDifficulty = "normal";
            
            if (agent.attributes.health.current < 2 / 10 * agent.attributes.health.max)
            {
                if (healthPotionCount != 0)
                    this.neighborsStructureDifficulty = "veryEasy";
                else
                    this.neighborsStructureDifficulty = "Easy";
            }
        }
    }

    public void GenerateBossChamberNeighborsBasedOnDifficulty(Chamber anticipatedChamber)
    {
        int numberOfNormalChambers;
        if (neighborsStructureDifficulty == "hard")
        {
            this.AddChambers(anticipatedChamber,GameSystem.instance.treasureChamberPrefab);
            numberOfNormalChambers = random.Next(1,4);
            this.AddChambers(allNeighborChambers.Last(),GameSystem.instance.normalChamberPrefab,numberOfNormalChambers);
        }
        else if (neighborsStructureDifficulty == "normal")
        {
            numberOfNormalChambers = random.Next(1, 3);
            this.AddChambers(anticipatedChamber, GameSystem.instance.normalChamberPrefab, numberOfNormalChambers);
            this.AddNextChambersToANumberOfPreviousChambersRandomly(numberOfNormalChambers,GameSystem.instance.treasureChamberPrefab);
            if (numberOfNormalChambers == 1)
                this.AddChambers(this.allNeighborChambers.Last(), GameSystem.instance.teleportationChamberPrefab);
        }
        else if (neighborsStructureDifficulty == "easy")
        {
            this.AddChambers(anticipatedChamber, GameSystem.instance.teleportationChamberPrefab);
        }
        else if (neighborsStructureDifficulty == "veryEasy")
        {
            this.AddChambers(anticipatedChamber, GameSystem.instance.teleportationChamberPrefab);
            this.AddChambers(this.allNeighborChambers.Last(), GameSystem.instance.treasureChamberPrefab);
        }
    }
    public void GenerateNormalChamberNeighborsBasedOnDifficulty(Chamber anticipatedChamber)
    {
        int numberOfNormalChambers;
        int numberOfMercenaryCamps;
        int randomNumberOfPossibleNeighborHoods;
        if (this.neighborsStructureDifficulty == "hard")
        {
            randomNumberOfPossibleNeighborHoods = random.Next(1, 3);
            if (randomNumberOfPossibleNeighborHoods == 1)
            {
                this.AddChambers(anticipatedChamber, GameSystem.instance.bossChamberPrefab);
            }
            else if (randomNumberOfPossibleNeighborHoods == 2)
            {
                numberOfMercenaryCamps = random.Next(1, 4);
                this.AddChambers(anticipatedChamber, GameSystem.instance.mercenaryChamberPrefab, numberOfMercenaryCamps);
                this.AddNextChambersToANumberOfPreviousChambers(numberOfMercenaryCamps,GameSystem.instance.bossChamberPrefab);
            }
        }
        else if (neighborsStructureDifficulty == "normal")
        {
            ////i made some adjustments in here. this code differs from algorithm
            //randomNumberOfPossibleNeighborHoods = random.Next(1, 2);
            //if (randomNumberOfPossibleNeighborHoods == 1)
            //{
            //    numberOfNormalChambers = random.Next(1, 4);
            //    this.AddChambers(anticipatedChamber, GameSystem.instance.normalChamberPrefab, numberOfNormalChambers);
            //}
            //else if (randomNumberOfPossibleNeighborHoods == 2)
            //    this.AddChambers(anticipatedChamber, GameSystem.instance.bossChamberPrefab);

            //this.AddChambers(anticipatedChamber, GameSystem.instance.mercenaryChamberPrefab);
            //numberOfMercenaryCamps = random.Next(1, 4);
            //this.AddChambers(anticipatedChamber, GameSystem.instance.mercenaryChamberPrefab,numberOfMercenaryCamps);
            //i don't know why i made those changes, now im refactoring my code and this seems stupid. next time i should explain further why i do something.
            numberOfNormalChambers = random.Next(1, 2);
            AddChambers(anticipatedChamber,GameSystem.instance.normalChamberPrefab,numberOfNormalChambers);

            randomNumberOfPossibleNeighborHoods = random.Next(1, 3);
            if (randomNumberOfPossibleNeighborHoods == 1)
                AddNextChambersToANumberOfPreviousChambersRandomly(numberOfNormalChambers,GameSystem.instance.bossChamberPrefab);
            
            else if (randomNumberOfPossibleNeighborHoods == 2)
                AddNextChambersToANumberOfPreviousChambersRandomly(numberOfNormalChambers, GameSystem.instance.mercenaryChamberPrefab);

        }
        else if (neighborsStructureDifficulty == "easy")
        {
            numberOfNormalChambers = random.Next(1, 4);//first intended value was 1,5
            this.AddChambers(anticipatedChamber, GameSystem.instance.normalChamberPrefab,numberOfNormalChambers);
        }
        else if (neighborsStructureDifficulty == "veryEasy")
            this.AddChambers(anticipatedChamber, GameSystem.instance.treasureChamberPrefab);
        
    }
    public void GenerateTreasureChamberNeighborsBasedOnDifficulty(Chamber anticipatedChamber)
    {
        if (this.neighborsStructureDifficulty == "hard")
        {
            int numberOfMercenaryCamps = random.Next(1, 4);
            this.AddChambers(anticipatedChamber, GameSystem.instance.mercenaryChamberPrefab,numberOfMercenaryCamps);
            this.AddNextChambersToANumberOfPreviousChambers(numberOfMercenaryCamps,GameSystem.instance.bossChamberPrefab);
        }
        else if (this.neighborsStructureDifficulty == "normal")
        {
            int randomPossibleNeighborHoods = random.Next(1, 2);
            if (randomPossibleNeighborHoods == 1)
                this.AddChambers(anticipatedChamber, GameSystem.instance.bossChamberPrefab);
            
            else if (randomPossibleNeighborHoods == 2)
                this.AddChambers(anticipatedChamber, GameSystem.instance.mercenaryChamberPrefab);
        }
        else
            anticipatedChamber.areNeighborsDescribed = true;
        
    }
    public void GenerateMercenaryCampNeighborsBasedOnDifficulty(Chamber anticipatedChamber)
    {
        if (neighborsStructureDifficulty == "hard")
            this.AddChambers(anticipatedChamber, GameSystem.instance.bossChamberPrefab);

        else if (neighborsStructureDifficulty == "normal")
        {
            int numberOfNormalChambers = random.Next(1, 4);
            this.AddChambers(anticipatedChamber, GameSystem.instance.normalChamberPrefab,numberOfNormalChambers);
            this.AddNextChambersToANumberOfPreviousChambersRandomly(numberOfNormalChambers,GameSystem.instance.bossChamberPrefab);
        }
        else if (neighborsStructureDifficulty == "easy" || neighborsStructureDifficulty == "veryEasy")
            this.AddChambers(anticipatedChamber, GameSystem.instance.treasureChamberPrefab);
    }


    //public void AddStartingChamber(Chamber startingChamber, Agent newAgent)
    //{
    //    RectangularChamber startingChamber = Instantiate(GameSystem.instance.safeHouseChamberPrefab)
    //}
    private void AddChambers(Chamber anticipatedChamber, RectangularChamber chamberPrefab, int count = 1)
    {
        foreach (int C in Enumerable.Range(0, count + 1))
        {
            RectangularChamber newNeighborChamber = Instantiate(chamberPrefab);
            this.DetermineRectangularChamberSize(newNeighborChamber);
            this.allNeighborChambers.Add(newNeighborChamber);
            anticipatedChamber.neighborChambers.Add(newNeighborChamber);
            newNeighborChamber.neighborChambers.Add(anticipatedChamber);
        }

        anticipatedChamber.areNeighborsDescribed = true;
    }
    private void AddNextChambersToANumberOfPreviousChambers(int countOfPreviousChambers, RectangularChamber chamberPrefab, int count = 1)//adds a number of chambers to a number of previous chambers iterating from the end. then sets previous chambers as "neighbors described".
    {
        int indexOfLastPreviousChamber = this.allNeighborChambers.Count - 1;
        int indexOfFirstPreviousChamber = this.allNeighborChambers.Count - count - 1;
        for (int c = 0; c < count; c++)
        {
            RectangularChamber newNeighborChamber = Instantiate(chamberPrefab);
            this.DetermineRectangularChamberSize(newNeighborChamber);
            this.allNeighborChambers.Add(newNeighborChamber);
            for (int i = indexOfLastPreviousChamber; i <= indexOfFirstPreviousChamber; i--)
            {
                newNeighborChamber.neighborChambers.Add(this.allNeighborChambers[i]);
                this.allNeighborChambers[i].neighborChambers.Add(newNeighborChamber);
                this.allNeighborChambers[i].areNeighborsDescribed = true;
            }
        }
    }
    private void AddNextChambersToANumberOfPreviousChambersRandomly(int numberOfPreviousChambers, RectangularChamber chamberPrefab, int count = 1)//adds next chambers to a number of randomly chosen past chambers. i might need to make another function that adds a number of randomly selected next chambers to a number of randomly selected past chambers
    {
        var random = new Random();
        ArrayList indexesOfChosenObjects = ChooseRandomly(numberOfPreviousChambers, random.Next(1,numberOfPreviousChambers + 1));

        for (int c = 0; c < count; c++)
        {
            RectangularChamber newNeighborChamber = Instantiate(chamberPrefab);
            this.DetermineRectangularChamberSize(newNeighborChamber);
            this.allNeighborChambers.Add(newNeighborChamber);
            foreach (int previousChamberIndex in indexesOfChosenObjects)
            {
                this.allNeighborChambers[previousChamberIndex].neighborChambers.Add(newNeighborChamber);
                this.allNeighborChambers[previousChamberIndex].areNeighborsDescribed = true;
                newNeighborChamber.neighborChambers.Add(this.allNeighborChambers[previousChamberIndex]);
            }
        }
    }


    public void DetermineRectangularChamberSize(Chamber chamber)
    {
        Debug.LogError("NotImplemented");
    }
        public void DetermineRectangularChamberSize(RectangularChamber chamber)//TODO: implement a Better version Based on number of Objects inside Chamber.
    {
        if (chamber is BossChamber)
        {
            chamber.sizeX = random.Next(15, 26);
            chamber.sizeZ = random.Next(15, 26);
        }
        else if (chamber is NormalChamber)
        {
            chamber.sizeX = random.Next(5, 11);
            chamber.sizeZ = random.Next(5, 11);
        }
        else if (chamber is TreasureChamber)
        {
            chamber.sizeX = random.Next(4, 6);
            chamber.sizeZ = random.Next(4, 6);
        }
        else if (chamber is MercenaryCamp)
        {
            chamber.sizeX = random.Next(15, 21);
            chamber.sizeZ = random.Next(15, 21);
        }
        else if (chamber is SafeHouseChamber)
        {
            chamber.sizeX = random.Next(5, 7);
            chamber.sizeZ = random.Next(5, 7);
        }
        else if (chamber is TeleportationChamber)
        {
            chamber.sizeX = random.Next(9, 15);
            chamber.sizeZ = random.Next(9, 15);
        }
    }
    private ArrayList ChooseRandomly(int numberOfObjects, int numberOfChoices)//returns an array of indexes.
    {
        var random = new Random();
        ArrayList chosenObjectsIndexes = new ArrayList(numberOfChoices);

        for (int i = 0; i < numberOfChoices; i++)
        {
            int index;
            do
                index = random.Next(numberOfObjects);
            while (chosenObjectsIndexes.Contains(index));

            chosenObjectsIndexes.Add(index);
        }
        return chosenObjectsIndexes;
    }

    public void DescribeChamberContent(HumanoidAgent agent, Chamber newChamber)
    {
        //if (newChamber is SafeHouseChamber)
        //{
        //    newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.teleportationAltarPrefab).gameObject);
        //    newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.chestPrefab).gameObject);
        //}

        if (newChamber is TreasureChamber)
            newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.chestPrefab, newChamber.transform.position,Quaternion.identity,newChamber.transform).gameObject);

        else if (newChamber is BossChamber)
            newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.bossPrefab, newChamber.transform.position, Quaternion.identity, newChamber.transform).gameObject);

        else if (newChamber is NormalChamber)
        {
            for (int i = 0; i < random.Next(2, 6); i++)
            {
                newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.mobPrefab,newChamber.transform.position,Quaternion.identity, newChamber.transform).gameObject);
            }
        }

        else if (newChamber is MercenaryCamp)
        {
            newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.campPrefab,newChamber.transform.position, Quaternion.identity, newChamber.transform).gameObject);
            
            for (int i = 0; i < random.Next(4, 8); i++)
                newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.mobPrefab, newChamber.transform.position + new Vector3(0f,0f,-4f), Quaternion.identity, newChamber.transform).gameObject);

        }

        //else if (newChamber is TeleportationChamber)
        //    newChamber.chamberObjects.Add(Instantiate(GameSystem.instance.teleportationAltarPrefab).gameObject);

    }

    public void DescribeAllHallways()//this function is super simplified, in order to have different shapes of hallways i must define different prefabs for it and also a more capable generation method must be used.
    {
        foreach (RectangularChamber neighborChamber in this.allNeighborChambers)
        {
            foreach (RectangularChamber neighborOfNeighbor in neighborChamber.neighborChambers)
            {
                //this.SetEntrancesBetweenTwoChambers(neighborChamber, anticipatedChamber);
                if (neighborChamber.GetConnectingHallway(neighborOfNeighbor) == null)
                    this.AddHallwayBetweenChambers(neighborOfNeighbor,neighborChamber, GameSystem.instance.linearHallwayPrefab);
            }
        }
    }

    public void AddHallwayBetweenChambers(RectangularChamber neighborOfNeighbor, RectangularChamber neighborChamber, SimpleLinearHallway hallwayPrefab)
    {
        SimpleLinearHallway newHallway = Instantiate(hallwayPrefab);

        if (!neighborChamber.isGenerated && neighborOfNeighbor.isGenerated)
        {
            newHallway.SetFirstChamber(neighborOfNeighbor);
            newHallway.connectedChambers.Add(neighborChamber);
        }
        else 
        { 
            newHallway.SetFirstChamber(neighborChamber);
            newHallway.connectedChambers.Add(neighborOfNeighbor);
        }


        if (neighborOfNeighbor is BossChamber)
        {
            newHallway.length = random.Next(20, 26);
            this.allNewHallways.Add(newHallway);
        }
        else if (neighborOfNeighbor is MercenaryCamp)
        {
            newHallway.length = random.Next(15, 26);
            this.allNewHallways.Add(newHallway);
        }
        else
        {
            newHallway.length = random.Next(10, 21);
            this.allNewHallways.Add(newHallway);

        }


        neighborOfNeighbor.connectedHallways.Add(newHallway);
        neighborChamber.connectedHallways.Add(newHallway);
    }

    public void SetEntrancesBetweenTwoChambers(Chamber neighborChamber, Chamber anticipatedChamber)
    {

    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one StoryTeller!");
            return;
        }
        instance = this;

        this.allNeighborChambers = new List<Chamber>();
        this.neighborsStructureDifficulty = "";
        this.deadEnd = false;
    }
    

}
