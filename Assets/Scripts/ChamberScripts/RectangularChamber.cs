using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class RectangularChamber : Chamber
{
    public enum shapeDefinition {Empty,Block,EntranceBlock};

    public int sizeX;
    public int sizeZ;

    public List<List<shapeDefinition>> shapeMatrix;//used by map generator
    public List<List<ChamberBlock>> blocksMatrix;//literally this is all of chamber's data
    public List<ChamberBlock> entranceBlocks;//literally this is all of chamber's data

    public ChamberBlock blockPrefab;


    public GameObject wallPrefab;

    public GameObject floorPrefab;

    public Tuple<Tuple<int, int>, Hallway> exits;

    public List<Tuple<Item, Tuple<int, int>>> itemsInsideAndTheirPositionInChamber;

    public void InstantiateAllBlocks()
    {
        this.blocksMatrix = new List<List<ChamberBlock>>();
        for (int i = 0; i < this.sizeX; i++)
        {
            this.blocksMatrix.Add(new List<ChamberBlock>());
            for (int j = 0; j < this.sizeZ; j++)
            {
                ChamberBlock block = null;
                if (this.shapeMatrix[i][j] != shapeDefinition.Empty)
                {
                    block = Instantiate(this.blockPrefab, TranslateGivenIndexesToBlockPosition(i,j), Quaternion.identity, this.transform);
                    this.blocksMatrix[i].Add(block);
                }
            }
        }
        this.isGenerated = true;
        GameSystem.instance.allChambers.Add(this);
    }
    public Vector3 TranslateGivenIndexesToBlockPosition(int x, int z)
    {
        Vector3 firstBlockPosition =
        new Vector3(
                            this.transform.position.x - (float)this.sizeX / 2f * this.blockPrefab.actualSizeX + (float)this.blockPrefab.actualSizeX / 2f,
                            this.transform.position.y,
                            this.transform.position.z - (float)this.sizeZ / 2f * this.blockPrefab.actualSizeZ + (float)this.blockPrefab.actualSizeZ / 2f);

        Vector3 thisBlockPosition = firstBlockPosition + new Vector3((float)x * this.blockPrefab.actualSizeX, 0f, (float)z * this.blockPrefab.actualSizeZ);

        return thisBlockPosition;
    }

    public override void GenerateChamber()
    {
        StartCoroutine(this.BeginChamberGeneration());
    }

    public override IEnumerator BeginChamberGeneration()
    {
        yield return new WaitForFixedUpdate();
        this.UpdateChamberShapeMatrix();
    }

    public void InitializeShapeMatrixWithFloors()
    {
        this.shapeMatrix = new List<List<SafeHouseChamber.shapeDefinition>>();

        for (int i = 0; i < this.sizeX; i++)
        {
            this.shapeMatrix.Add(new List<shapeDefinition>());
            for (int j = 0; j < this.sizeZ; j++)
                this.shapeMatrix[i].Add(shapeDefinition.Block);
        }
    }

    public void UpdateChamberShapeMatrix()
    {
        foreach (SimpleLinearHallway hallway in this.connectedHallways)
        {
            if (hallway.lastBlock != null && hallway.lastBlock.ConnectedChamber == this && !hallway.lastBlock.allItems[1].Exists(item => item.GetComponent<Portal>() != null ))//this? or this.gameObject?
            {
                Tuple<int, int> indexes = GetAdjacentEntranceBlockIndexes(hallway.lastBlock, hallway.allBlocks.Last.Previous.Value);

                this.shapeMatrix[indexes.Item1][indexes.Item2] = shapeDefinition.EntranceBlock;
                this.blocksMatrix[indexes.Item1][indexes.Item2].connectedHallway = hallway;
                this.entranceBlocks.Add(this.blocksMatrix[indexes.Item1][indexes.Item2]);
            }
            else if (hallway.firstBlock != null && hallway.firstBlock.ConnectedChamber == this && !hallway.lastBlock.allItems[1].Exists(item => item.GetComponent<Portal>() != null))//this? or this.gameObject?
            {
                Tuple<int,int> indexes = GetAdjacentEntranceBlockIndexes(hallway.firstBlock, hallway.allBlocks.First.Next.Value);

                this.shapeMatrix[indexes.Item1][indexes.Item2] = shapeDefinition.EntranceBlock;
                this.blocksMatrix[indexes.Item1][indexes.Item2].connectedHallway = hallway;
                this.entranceBlocks.Add(this.blocksMatrix[indexes.Item1][indexes.Item2]);
            }
        }
    }

    public Tuple<int,int> GetAdjacentEntranceBlockIndexes(HallwayBlock endBlock, HallwayBlock previousBlock)
    {
        Vector3 transformToChamberEntranceBlock = endBlock.transform.position - previousBlock.transform.position;
        Vector3 chamberEntranceBlockPosition = endBlock.transform.position + transformToChamberEntranceBlock;

        Vector3 relationalEntranceBlockPosition = chamberEntranceBlockPosition - TranslateGivenIndexesToBlockPosition(0,0);
        relationalEntranceBlockPosition.x = (float)Math.Floor(relationalEntranceBlockPosition.x / this.blockPrefab.actualSizeX);
        relationalEntranceBlockPosition.z = (float)Math.Floor(relationalEntranceBlockPosition.z / this.blockPrefab.actualSizeZ);

        Tuple.Create(relationalEntranceBlockPosition.x, relationalEntranceBlockPosition.z);

        return Tuple.Create((int)relationalEntranceBlockPosition.x, (int)relationalEntranceBlockPosition.z);
    }

}
