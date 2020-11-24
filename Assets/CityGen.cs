using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour
{
    [SerializeField] private int width, height;

    public Vector3[,] mapData;

    public List<Vector3[,]> listOfBlocks = new List<Vector3[,]>();


    public float sizePerArea;
    public float spacing;


    private Vector3 top;
    private Vector3 bottom;
    private Vector3 left;
    private Vector3 right;

    public GameObject cube;


    // Start is called before the first frame update
    void Start()
    {
        width = (int) gameObject.transform.localScale.x;
        height = (int) gameObject.transform.localScale.z;

        GenArray();
        CutVertical(mapData);
        buildBlock();
        // buildBlock(mapData);
    }


    void buildBlock()
    {
        // for (int z = 0; z <= array.GetUpperBound(1); z++)
        // {
        //     for (int x = 0; x <= array.GetUpperBound(0); x++)
        //     {
        //         var point = array[x, z];
        //         var block = Instantiate(cube, point, Quaternion.identity);
        //         block.transform.parent = gameObject.transform;
        //     }
        // }

        foreach (var blocks in listOfBlocks)
        {
            foreach (var point in blocks)
            {
                var block = Instantiate(cube, point, Quaternion.identity);
                block.transform.parent = gameObject.transform;
            }
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
                arrayBlock[x, z] = initalArray[x, z];
            }
        }

        listOfBlocks.Add(arrayBlock);
        //
        // foreach (var point in arrayBlock)
        // {
        //     print($"ArrayBlock point: {point}");
        // }
        //
        // foreach (var point in listOfBlocks[0])
        // {
        //     print($"ListBlock  point: {point}"); 
        // }
        // buildBlock(arrayBlock);
        // print(arrayBlock.GetUpperBound(0));

        Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0), arrayBlock.GetUpperBound(1) + 1];

        // print($"Updated Init array Size x:{UpdatedInitArry.GetUpperBound(0)} z: {UpdatedInitArry.GetUpperBound(1)}");

        for (int z = 0, zIndex = 0; z <= arrayBlock.GetUpperBound(1); z++, zIndex++)

        {
            for (int x = arrayBlock.GetUpperBound(0) + 1, xIndex = 0; x <= initalArray.GetUpperBound(0); x++, xIndex++)
            {
                UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
            }
        }

        listOfBlocks.Add(UpdatedInitArry);


        CutHorizontal(arrayBlock);
    }

    void CutHorizontal(Vector3[,] initalArray)
    {
        Vector3[,] arrayBlock = new Vector3[(width / 2) + 1, (height / 2) + 1];

        for (int z = 0; z <= initalArray.GetUpperBound(1) / 2 - 1; z++)
        {
            for (int x = 0; x <= initalArray.GetUpperBound(0); x++)
            {
                arrayBlock[x, z] = initalArray[x, z];
            }
        }

        listOfBlocks.Add(arrayBlock);


        Vector3[,] UpdatedInitArry = new Vector3[arrayBlock.GetUpperBound(0) + 1, arrayBlock.GetUpperBound(1) - 1];

        // +2
        for (int z = arrayBlock.GetUpperBound(0) + 2, zIndex = 0; z <= initalArray.GetUpperBound(1); z++, zIndex++)

        {
            for (int x = 0, xIndex = 0; x <= arrayBlock.GetUpperBound(0); x++, xIndex++)
            {
                UpdatedInitArry[xIndex, zIndex] = initalArray[x, z];
            }
        }

        listOfBlocks.Add(UpdatedInitArry);
    }
}