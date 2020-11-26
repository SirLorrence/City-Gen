using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.PointerType;
using Random = UnityEngine.Random;

public class CityGen : MonoBehaviour
{
    private int width, height;

    public Vector3[,] mapData;

    private List<Vector3[,]> listOfBlocks = new List<Vector3[,]>();
    private List<Vector3> buildingLocations = new List<Vector3>();
    private List<Vector3> removeObject = new List<Vector3>();

    private List<GameObject> Buildings = new List<GameObject>();
    private List<GameObject> Streets = new List<GameObject>();


    public float spacing;
    public int blockSize;


    private Vector3 top;
    private Vector3 bottom;
    private Vector3 left;
    private Vector3 right;

    public GameObject cube;
    public GameObject road;

    public GameObject building_Holder;
    public GameObject street_Holder;

    // Start is called before the first frame update
    void Start()
    {
        width = (int) gameObject.transform.localScale.x;
        height = (int) gameObject.transform.localScale.z;

        GenArray();
        listOfBlocks = CreateSpacePartitioning(mapData, blockSize);
        // CutVertical(mapData);
        BuildMap();
    }


    List<Vector3[,]> CreateSpacePartitioning(Vector3[,] data, int areaSize)
    {
        List<Vector3[,]> aList = new List<Vector3[,]>();
        List<Vector3[,]> bList = new List<Vector3[,]>();
        List<Vector3[,]> splitList = new List<Vector3[,]>();

        List<Vector3[,]> Areas = new List<Vector3[,]>();


        //init cut in half

        splitList.AddRange(SplitDesicion(data));
        aList.Add(splitList[0]);
        bList.Add(splitList[1]);

        for (int i = 0; i < blockSize; i++)
        {
            splitList.Clear();
            var listHolder = SplitDesicion(aList[i]);
            if (listHolder != null) splitList.AddRange(listHolder);
            else break;
            aList.Add(splitList[0]);
            aList.Add(splitList[1]);
        }

        for (int i = 0; i < blockSize; i++)
        {
            splitList.Clear();
            var listHolder = SplitDesicion(bList[i]);
            if (listHolder != null) splitList.AddRange(listHolder);
            else break;
            bList.Add(splitList[0]);
            bList.Add(splitList[1]);
        }

        Areas.AddRange(aList);
        Areas.AddRange(bList);

        return Areas;
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
            print("itemlist return null");
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

        List<Vector3> Boarder = new List<Vector3>();

        foreach (var point in mapData)
        {
            if (point.z == 0 || point.x == 0 || point.z == height || point.x == width)
            {
                var block = Instantiate(road, point, Quaternion.identity);
                Boarder.Add(point);
                Streets.Add(block);
            }
        }

        //create blocks/neigbberhoods/buildings
        foreach (var blocks in listOfBlocks)
        {
            foreach (var point in blocks)
            {
                if (!buildingLocations.Contains(point) && !Boarder.Contains(point))
                {
                    if (!removeObject.Contains(point))
                    {
                        buildingLocations.Add(point);
                    }
                }
            }
        }

        for (int i = 0; i < buildingLocations.Count; i++)
        {
            var temp = buildingLocations[i];
            var y = (cube.transform.localScale.y / 2);
            var setLevel = new Vector3(temp.x, y ,temp.z );
            buildingLocations[i] = setLevel;
        }


        foreach (var buildingLocation in buildingLocations)
        {
            var block = Instantiate(cube, buildingLocation, Quaternion.identity);
            Buildings.Add(block);
        }
        //Remove objects will become Roads & Streets
        foreach (var point in removeObject)
        {
            var block = Instantiate(road, point, Quaternion.identity);
            Streets.Add(block);
        }

        foreach (var building in Buildings)
        {
            building.transform.parent = building_Holder.transform;
        }
        
        foreach (var street in Streets)
        {
            street.transform.parent = street_Holder.transform;
        }
    }

    void GenArray()
    {
        mapData = new Vector3[width + 1, height + 1];
        float ySpace = 0;
        float xSpace = 0;
        for (int y = 0, i = 0; y <= height; y++, ySpace += spacing)
        {
            for (int x = 0; x <= width; x++, i++, xSpace += spacing)
            {
                mapData[x, y] = new Vector3(x + xSpace, 0, y + ySpace);
            }

            xSpace = 0;
        }
    }


