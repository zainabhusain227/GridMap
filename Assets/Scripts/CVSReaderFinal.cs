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
    public SoundManager soundManager;  // Reference to your SoundManager script.

    public List<string> mapItems = new List<string>();
    public List<int> mapItemRow;
    public List<int> mapItemCol;

    public AudioClip trainSound;
    public AudioClip streetSound;


    public List<GameObject> ListofGridsWithAmbient;
    //public List<int[]> ListofGridsWithAmbient = new List<int[]>();

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
            
            int numRows = lines.Length -1; // for some reason, converting from excel to CSV adds an extra line
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
                    gridManager.SetText(i, j, dataArray[i,j]); //gets rows and colums from csv file
                    if (dataArray[i,j].Contains("*")){
                        //make a list of audioclip locations, track the grid locations that have the audio source
                        string[] substring= dataArray[i,j].Split("*");
                        if (substring[1] == "trains")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = trainSound;
                        }
                        else if (substring[1] == "street")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = streetSound;
                        }
                        gridManager.grid[i, j].GetComponent<SoundManager>().myRow = i;
                        gridManager.grid[i, j].GetComponent<SoundManager>().myCol = j;
                        ListofGridsWithAmbient.Add(gridManager.grid[i,j]);
                    }


                    //find if theres an asterisk, 
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

    // Function to append a string to a list if it doesn't exist
    public void AppendIfNotExists(string newString, int row, int col)
    {
        if (string.IsNullOrEmpty(newString) || string.IsNullOrWhiteSpace(newString))
        {
            //Debug.Log("Empty string. Not added to the list.");
        }
        else if (newString == "Wall")
        {
            //Debug.Log("Ignoring irrelevant POI");
        }
        else if (!mapItems.Contains(newString))
        {
            mapItems.Add(newString);
            mapItemRow.Add(row);
            mapItemCol.Add(col);    

            //Debug.Log("String appended to the list.");
        }
        else
        {
            //Debug.Log("String already exists in the list.");
        }
    }
}