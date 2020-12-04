using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using PointerType = UnityEngine.PointerType;
using Random = UnityEngine.Random;

public class CityGen : MonoBehaviour
{
    /*--------------------------------------------
     the optimal chunks size is 10 - 20 - 40
     to have diverse blocks
    --------------------------------------------*/
    [SerializeField] private int width, length;
    public GameObject building_Holder;
    public int boarder;

    public Vector3[,] mapData; // init data

    private List<Vector3[,]> streetBlocks = new List<Vector3[,]>();
    private List<Vector3> buildingLocations = new List<Vector3>();
    private List<Vector3> removeObject = new List<Vector3>();

    private List<GameObject> Buildings = new List<GameObject>();

    private int sliceAmount = 5;
    public float tileSpacing = 2;

    public List<GameObject> buildingsList;
    public GameObject road;

    private Mesh ground;
    private void Awake() => ground = GetComponent<MeshFilter>().mesh;

    // Start is called before the first frame update
    void Start() => StartCoroutine(CreateCityScape());

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator CreateCityScape()
    {
        GenArray();
        streetBlocks.AddRange(CreateSpacePartitioning(mapData));
        BuildMap();
        yield return null;
    }

    List<Vector3[,]> CreateSpacePartitioning(Vector3[,] data)
    {
        List<Vector3[,]> aList = new List<Vector3[,]>();
        List<Vector3[,]> bList = new List<Vector3[,]>();

        List<Vector3[,]> splitList = new List<Vector3[,]>();

        List<Vector3[,]> areas = new List<Vector3[,]>();


        //init cut in half

        splitList.AddRange(SplitDesicion(data));
        aList.Add(splitList[0]);
        bList.Add(splitList[1]);

        for (int i = 0; i < sliceAmount; i++)
        {
            splitList.Clear();
            var listHolder = SplitDesicion(aList[i]);
            if (listHolder != null) splitList.AddRange(listHolder);
            else break;
            aList.Add(splitList[0]);
            aList.Add(splitList[1]);
        }

        for (int i = 0; i < sliceAmount; i++)
        {
            splitList.Clear();
            var listHolder = SplitDesicion(bList[i]);
            if (listHolder != null) splitList.AddRange(listHolder);
            else break;
            bList.Add(splitList[0]);
            bList.Add(splitList[1]);
        }

        areas.AddRange(aList);
        areas.AddRange(bList);

        return areas;
    }

    List<Vector3[,]> SplitDesicion(Vector3[,] sizeData)
    {
        List<Vector3[,]> itemList = new List<Vector3[,]>();
        int num = Random.Range(0, 2);

        switch (num)
        {
            case 0:
                if (sizeData.GetUpperBound(0) % 2 == 0) itemList.AddRange(CutVertical(sizeData));
                else if (sizeData.GetUpperBound(1) % 2 == 0) itemList.AddRange(CutHorizontal(sizeData));
                else print("Data size is to small to partition");
                break;
            case 1:
                if (sizeData.GetUpperBound(1) % 2 == 0) itemList.AddRange(CutHorizontal(sizeData));
                else if (sizeData.GetUpperBound(0) % 2 == 0) itemList.AddRange(CutVertical(sizeData));
                else print("Data size is to small to partition");
                break;
        }

        if (!itemList.Any())
        {
            print("item-list return null");
            return null;
        }

        return itemList;
    }


