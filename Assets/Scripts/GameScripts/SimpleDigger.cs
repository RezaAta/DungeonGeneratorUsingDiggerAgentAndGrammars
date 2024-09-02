using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using Object = System.Object;



//simple digger with simple pleasures
public class SimpleDigger : Block
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    public List<Direction> allPossibleDirections = new List<Direction> {Direction.Up, Direction.Down, Direction.Left, Direction.Right };
    public List<(Direction,float)> allDirectionsAndProbabilities = new List<(Direction, float)>();
    public List<Direction> setOfDirectionsTaken = new List<Direction>();
    public Direction chosenDirection;
    public Direction preferredDirection;
    public float preferencePercentage;

    public HallwayBlock hallwayBlockPrefab;
    public ChamberBlock chamberBlockPrefab;

    public RectangularChamber startingChamber;
    public RectangularChamber endChamber;

    public override void Initialize()
    {

    }

    public void DigHallwayBetweenExistingChambers(Chamber lastBlockConnectedChamber, Chamber firstBlockConnectedChamber, SimpleLinearHallway newHallway)
    {
        throw new NotImplementedException();
    }
    public void DigHallwayBetweenExistingChambers(RectangularChamber endChamber, RectangularChamber startingChamber, SimpleLinearHallway newHallway)
    {
        //todo: implement some path finding method like A*
    }

    public void StartDigging(Chamber firstBlockConnectedChamber, Chamber lastBlockConnectedChamber,
        SimpleLinearHallway newHallway, Direction preferredDirection, float preferencePercentage = 5)
    {
        throw new NotImplementedException();
    }

    public void StartDigging(RectangularChamber startingChamber, List<Chamber> endChamber,
        SimpleLinearHallway newHallway, Direction preferredDirection, float preferencePercentage = 5)
    {

    }
    public void StartDigging(RectangularChamber startingChamber, RectangularChamber endChamber,
        SimpleLinearHallway newHallway, Direction preferredDirection, float preferencePercentage = 5)
    {
        
        this.startingChamber = startingChamber;
        this.endChamber = endChamber;
        

        this.chamberBlockPrefab = endChamber.blockPrefab;
        this.hallwayBlockPrefab = newHallway.hallwayBlockPrefab;

        this.floorPrefab = newHallway.hallwayBlockPrefab.floorPrefab;
        this.wallPrefab = newHallway.hallwayBlockPrefab.wallPrefab;
        this.actualSizeX = floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        this.actualSizeZ = floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;

        this.preferredDirection = preferredDirection;
        this.preferencePercentage = preferencePercentage;
        this.allPossibleDirections = new List<Direction>
            {Direction.Up, Direction.Down, Direction.Left, Direction.Right};
        if (this.preferredDirection == Direction.Left)
            this.allPossibleDirections.Remove(Direction.Right);
        
        else if (this.preferredDirection == Direction.Right)
            this.allPossibleDirections.Remove(Direction.Left);
        
        else if (this.preferredDirection == Direction.Up)
            this.allPossibleDirections.Remove(Direction.Down);
        
        else if (this.preferredDirection == Direction.Down)
            this.allPossibleDirections.Remove(Direction.Up);

        allDirectionsAndProbabilities = new List<(Direction, float)>();
        foreach (Direction possibleDirection in this.allPossibleDirections)
        {
            if (possibleDirection == this.preferredDirection)
                this.allDirectionsAndProbabilities.Add((possibleDirection,this.preferencePercentage));
            else
                this.allDirectionsAndProbabilities.Add((possibleDirection,this.preferencePercentage /2));
        }

        this.SetBeginningCoordinatesBasedOnProbability(newHallway);

        this.DigHallway(newHallway);

        newHallway.GenerateHallway();

        PlaceChamberInPossibleCoordinates(endChamber,newHallway);

        foreach (List<ChamberBlock> blockRow in startingChamber.blocksMatrix)
            foreach (ChamberBlock block in blockRow)
                block.GenerateWalls();
        foreach (List<ChamberBlock> blockRow in endChamber.blocksMatrix)
            foreach (ChamberBlock block in blockRow)
                block.GenerateWalls();
        foreach (HallwayBlock block in newHallway.allBlocks)
                block.GenerateWalls();
    }
    private void SetBeginningCoordinatesBasedOnProbability(Hallway newHallway)
    {
        startingChamber.UpdateChamberShapeMatrix();

        List<Direction> possibleStartingDirections = new List<Direction>(this.allPossibleDirections);

        List<Tuple<ChamberBlock, float, Direction>> probabilityList = new List<Tuple<ChamberBlock, float, Direction>>();

        foreach (Direction direction in possibleStartingDirections)
        {
            if (direction == Direction.Up)
            {
                for (int i = 0; i < startingChamber.sizeX; i++)
                    if (!probabilityList.Contains(Tuple.Create(startingChamber.blocksMatrix[i].Last(), 0f, Direction.Up)))
                        probabilityList.Add(Tuple.Create(startingChamber.blocksMatrix[i].Last(), 0f, Direction.Up));
            }

            else if (direction == Direction.Right)
            {
                for (int j = startingChamber.sizeZ - 1; j >= 0; j--)
                    if (!probabilityList.Contains(Tuple.Create(startingChamber.blocksMatrix.Last()[j], 0f, Direction.Right)))
                        probabilityList.Add(Tuple.Create(startingChamber.blocksMatrix.Last()[j], 0f, Direction.Right));
            }

            else if (direction == Direction.Down)
            {
                for (int i = startingChamber.sizeX - 1; i >= 0; i--)
                    if (!probabilityList.Contains(Tuple.Create(startingChamber.blocksMatrix[i].First(), 0f, Direction.Down)))
                        probabilityList.Add(Tuple.Create(startingChamber.blocksMatrix[i].First(), 0f, Direction.Down));
            }

            else if (direction == Direction.Left)
            {
                for (int j = 0; j < startingChamber.sizeZ; j++)
                    if (!probabilityList.Contains(Tuple.Create(startingChamber.blocksMatrix.First()[j], 0f, Direction.Left)))
                        probabilityList.Add(Tuple.Create(startingChamber.blocksMatrix.First()[j], 0f, Direction.Left));
            }
        }

        for (int i = 0; i < probabilityList.Count; i++)
        {
            if (probabilityList[i].Item1.connectedHallway!=null)
            {
                probabilityList.RemoveAt(i);
                int c = 0;
                while (i > 0)
                {
                    probabilityList.RemoveAt(--i);
                    c++;
                    if (c == 2)
                        break;
                }
                c = 0;
                while (i < probabilityList.Count())
                {
                    probabilityList.RemoveAt(i);
                    c++;
                    if (c == 2)
                        break;
                }
            }
        }

        float baseProbability = 100f / probabilityList.Count();

        for (int i = 0; i < probabilityList.Count; i++)
            probabilityList[i] = Tuple.Create(probabilityList[i].Item1,baseProbability, probabilityList[i].Item3) ;

        float outcome = UnityEngine.Random.Range(0f, 100f);

        for (int i = 0; i < probabilityList.Count; i++)// i used a little trick to simulate distribution function.
        {
            if (outcome < probabilityList[i].Item2)
            {
                probabilityList[i].Item1.connectedHallway = newHallway;
                if (probabilityList[i].Item3 == Direction.Up)
                {
                    this.transform.position = new Vector3(probabilityList[i].Item1.transform.position.x, 0f, probabilityList[i].Item1.transform.position.z + this.startingChamber.blockPrefab.actualSizeZ / 2f + this.actualSizeZ / 2f);
                    this.setOfDirectionsTaken.Add(Direction.Up);
                }
                else if (probabilityList[i].Item3 == Direction.Right)
                {
                    this.transform.position = new Vector3(probabilityList[i].Item1.transform.position.x + this.startingChamber.blockPrefab.actualSizeX / 2f + this.actualSizeX / 2f, 0f, probabilityList[i].Item1.transform.position.z);
                    this.setOfDirectionsTaken.Add(Direction.Right);
                }
                else if (probabilityList[i].Item3 == Direction.Down)
                {
                    this.transform.position = new Vector3(probabilityList[i].Item1.transform.position.x, 0f, probabilityList[i].Item1.transform.position.z - this.startingChamber.blockPrefab.actualSizeZ / 2f - this.actualSizeZ / 2f);
                    this.setOfDirectionsTaken.Add(Direction.Down);
                }
                else if (probabilityList[i].Item3 == Direction.Left)
                {
                    this.transform.position = new Vector3(probabilityList[i].Item1.transform.position.x - this.startingChamber.blockPrefab.actualSizeX / 2f - this.actualSizeX / 2f, 0f, probabilityList[i].Item1.transform.position.z);
                    this.setOfDirectionsTaken.Add(Direction.Left);
                }
                break;
            }
            outcome = outcome - probabilityList[i].Item2;
        }
    }
    private void SetBeginningCoordinates()
    {
        List<(float, float)> allPossibleStartingCoordinates;
        startingChamber.UpdateChamberShapeMatrix();

        List<Direction> possibleStartingDirections = new List<Direction>(this.allPossibleDirections);

        foreach (var entranceBlock in startingChamber.entranceBlocks)
        {

            //if (!entranceBlock.allItems.Exists(item => item.GetComponent<Portal>() != null))
            //{
                Vector3 relationalPosition = entranceBlock.transform.position - this.startingChamber.blocksMatrix[0][0].gameObject.transform.position;

                relationalPosition.x = (int)relationalPosition.x / this.startingChamber.blockPrefab.actualSizeX;
                relationalPosition.z = (int)relationalPosition.z / this.startingChamber.blockPrefab.actualSizeZ;

                //if (relationalPosition.x != relationalPosition.z)
                //{

                    if (relationalPosition.x == startingChamber.sizeX - 1)
                        possibleStartingDirections.Remove(Direction.Right);

                    if (relationalPosition.x == 0)
                        possibleStartingDirections.Remove(Direction.Left);

                    if (relationalPosition.z == startingChamber.sizeZ - 1)
                        possibleStartingDirections.Remove(Direction.Up);

                    if (relationalPosition.z == 0)
                        possibleStartingDirections.Remove(Direction.Down);
                //}

            //}
        }


        if (possibleStartingDirections.Count == 0)
            possibleStartingDirections.Add(this.preferredDirection);
        else
        {
            int chosenIndex = UnityEngine.Random.Range(0, possibleStartingDirections.Count-1);
            Direction d = possibleStartingDirections[chosenIndex];
            possibleStartingDirections = new List<Direction> { d };
            this.setOfDirectionsTaken.Add(d);
        }

        allPossibleStartingCoordinates = FindAllPossibleCoordinates(possibleStartingDirections);

        int r = UnityEngine.Random.Range(0, allPossibleStartingCoordinates.Count);
        this.transform.position = new Vector3(allPossibleStartingCoordinates[r].Item1, 0, allPossibleStartingCoordinates[r].Item2);
    }
    //this function fills all possible starting blocks with empty non-entrance blocks based on preferred direction
    private List<(float, float)> FindAllPossibleCoordinates(List<Direction> possibleStartingDirections)
    {
        List<(float, float)> allPossibleStartingCoordinates = new List<(float, float)>();


        foreach (Direction possibleDirection in possibleStartingDirections)
        {
            switch (possibleDirection)
            {
                case Direction.Left:
                    {
                        for (int j = 0; j < this.startingChamber.sizeZ; j++)
                        {
                            if (startingChamber.blocksMatrix.First()[j].allItems.Any()) continue;
                            if (startingChamber.blocksMatrix.First()[j].connectedHallway != null) continue;
                            if (j == 0)
                            {
                                if (startingChamber.blocksMatrix.First()[j + 1].connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix.First()[j].transform.position.x - this.actualSizeX,
                                            startingChamber.blocksMatrix.First()[j].transform.position.z));
                            }
                            else if (j == startingChamber.sizeZ - 1)
                            {
                                if (startingChamber.blocksMatrix.First()[j - 1].connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix.First()[j].transform.position.x - this.actualSizeX,
                                            startingChamber.blocksMatrix.First()[j].transform.position.z));
                            }
                            else if (startingChamber.blocksMatrix.First()[j - 1].connectedHallway == null &&
                                     startingChamber.blocksMatrix.First()[j + 1].connectedHallway == null)
                            {
                                allPossibleStartingCoordinates.Add(
                                    (startingChamber.blocksMatrix.First()[j].transform.position.x - this.actualSizeX,
                                        startingChamber.blocksMatrix.First()[j].transform.position.z));
                            }
                        }

                        break;
                    }
                case Direction.Right:
                    {

                        for (int j = 0; j < this.startingChamber.sizeZ; j++)
                        {

                            if (startingChamber.blocksMatrix.Last()[j].allItems.Any()) continue;
                            if (startingChamber.blocksMatrix.Last()[j].connectedHallway != null) continue;

                            if (j == 0)
                            {
                                if (startingChamber.blocksMatrix.Last()[j + 1].connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix.Last()[j].transform.position.x + this.actualSizeX,
                                            startingChamber.blocksMatrix.Last()[j].transform.position.z));
                            }
                            else if (j == startingChamber.sizeZ - 1)
                            {
                                if (startingChamber.blocksMatrix.Last()[j - 1].connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix.Last()[j].transform.position.x + this.actualSizeX,
                                            startingChamber.blocksMatrix.Last()[j].transform.position.z));
                            }
                            else if (startingChamber.blocksMatrix.Last()[j - 1].connectedHallway == null &&
                                     startingChamber.blocksMatrix.Last()[j + 1].connectedHallway == null)
                            {
                                allPossibleStartingCoordinates.Add(
                                    (startingChamber.blocksMatrix.Last()[j].transform.position.x + this.actualSizeX,
                                        startingChamber.blocksMatrix.Last()[j].transform.position.z));
                            }
                        }

                        break;
                    }
                case Direction.Down:
                    {
                        for (int i = 0; i < this.startingChamber.sizeX; i++)
                        {
                            if (startingChamber.blocksMatrix[i].First().allItems.Any()) continue;
                            if (startingChamber.blocksMatrix[i].First().connectedHallway != null) continue;
                            if (i == 0)
                            {
                                if (startingChamber.blocksMatrix[i + 1].First().connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix[i].First().transform.position.x,
                                            startingChamber.blocksMatrix[i].First().transform.position.z - this.actualSizeZ));
                            }
                            else if (i == startingChamber.sizeX - 1)
                            {
                                if (startingChamber.blocksMatrix[i - 1].First().connectedHallway == null)
                                    allPossibleStartingCoordinates.Add(
                                        (startingChamber.blocksMatrix[i].First().transform.position.x,
                                            startingChamber.blocksMatrix[i].First().transform.position.z - this.actualSizeZ));
                            }
                            else if (startingChamber.blocksMatrix[i + 1].First().connectedHallway == null &&
                                     startingChamber.blocksMatrix[i - 1].First().connectedHallway == null)
                            {
                                allPossibleStartingCoordinates.Add(
                                    (startingChamber.blocksMatrix[i].First().transform.position.x,
                                        startingChamber.blocksMatrix[i].First().transform.position.z - this.actualSizeZ));
                            }
                        }

                        break;
                    }
                case Direction.Up:
                    {
                        for (int i = 0; i < this.startingChamber.sizeX; i++)
                        {

                            if (startingChamber.blocksMatrix[i].Last().allItems.Any()) continue;
                            if (startingChamber.blocksMatrix[i].Last().connectedHallway != null) continue;

                            if (i == 0)
                            {
                                if (startingChamber.blocksMatrix[i + 1].Last().connectedHallway == null)
                                    allPossibleStartingCoordinates.Add((startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.x,
                                        startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.z + this.actualSizeZ));
                            }
                            else if (i == startingChamber.sizeX - 1)
                            {
                                if (startingChamber.blocksMatrix[i - 1].Last().connectedHallway == null)
                                    allPossibleStartingCoordinates.Add((startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.x,
                                        startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.z + this.actualSizeZ));
                            }
                            else if (startingChamber.blocksMatrix[i - 1].Last().connectedHallway == null &&
                                     startingChamber.blocksMatrix[i + 1].Last().connectedHallway == null)
                            {
                                allPossibleStartingCoordinates.Add(
                                    (startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.x,
                                        startingChamber.blocksMatrix[i][startingChamber.sizeZ - 1].transform.position.z + this.actualSizeZ));
                            }
                        }

                        break;
                    }
            }
        }

        return allPossibleStartingCoordinates;
    }

    private void DigHallway(SimpleLinearHallway newHallway)
    {

        HallwayBlock firstBlock = Instantiate
            (hallwayBlockPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity, newHallway.transform);
        firstBlock.ConnectedChamber = this.startingChamber;
        newHallway.firstBlock = firstBlock;

        newHallway.allBlocks.AddLast(firstBlock);

        if (this.allDirectionsAndProbabilities.Count != 0)
        {
            for (int i = 1; i < newHallway.length; i++)// we start from one because there is already one block in the hallway
            {
                this.TakeStep(newHallway);
                if (this.allDirectionsAndProbabilities.Count == 0)
                {
                    newHallway.length = i + 1;
                    break;
                }

                HallwayBlock newBlock = Instantiate
                    (hallwayBlockPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity, newHallway.transform);

                newHallway.allBlocks.AddLast(newBlock);
            }
        }
        if (newHallway.length <= 2)//rebuild the hallway if it was too short
        {
            foreach (var item in newHallway.allBlocks)
                Destroy(item.gameObject);

            newHallway.allBlocks = new LinkedList<HallwayBlock>();

            SetBeginningCoordinatesBasedOnProbability(newHallway);

            this.DigHallway(newHallway);
        }
        newHallway.lastBlock = newHallway.allBlocks.Last();
        newHallway.lastBlock.ConnectedChamber = this.endChamber;
    }

    public void TakeStep(SimpleLinearHallway newHallway)
    {   
        this.UpdateAllPossibleDirections(newHallway);
        this.UpdateListOfDirectionsAndProbablities();
        
        if (this.allDirectionsAndProbabilities.Count != 0) 
        { 
            Direction newDirection = this.ChooseDirectionRandomly();

            bool DirectionChanged = (this.chosenDirection != newDirection);


            //TODO: implement and debug removal of the previously selected direction
            if (DirectionChanged) //resetting the probability distribution.
            {
                this.allPossibleDirections.Remove(this.chosenDirection);
                this.preferencePercentage = 5;
            }
            else
                this.preferencePercentage = this.preferencePercentage * 1.5f;


            this.chosenDirection = newDirection;

            if (this.chosenDirection == Direction.Down)
                this.transform.position = 
                    new Vector3(this.transform.position.x, this.transform.position.y , this.transform.position.z - this.actualSizeZ);
        
            else if (this.chosenDirection == Direction.Up)
                this.transform.position = 
                    new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + this.actualSizeZ);
        
            else if (this.chosenDirection == Direction.Left)
                this.transform.position = 
                    new Vector3(this.transform.position.x - this.actualSizeX, this.transform.position.y, this.transform.position.z);
        
            else if (this.chosenDirection == Direction.Right)
                this.transform.position = 
                    new Vector3(this.transform.position.x + this.actualSizeX, this.transform.position.y, this.transform.position.z);
        
            this.setOfDirectionsTaken.Add(newDirection);
        }
    }

    private void UpdateAllPossibleDirections(SimpleLinearHallway newHallway)
    {
        this.allPossibleDirections = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        this.allPossibleDirections.Remove(GetOppositeDirection(this.preferredDirection));
        
        this.allPossibleDirections.Remove(GetOppositeDirection(this.chosenDirection));

        if (this.setOfDirectionsTaken.Any())
        {
            foreach (Direction takenDirection in this.setOfDirectionsTaken)
            {
                this.allPossibleDirections.Remove(takenDirection);
                this.allPossibleDirections.Remove(GetOppositeDirection(takenDirection));
            }

            this.allPossibleDirections.Add(this.setOfDirectionsTaken.Last());
        }

        //checking for collisions in a 5*5 square area around the agent. 
        //i reduced the half extents by a small fraction because i dont want to get colliders that are only touching the overlap box
        Collider[] overlappingColliders = Physics.OverlapBox(this.gameObject.transform.position, new Vector3(this.actualSizeX * 1.5f, 2, this.actualSizeZ * 1.5f));

        if (overlappingColliders.Length != 0)
            foreach (Collider overlappingCollider in overlappingColliders)
            {
                if (overlappingCollider.gameObject == this.gameObject)
                    continue;
                else if (overlappingCollider.gameObject.transform.parent != null && overlappingCollider.gameObject.transform.parent.gameObject == newHallway.gameObject)
                    continue;
                else if ((overlappingCollider.gameObject.transform.position - this.transform.position).magnitude <=
                         this.actualSizeX) //this checks for adjacent objects
                {
                    Direction directionOfAdjacentObject = WhichDirectionIsThisObject(this.gameObject, overlappingCollider.gameObject);

                    this.allPossibleDirections = new List<Direction>() { this.GetOppositeDirection(directionOfAdjacentObject) };
                }
                else
                    this.allPossibleDirections.Remove(WhichDirectionIsThisObject(this.gameObject, overlappingCollider.gameObject));

            }

    }

    private Direction GetOppositeDirection(Direction d)
    {
        switch (d)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            default:
                Debug.LogError("OppositeDirection Not Defined.");
                return d;
        }
    }

    private void UpdateListOfDirectionsAndProbablities()
    {
        this.allDirectionsAndProbabilities = new List<(Direction, float)>();
        foreach (Direction possibleDirection in this.allPossibleDirections)
        {
            if (possibleDirection == this.preferredDirection)
                this.allDirectionsAndProbabilities.Add((possibleDirection, this.preferencePercentage));
            else
                this.allDirectionsAndProbabilities.Add((possibleDirection, this.preferencePercentage / 2));
        }
    }

    //Gets the Direction of Second Object Based on First Object's Location.
    private Direction WhichDirectionIsThisObject(GameObject firstObject, GameObject secondObject)
    {
        Vector3 transitionVector = new Vector3(
            secondObject.transform.position.x - firstObject.transform.position.x,
            secondObject.transform.position.y - firstObject.transform.position.y,
            secondObject.transform.position.z - firstObject.transform.position.z);

        float max = Math.Max(Math.Abs(transitionVector.x), Math.Abs(Math.Max(Math.Abs(transitionVector.y), Math.Abs(transitionVector.z))));

        if (Math.Abs(max - Math.Abs(transitionVector.x)) < 0.0001)
        {
            if (transitionVector.x >= 0)
                return Direction.Right;
            else
                return Direction.Left;
        }
        if (Math.Abs(max - Math.Abs(transitionVector.z)) < 0.0001)
        {
            if (transitionVector.z >= 0)
                return Direction.Up;
            else
                return Direction.Down;
        }
        //Function not implemented for 3d space. i just return left in the end because preferred direction i always have in mind is right.
        return Direction.Left;
    }

    private Direction ChooseDirectionRandomly()
    {
        Direction newDirection;

        if (this.allPossibleDirections.Count == 1)
            newDirection = allPossibleDirections[0];

        float outcome = UnityEngine.Random.Range(0f, 101f);
        bool newDirectionGotChosen = false;

        for (int i = 0; i < this.allDirectionsAndProbabilities.Count; i++)// i used a little trick to simulate distribution function.
        {
            if (outcome < this.allDirectionsAndProbabilities[i].Item2)
            {
                newDirection = allPossibleDirections[i];
                newDirectionGotChosen = true;
                return newDirection;
            }

            outcome = outcome - this.allDirectionsAndProbabilities[i].Item2;
        }
        
        if (newDirectionGotChosen == false && this.allPossibleDirections.Contains(this.chosenDirection) == false)
        {
            int i = UnityEngine.Random.Range(0, this.allPossibleDirections.Count);
            newDirection = this.allPossibleDirections[i];
        }
        else
            newDirection = this.chosenDirection;

        return newDirection;
    }
    //TODO:Implement BackTracking
    public void PlaceChamberInPossibleCoordinatesWithBackTracking(RectangularChamber chamber, SimpleLinearHallway newHallway)
    {
        while (true)
        {
            Vector3 chamberCoordinates = this.GenerateChamberCoordinates(chamber);
            bool thereIsCollision = this.CheckNewChamberCollision(chamberCoordinates, chamber);

            chamber.transform.position = chamberCoordinates;

            if (thereIsCollision == false)
            {
                chamber.InitializeShapeMatrixWithFloors();
                chamber.GenerateChamber();
                chamber.isGenerated = true;
            }
            else
            {
                this.SkipWithSteps(5);
                continue;
            }

            break;
        }
    }
    public void PlaceChamberInPossibleCoordinates(RectangularChamber chamber,SimpleLinearHallway newHallway)
    {
        Tuple<int, int> portalEntranceBlockIndexes = null;
        while (true)
        {
            Vector3 chamberCoordinates = this.GenerateChamberCoordinates(chamber);
            bool thereIsCollision = this.CheckNewChamberCollision(chamberCoordinates, chamber);
            
            chamber.transform.position = chamberCoordinates;

            if (thereIsCollision == false)
            {
                chamber.InitializeShapeMatrixWithFloors();
                chamber.InstantiateAllBlocks();
                chamber.GenerateChamber();
                chamber.isGenerated = true;

                //TODO: implement portals and Connect them. PROGRESS: Primitive Implementation Almost Done
                if (newHallway.lastBlock.allItems[1].Exists(item => item.GetComponent<Portal>() != null))//if last block had a portal, then place a portal in chamberEntrance
                {
                    if (!chamber.blocksMatrix[portalEntranceBlockIndexes.Item1][portalEntranceBlockIndexes.Item2].allItems[1].Exists(item => item.GetComponent<Portal>() != null))
                    {
                        Portal firstPortal = newHallway.lastBlock.allItems[1].Find(item => item.GetComponent<Portal>() != null).GetComponent<Portal>();
                        Portal secondPortal = Instantiate(GameSystem.instance.portalPrefab, chamber.blocksMatrix[portalEntranceBlockIndexes.Item1][portalEntranceBlockIndexes.Item2].transform.position + new Vector3(0f, 0.3f, 0f), GameSystem.instance.portalPrefab.transform.rotation, chamber.transform);
                        secondPortal.otherPortal = firstPortal;
                        firstPortal.otherPortal = secondPortal;
                        firstPortal.GenerateNavLinkTo(secondPortal);
                        chamber.blocksMatrix[portalEntranceBlockIndexes.Item1][portalEntranceBlockIndexes.Item2].allItems[1].Add(secondPortal.gameObject);
                    }
                }
            }
            else
            {
                if (!newHallway.lastBlock.allItems[1].Exists(item => item.GetComponent<Portal>() != null))// if last block didnt already have a portal, then place a portal
                {   
                    newHallway.lastBlock.allItems[1].Add(Instantiate(GameSystem.instance.portalPrefab, newHallway.lastBlock.transform.position + new Vector3(0f, 0.3f, 0f), GameSystem.instance.portalPrefab.transform.rotation, newHallway.transform).gameObject);
                    portalEntranceBlockIndexes = chamber.GetAdjacentEntranceBlockIndexes(newHallway.lastBlock, newHallway.allBlocks.Last.Previous.Value);
                }
                this.SkipWithSteps(5);
                continue;
            }

            break;
        }
    }

    public bool CheckNewChamberCollision(Vector3 chamberCoordinates,RectangularChamber chamber)
    {
        Collider[] overlappingColliders = Physics.OverlapBox(chamberCoordinates, new Vector3((chamber.sizeX + 2) * this.chamberBlockPrefab.actualSizeX / 2, 0, (chamber.sizeZ + 2) * this.chamberBlockPrefab.actualSizeZ / 2));

        foreach (Collider overlappingCollider in overlappingColliders)
        {
            if (overlappingCollider.gameObject.GetComponent<Block>() != null)
            {
                if (overlappingCollider.transform.parent.gameObject.GetComponent<Chamber>() != null)
                {
                    if (overlappingCollider.transform.parent.position == chamberCoordinates)
                        continue;
                }
                else if (overlappingCollider.transform.parent.gameObject.GetComponent<Hallway>() != null)
                {
                    if (overlappingCollider.transform.parent.gameObject.GetComponent<Hallway>().connectedChambers.Last() == endChamber)
                            continue;
                }
            }
            return true;
        }
        return false;
        
    }
    public Vector3 GenerateChamberCoordinates(RectangularChamber chamber)
    {
        Vector3 chamberCoordinates = new Vector3(0, 0, 0);

        Vector3 transformAgentToChamberCenter = new Vector3(0,0,0);//just an initialization.

        if (this.setOfDirectionsTaken.Last() == Direction.Down)
        {
            if (chamber.sizeX % 2 == 0)
                transformAgentToChamberCenter.x = transformAgentToChamberCenter.x + (float)this.actualSizeX / 2;
            transformAgentToChamberCenter.z = transformAgentToChamberCenter.z - (float)this.actualSizeZ / 2;
            transformAgentToChamberCenter.z = transformAgentToChamberCenter.z - (float) chamber.sizeZ * chamber.blockPrefab.actualSizeZ / 2 ;
        }
        if (this.setOfDirectionsTaken.Last() == Direction.Up)
        {
            if (chamber.sizeX % 2 == 0)
                transformAgentToChamberCenter.x = transformAgentToChamberCenter.x + (float)this.actualSizeX / 2;
            transformAgentToChamberCenter.z = transformAgentToChamberCenter.z + (float)this.actualSizeZ / 2;
            transformAgentToChamberCenter.z = transformAgentToChamberCenter.z + (float)chamber.sizeZ * chamber.blockPrefab.actualSizeZ / 2;
        }
        if (this.setOfDirectionsTaken.Last() == Direction.Left)
        {
            if (chamber.sizeZ % 2 == 0)
                transformAgentToChamberCenter.z = transformAgentToChamberCenter.z - (float)this.actualSizeZ / 2;
            transformAgentToChamberCenter.x = transformAgentToChamberCenter.x - (float)this.actualSizeX / 2;
            transformAgentToChamberCenter.x = transformAgentToChamberCenter.x - (float)chamber.sizeX * chamber.blockPrefab.actualSizeX / 2;
        }
        if (this.setOfDirectionsTaken.Last() == Direction.Right)
        {
            if (chamber.sizeZ%2 == 0)
                transformAgentToChamberCenter.z = transformAgentToChamberCenter.z - (float)this.actualSizeZ / 2;
            transformAgentToChamberCenter.x = transformAgentToChamberCenter.x + (float)this.actualSizeX / 2;
            transformAgentToChamberCenter.x = transformAgentToChamberCenter.x + (float)chamber.sizeX * chamber.blockPrefab.actualSizeX / 2;
        }

        chamberCoordinates = this.transform.position + transformAgentToChamberCenter;

        return chamberCoordinates;

    }
    private void SkipWithSteps(int steps)
    {
        if (this.setOfDirectionsTaken.Last() == Direction.Down)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - steps * this.actualSizeZ);
        else if (this.setOfDirectionsTaken.Last() == Direction.Up)
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + steps * this.actualSizeZ);
        else if (this.setOfDirectionsTaken.Last() == Direction.Left)
            this.transform.position = new Vector3(this.transform.position.x - steps * this.actualSizeX, this.transform.position.y ,this.transform.position.z);
        else if (this.setOfDirectionsTaken.Last() == Direction.Right)
            this.transform.position = new Vector3(this.transform.position.x + steps * this.actualSizeX, this.transform.position.y ,this.transform.position.z);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