    // void CutVertical(Vector3[,] initalArray)
    // {
    //     Vector3[,] arrayBlock = new Vector3[(initalArray.GetUpperBound(0) / 2) + 1, initalArray.GetUpperBound(1) + 1];
    //
    //     for (int z = 0; z <= initalArray.GetUpperBound(1); z++)
    //     {
    //         for (int x = 0; x <= initalArray.GetUpperBound(0) / 2 - 1; x++)
    //         {
    //             // Gizmos.color = Color.green;
    //             // Gizmos.DrawSphere(initalArray[x, z], 0.5f);
    //             arrayBlock[x, z] = initalArray[x, z];
    //         }
    //     }
    //
    //     listOfBlocks.Add(arrayBlock);
    //     //
    //
    //     Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0), arrayBlock.GetUpperBound(1) + 1];
    //
    //     // print($"Updated Init array Size x:{UpdatedInitArry.GetUpperBound(0)} z: {UpdatedInitArry.GetUpperBound(1)}");
    //
    //     for (int z = 0, zIndex = 0; z <= arrayBlock.GetUpperBound(1); z++, zIndex++)
    //
    //     {
    //         for (int x = arrayBlock.GetUpperBound(0) + 1, xIndex = 0; x <= initalArray.GetUpperBound(0); x++, xIndex++)
    //         {
    //             // Gizmos.color = Color.red;
    //             // Gizmos.DrawSphere(initalArray[x, z], 0.5f);
    //             UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
    //         }
    //     }
    //
    //     listOfBlocks.Add(UpdatedInitArry);
    //
    //
    //     for (int z = 0; z < arrayBlock.GetUpperBound(1) + 1; z++)
    //     {
    //         // Gizmos.color = Color.magenta;
    //         // Gizmos.DrawSphere(initalArray[arrayBlock.GetUpperBound(0), z], 0.5f);
    //         removeObject.Add(initalArray[arrayBlock.GetUpperBound(0), z]);
    //     }
    //
    //     // CutHorizontal(arrayBlock);
    // }

    // void CutHorizontal(Vector3[,] initalArray)
    // {
    //     Vector3[,] arrayBlock = new Vector3[initalArray.GetUpperBound(0) + 1, (initalArray.GetUpperBound(1) / 2) + 1];
    //
    //     for (int z = 0; z <= initalArray.GetUpperBound(1) / 2 - 1; z++)
    //     {
    //         for (int x = 0; x <= initalArray.GetUpperBound(0); x++)
    //         {
    //             // Gizmos.color = Color.blue;
    //             // Gizmos.DrawSphere(initalArray[x, z], 0.05f);
    //             arrayBlock[x, z] = initalArray[x, z];
    //         }
    //     }
    //
    //     listOfBlocks.Add(arrayBlock);
    //
    //
    //     Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0) + 1, arrayBlock.GetUpperBound(1) + 1];
    //     print($"x: {UpdatedInitArry.GetUpperBound(0)}, z: {UpdatedInitArry.GetUpperBound(1)}");
    //
    //     for (int z = arrayBlock.GetUpperBound(0) + 1, zIndex = 0; z <= initalArray.GetUpperBound(1); z++, zIndex++)
    //
    //     {
    //         for (int x = 0, xIndex = 0; x <= arrayBlock.GetUpperBound(0) - 1; x++, xIndex++)
    //         {
    //             // Gizmos.color = Color.yellow;
    //             // Gizmos.DrawSphere(initalArray[x, z], 0.05f);
    //             UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
    //         }
    //     }
    //
    //
    //     for (int x = 0; x < arrayBlock.GetUpperBound(0); x++)
    //     {
    //         // Gizmos.color = Color.magenta;
    //         // Gizmos.DrawSphere(initalArray[x, arrayBlock.GetUpperBound(1)], 0.05f);
    //         removeObject.Add(initalArray[x, arrayBlock.GetUpperBound(1)]);
    //     }
    //
    //     listOfBlocks.Add(UpdatedInitArry);
    // }

    //
    // private void OnDrawGizmos()
    // {
    //     foreach (var p in mapData)
    //     {
    //         Gizmos.color = Color.black;
    //         Gizmos.DrawSphere(p, 0.05f);
    //     }
    //
    //     CutHorizontal(mapData);
    // }
}