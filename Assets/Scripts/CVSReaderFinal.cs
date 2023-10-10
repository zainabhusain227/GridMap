using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CVSReaderFinal : MonoBehaviour
{
    public TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;
    public GridManager gridManager;

    void Start()
    {
        LoadCSV();
        //PrintData();
    }

    void LoadCSV()
    {
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n');
            //int numRows = 26;
            //int numCols = 26;
            
            int numRows = lines.Length; // for some reason, converting from excel to CSV adds an extra line
            int numCols = lines[0].Split(',').Length;
            Debug.Log("Rows: " + numRows);
            Debug.Log("Columns: " + numCols);
            
            dataArray = new string[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                string[] row = lines[i].Split(',');

                for (int j = 0; j < numCols; j++)
                {
                    dataArray[i, j] = row[j];
                    gridManager.SetText(i, j, dataArray[i,j]);
                }
            }
        }
        else
        {
            Debug.LogError("CSV file is not assigned!");
        }
    }

    void PrintData()
    {
        int numRows = dataArray.GetLength(0);
        int numCols = dataArray.GetLength(1);

        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Debug.Log($"Data[{i},{j}]: {dataArray[i, j]}");
            }
        }
    }
}