    List<Vector3[,]> CutVertical(Vector3[,] initalArray)
    {
        List<Vector3[,]> itemList = new List<Vector3[,]>();

        Vector3[,] arrayBlock = new Vector3[(initalArray.GetUpperBound(0) / 2) + 1, initalArray.GetUpperBound(1) + 1];

        for (int z = 0; z <= initalArray.GetUpperBound(1); z++)
        {
            for (int x = 0; x <= initalArray.GetUpperBound(0) / 2 - 1; x++)
            {
                // Gizmos.color = Color.green;
                // Gizmos.DrawSphere(initalArray[x, z], 0.5f);
                arrayBlock[x, z] = initalArray[x, z];
            }
        }

        itemList.Add(arrayBlock);

        Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0), arrayBlock.GetUpperBound(1) + 1];

        // print($"Updated Init array Size x:{UpdatedInitArry.GetUpperBound(0)} z: {UpdatedInitArry.GetUpperBound(1)}");

        for (int z = 0, zIndex = 0; z <= arrayBlock.GetUpperBound(1); z++, zIndex++)

        {
            for (int x = arrayBlock.GetUpperBound(0) + 1, xIndex = 0; x <= initalArray.GetUpperBound(0); x++, xIndex++)
            {
                // Gizmos.color = Color.red;
                // Gizmos.DrawSphere(initalArray[x, z], 0.5f);
                UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
            }
        }

        itemList.Add(UpdatedInitArry);


        for (int z = 0; z < arrayBlock.GetUpperBound(1) + 1; z++)
        {
            // Gizmos.color = Color.magenta;
            // Gizmos.DrawSphere(initalArray[arrayBlock.GetUpperBound(0), z], 0.5f);
            removeObject.Add(initalArray[arrayBlock.GetUpperBound(0), z]);
        }


        return itemList;
    }

    List<Vector3[,]> CutHorizontal(Vector3[,] initalArray)
    {
        List<Vector3[,]> itemList = new List<Vector3[,]>();

        Vector3[,] arrayBlock = new Vector3[initalArray.GetUpperBound(0) + 1, (initalArray.GetUpperBound(1) / 2) + 1];

        for (int z = 0; z <= initalArray.GetUpperBound(1) / 2 - 1; z++)
        {
            for (int x = 0; x <= initalArray.GetUpperBound(0); x++)
            {
                // Gizmos.color = Color.blue;
                // Gizmos.DrawSphere(initalArray[x, z], 0.05f);
                arrayBlock[x, z] = initalArray[x, z];
            }
        }

        itemList.Add(arrayBlock);

        Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0) + 1, arrayBlock.GetUpperBound(1) + 1];
        // print($"x: {UpdatedInitArry.GetUpperBound(0)}, z: {UpdatedInitArry.GetUpperBound(1)}");


        for (int z = arrayBlock.GetUpperBound(1) + 1, zIndex = 0; z <= initalArray.GetUpperBound(1); z++, zIndex++)

        {
            for (int x = 0, xIndex = 0; x <= arrayBlock.GetUpperBound(0); x++, xIndex++)
            {
                // Gizmos.color = Color.green;
                // Gizmos.DrawSphere(initalArray[x, z], 0.05f);
                UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
            }

            print("ran");
        }

        itemList.Add(UpdatedInitArry);
        for (int x = 0; x < arrayBlock.GetUpperBound(0) + 1; x++)
        {
            // Gizmos.color = Color.magenta;
            // Gizmos.DrawSphere(initalArray[x, arrayBlock.GetUpperBound(1)], 0.05f);
            removeObject.Add(initalArray[x, arrayBlock.GetUpperBound(1)]);
        }

        return itemList;
    }

    void BuildMap()
    {
        //Create RoadBoarder

        List<Vector3> streetLocations = new List<Vector3>();

        foreach (var point in mapData)
        {
            if (point.z == 0 || point.x == 0 || point.z == length || point.x == width) streetLocations.Add(point);
        }

        //create blocks/neigbberhoods/buildings
        foreach (var blocks in streetBlocks)
        {
            foreach (var point in blocks)
            {
                if (!buildingLocations.Contains(point) && !streetLocations.Contains(point))
                {
                    if (!removeObject.Contains(point))
                    {
                        buildingLocations.Add(point);
                    }
                }
            }
        }

        foreach (var buildingLocation in buildingLocations)
        {
            var randomBuilding = Random.Range(0, buildingsList.Count);
            var block = Instantiate(buildingsList[randomBuilding], buildingLocation, Quaternion.identity);
            Buildings.Add(block);
        }

        foreach (var building in Buildings)
        {
            building.transform.parent = building_Holder.transform;
        }
    }

    void GenArray()
    {
        mapData = new Vector3[width + 1, length + 1];

        for (int z = 0, i = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                mapData[x, z] = transform.position + Vector3.zero + new Vector3(x * tileSpacing, 0, z * tileSpacing);
            }
        }
// create floor base
        var w = width + boarder;
        var l = length + boarder;

        Vector3[] vertices = new Vector3[(w + 1) * (l + 1)];


        for (int z = 0, i = 0; z <= l; z++)
        {
            for (int x = 0; x <= w; x++, i++)
            {
                vertices[i] = (new Vector3(x * tileSpacing, 0, z * tileSpacing));
            }
        }

        ground.vertices = vertices;

        int[] triangles = new int[w * l * 6];
        for (int ti = 0, vi = 0, y = 0; y < l; y++, vi++)
        {
            for (int x = 0; x < w; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + w + 1;
                triangles[ti + 5] = vi + w + 2;
            }
        }

        ground.triangles = triangles;
        ground.RecalculateNormals();
    }

    // private void OnDrawGizmos()
    // {
    //     GenArray();
    //     foreach (var p in mapData)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawSphere(p, 0.05f);
    //     }
    // }
}