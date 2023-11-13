using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class CVSReaderFinal : MonoBehaviour
{
    public TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;
    public GridManager gridManager;

    public List<string> mapItems = new List<string>();

    public List<string> salesItems = new List<string>();
    public List<string> entrancesItems = new List<string>();
    public List<string> informationItems = new List<string>();
    public List<string> movementItems = new List<string>();


    //public List<List<string>> mapItemsMainList = new List<List<string>>();
    public List<int> salesItemRow;
    public List<int> salesItemCol;

    public List<int> entrancessItemRow;
    public List<int> entrancesItemCol;

    public List<int> movementItemRow;
    public List<int> movementItemCol;

    public List<int> informationsItemRow;
    public List<int> informationItemCol;

    public List<int> mapItemRow;
    public List<int> mapItemCol;

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
                    gridManager.SetText(i, j, dataArray[i, j]);
                    AppendIfNotExists(dataArray[i, j], i, j);
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
    

    public void AppendIfNotExists(string newString, int row, int col)
    {
        if (newString.Contains(";"))
        {
            string mainItem = newString.Substring(0, newString.IndexOf(";"));
            string subItem = newString.Substring(newString.IndexOf(";") + 1);

            if (!mapItems.Contains(mainItem))
            {
                mapItems.Add(mainItem);
                //mapItemRow.Add(row);
                //mapItemCol.Add(col);
            }
            
            
            if (mainItem == "Sales")
            {
                if (!salesItems.Contains(subItem))
                {
                    salesItems.Add(subItem);
                    salesItemRow.Add(row);
                    salesItemCol.Add(col);
                }

            }
            else if (mainItem == "Entrances")
            {
                if (!entrancesItems.Contains(subItem))
                {
                    entrancesItems.Add(subItem);
                    entrancessItemRow.Add(row);
                    entrancesItemCol.Add(col);
                }

            }
            else if (mainItem == "Movement")
            {
                if (!movementItems.Contains(subItem))
                {
                    movementItems.Add(subItem);
                    movementItemRow.Add(row);
                    movementItemCol.Add(col);
                }

            }
            else if (mainItem == "Information")
            {
                if (!informationItems.Contains(subItem))
                {
                    informationItems.Add(subItem);
                    informationsItemRow.Add(row);
                    informationItemCol.Add(col);
                }

            }
            
            
        }
    }

       
}