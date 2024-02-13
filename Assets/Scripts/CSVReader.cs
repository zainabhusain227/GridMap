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

public class CSVReaderFinal : MonoBehaviour
{
    public TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;
    public GridManager gridManager;
    public SoundManager soundManager;  // Reference to your SoundManager script.

    //public List<string> mapItems = new List<string>();

    public List<string> mainList = new List<string>();
    public Dictionary<string, List<string>> subLists = new Dictionary<string, List<string>>();
    public Dictionary<string, Tuple<int, int>> subListPositions = new Dictionary<string, Tuple<int, int>>();

    public AudioClip train_station_ambient;
    public AudioClip bank_booth;
    public AudioClip elevator;
    public AudioClip escalator;
    public AudioClip mall_sound;
    public AudioClip street_bg;
    public AudioClip ticket_counter;
    public AudioClip ticket_scan_main;
    public AudioClip washroom_ambient;



    public List<GameObject> ListofGridsWithAmbient;
    //public List<int[]> ListofGridsWithAmbient = new List<int[]>();


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
            //Debug.Log("Total number of Rows: " + numRows);
            //Debug.Log("Total number of Columns: " + numCols);
            gridManager.rows = numRows;
            gridManager.columns = numCols;


            dataArray = new string[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                string[] row = lines[i].Split(',');

                for (int j = 0; j < numCols; j++)
                {
                    dataArray[i, j] = row[j];
                    gridManager.SetText(i, j, dataArray[i, j]); //gets rows and colums from csv file
                    if (dataArray[i, j].Contains("*"))
                    {
                        //make a list of audioclip locations, track the grid locations that have the audio source
                        string[] substring = dataArray[i, j].Split("*");
                        //Debug.Log(substring[0]);
                        Debug.Log(substring[1]);

                        //string[] filename_radius= substring[1].Split("@");

                        //string filename = filename_radius[0];
                        //string radius = filename_radius[1];
                        //Debug.Log(filename);
                        //Debug.Log(radius);

                        //float radius_f= (float)Convert.ToDouble(radius);
                        //Debug.Log(radius_f);
                        //gridManager.grid[i, j].GetComponent<AudioSource>().clip = filename;
                        //substring[1] = GetStringBeforeAt(substring[1]);
                        Debug.Log("Sub string 1 = " + substring[1]);

                        if (substring[1] == "train_station_ambient")
                        {
                            //if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = train_station_ambient;
                            //Debug.Log("found train");
                        }
                        else if (substring[1] == "bank_booth")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = bank_booth;
                        }
                        else if (substring[1] == "elevator")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = elevator;
                        }
                        else if (substring[1] == "escalator")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = escalator;
                        }
                        else if (substring[1] == "mall_sound")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = mall_sound;
                        }
                        else if (substring[1] == "street_bg")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = street_bg;
                       
                        }
                        else if (substring[1] == "ticket_counter")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = ticket_counter;
                        }
                        else if (substring[1] == "ticket_scan_main")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = ticket_scan_main;
                        }
                        else if (substring[1] == "washroom_ambient")
                        {//if(substring[1] == "Train_station_sound"){
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = washroom_ambient;
                        }
                        else //if the item has no sound assigned to it, disable it to save on resources
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().enabled = false;
                            gridManager.grid[i, j].GetComponent<SoundManager>().enabled = false;
                        }

                        //gridManager.grid[i, j].GetComponent<SoundManager>().max_distance = radius_f;
                        gridManager.grid[i, j].GetComponent<SoundManager>().myRow = i;
                        gridManager.grid[i, j].GetComponent<SoundManager>().myCol = j;
                        ListofGridsWithAmbient.Add(gridManager.grid[i, j]);
                    }


                    //find if theres an asterisk, 
                    //AppendIfNotExists(dataArray[i, j], i, j);
                    StoreStrings(dataArray[i, j], i, j);
                }
            }
        }
        else
        {
            Debug.LogError("CSV file is not assigned!");
        }
        // Sort mainList alphabetically
        mainList.Sort();

        // Sort sublists alphabetically
        foreach (var sublist in subLists.Values)
        {
            sublist.Sort();
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
    public string GetStringBeforeAt(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }

        int atIndex = input.IndexOf('@');

        if (atIndex == -1)
        {
            return input; // No '@' character found, return the whole string.
        }

        return input.Substring(0, atIndex);
    }
}