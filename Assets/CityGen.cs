using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CityGen : MonoBehaviour
{
    [SerializeField] private int width, height;

    public Vector3[,] mapData;

    public List<Vector3[,]> listOfBlocks = new List<Vector3[,]>();
    public List<Vector3> buildings = new List<Vector3>();
    private List<Vector3> removeObject = new List<Vector3>();

    public float spacing;


    private Vector3 top;
    private Vector3 bottom;
    private Vector3 left;
    private Vector3 right;

    public GameObject cube;
    public GameObject road;


    // Start is called before the first frame update
    void Start()
    {
        width = (int) gameObject.transform.localScale.x;
        height = (int) gameObject.transform.localScale.z;

        GenArray();
        CutVertical(mapData);
        buildBlock();
    }


    void buildBlock()
    {
        print(removeObject[0]);
        //create blocks/neigbberhoods
        foreach (var blocks in listOfBlocks)
        {
            foreach (var point in blocks)
            {
                if (!buildings.Contains(point))
                {
                    if (!removeObject.Contains(point))
                    {
                        var block = Instantiate(cube, point, Quaternion.identity);
                        buildings.Add(block.transform.position);
                        block.transform.parent = gameObject.transform;
                    }
                }
            }
        }

        foreach (var point in removeObject)
        {
                  var block = Instantiate(road, point, Quaternion.identity);
                        buildings.Add(block.transform.position);
                        block.transform.parent = gameObject.transform;
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


    void CutVertical(Vector3[,] initalArray)
    {
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

        listOfBlocks.Add(arrayBlock);
        //

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

        listOfBlocks.Add(UpdatedInitArry);


        for (int z = 0; z < arrayBlock.GetUpperBound(1) + 1; z++)
        {
            // Gizmos.color = Color.magenta;
            // Gizmos.DrawSphere(initalArray[arrayBlock.GetUpperBound(0), z], 0.5f);
            removeObject.Add(initalArray[arrayBlock.GetUpperBound(0), z]);
        }

        CutHorizontal(arrayBlock);
    }

    void CutHorizontal(Vector3[,] initalArray)
    {
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

        listOfBlocks.Add(arrayBlock);


        Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0) + 1, arrayBlock.GetUpperBound(1) + 1];
        print($"x: {UpdatedInitArry.GetUpperBound(0)}, z: {UpdatedInitArry.GetUpperBound(1)}");

        for (int z = arrayBlock.GetUpperBound(0) + 1, zIndex = 0; z <= initalArray.GetUpperBound(1); z++, zIndex++)

        {
            for (int x = 0, xIndex = 0; x <= arrayBlock.GetUpperBound(0) - 1; x++, xIndex++)
            {
                // Gizmos.color = Color.yellow;
                // Gizmos.DrawSphere(initalArray[x, z], 0.05f);
                UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
            }
        }


        for (int x = 0; x < arrayBlock.GetUpperBound(0) ; x++)
        {
            // Gizmos.color = Color.magenta;
            // Gizmos.DrawSphere(initalArray[x, arrayBlock.GetUpperBound(1)], 0.05f);
            removeObject.Add(initalArray[x, arrayBlock.GetUpperBound(1)]);
        }

        listOfBlocks.Add(UpdatedInitArry);
    }


    private void OnDrawGizmos()
    {
        // foreach (var p in mapData)
        // {
        //     Gizmos.color = Color.black;
        //     Gizmos.DrawSphere(p, 0.05f);
        // }
        //
        // CutVertical(mapData);
    }
}