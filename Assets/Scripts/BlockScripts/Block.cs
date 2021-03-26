using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Block : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public List<List<GameObject>> allItems;//row 0 for static geometry, row 1 for static gameobjects

    public float actualSizeX;
    public float actualSizeY;
    public float actualSizeZ;

    
    //public MeshFilter meshFilter;

    public virtual void Initialize()
    {
        this.allItems = new List<List<GameObject>>();
        this.allItems.Add(new List<GameObject>());
        this.allItems.Add(new List<GameObject>());

        this.actualSizeX = floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        this.actualSizeZ = floorPrefab.GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
        this.actualSizeY = Mathf.Min(actualSizeX, actualSizeZ) * 2;

        if (this.GetComponent<BoxCollider>() == null)
        {
            this.gameObject.AddComponent<BoxCollider>();
            this.gameObject.GetComponent<BoxCollider>().size = new Vector3(actualSizeX, actualSizeY, actualSizeZ);
            this.gameObject.GetComponent<BoxCollider>().center = this.gameObject.GetComponent<BoxCollider>().center + new Vector3(0f, (float)this.actualSizeY/2f, 0f);
            this.gameObject.GetComponent<BoxCollider>().transform.parent = this.transform;
        }
    }

    public void GenerateWalls()
    {
        if (this.allItems[0].Any())
            foreach (GameObject item in this.allItems[0])
                Destroy(item);

        this.allItems[0] = new List<GameObject>();


        if (!Physics.Raycast(this.transform.position, new Vector3(0f, 0f, 1f), this.actualSizeZ))
            this.allItems[0].Add(Instantiate(wallPrefab, this.transform.position + new Vector3(0f, 0f, actualSizeZ / 2f), Quaternion.Euler(0f, 0f, 0f), this.transform));

        if (!Physics.Raycast(this.transform.position, new Vector3(1f, 0f, 0f), this.actualSizeX))
            this.allItems[0].Add(Instantiate(wallPrefab, this.transform.position + new Vector3(actualSizeX / 2f, 0f, 0f), Quaternion.Euler(0f,90f,0f), this.transform));

        if (!Physics.Raycast(this.transform.position, new Vector3(0f, 0f, -1f), this.actualSizeZ))
            this.allItems[0].Add(Instantiate(wallPrefab, this.transform.position + new Vector3(0f, 0f, - actualSizeZ / 2f), Quaternion.Euler(0f, 180f, 0f), this.transform));

        if (!Physics.Raycast(this.transform.position, new Vector3(-1f, 0f, 0f), this.actualSizeX))
            this.allItems[0].Add(Instantiate(wallPrefab, this.transform.position + new Vector3( - actualSizeX / 2f, 0f, 0f), Quaternion.Euler(0f, 270f, 0f), this.transform));

    }

    void Awake()
    {
        this.Initialize();
    }

}
