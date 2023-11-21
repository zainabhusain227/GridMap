using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class SublistData
{
    public List<string> SubItems { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

public class CVSReaderFinal : MonoBehaviour
{
    public TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;
    public GridManager gridManager;

    //public List<string> mapItems = new List<string>();
    
    public List<string> mainList = new List<string>();
    public Dictionary<string, List<string>> subLists = new Dictionary<string, List<string>>();
    public Dictionary<string, Tuple<int, int>> subListPositions = new Dictionary<string, Tuple<int, int>>();

    void Start()
    {
        LoadCSV();
        //PrintMain_SubList();
        //PrintSubListPositions();
    }

    void LoadCSV()
    {
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n');
            int numRows = lines.Length - 1; // for some reason, converting from excel to CSV adds an extra line ***************************************** may have to subtract 1 or not
            int numCols = lines[0].Split(',').Length;
            Debug.Log("Total number of Rows: " + numRows);
            Debug.Log("Total number of Columns: " + numCols);

            dataArray = new string[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                string[] row = lines[i].Split(',');

                for (int j = 0; j < numCols; j++)
                {
                    dataArray[i, j] = row[j];
                    gridManager.SetText(i, j, dataArray[i, j]);
                    //AppendIfNotExists(dataArray[i, j], i, j);
                    StoreStrings(dataArray[i, j], i, j);
                }
            }
        }
        else
        {
            Debug.LogError("CSV file is not assigned!");
        }

    }
    public void StoreStrings(string inputString, int row, int column)
    {
        if (inputString.Contains(";"))
        {
            string[] substrings = inputString.Split(';');
            //Debug.Log(substrings[0] + " | " + substrings[1]);
            if (substrings.Length >= 2)
            {
                string mainItem = substrings[0];
                string subItem = substrings[1];

                // Store in main list
                if (!mainList.Contains(mainItem))
                {
                    mainList.Add(mainItem);
                }

                // Store in sublist
                if (!subLists.ContainsKey(mainItem))
                {
                    subLists[mainItem] = new List<string>();
                }
                if (!subLists[mainItem].Contains(subItem))
                {
                    subLists[mainItem].Add(subItem);
                    subListPositions[subItem] = Tuple.Create(row, column);

                }
            }
            else
            {
                // Handle invalid input
                Console.WriteLine("Invalid input string format.");
            }
        }

    }
    public void PrintMain_SubList()
    {
        Debug.Log("Main List:");
        foreach (var mainItem in mainList)
        {
            Debug.Log($"- {mainItem}");

            if (subLists.ContainsKey(mainItem))
            {
                Debug.Log("  Sublist:");

                foreach (var subItem in subLists[mainItem])
                {
                    Debug.Log($"  - {subItem}");
                }
            }
            // print a seperator 
            Debug.Log("****************************************************************");
        }
        Debug.Log(subListPositions);
    }
    // Method to print all data in the subListPositions dictionary
    void PrintSubListPositions()
    {
        foreach (var kvp in subListPositions)
        {
            string key = kvp.Key;
            Tuple<int, int> value = kvp.Value;

            int firstValue = value.Item1;
            int secondValue = value.Item2;

           Debug.Log($"Key: {key}, Value: ({firstValue}, {secondValue})");
        }
    }
}