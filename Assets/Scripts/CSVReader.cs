using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class SublistData
{
    public List<string> SubItems { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

public class CSVReader : MonoBehaviour
{
    private TextAsset csvFile; // Reference to your .csv file (drag and drop it in the Unity inspector)
    public string[,] dataArray;
    public GridManager gridManager;

    public CSVLoader csvLoader;
    public UAP_AccessibilityManager uap; 
    //public List<string> mapItems = new List<string>();

    public List<string> mainList = new List<string>();
    public Dictionary<string, List<string>> subLists = new Dictionary<string, List<string>>();
    public Dictionary<string, Tuple<int, int>> subListPositions = new Dictionary<string, Tuple<int, int>>();
    // New dictionary for the third level sublist
    public Dictionary<string, List<string>> subSubLists = new Dictionary<string, List<string>>();

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

    void Start()
    {
        uap = GameObject.Find("Accessibility Manager").GetComponent<UAP_AccessibilityManager>();

        csvLoader = GameObject.FindWithTag("CSVLoader").GetComponent<CSVLoader>();
     
        csvFile = csvLoader.loadedCSV;

        LoadCSV();
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
            gridManager.setGridSize(numRows, numCols); 
            dataArray = new string[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                string[] row = lines[i].Split(',');

                for (int j = 0; j < numCols; j++)
                {
                    dataArray[i, j] = row[j];
                    gridManager.SetText(i, j, dataArray[i, j]);

                    StringParser parser = new StringParser();
                    Dictionary<string, string> parsedData = parser.ParseString(dataArray[i, j]);

                    if ((parsedData.TryGetValue("beacon", out string beaconValue)))
                    {
                        if (beaconValue == "train_station_ambient")
                        {
                            
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = train_station_ambient;
      
                        }
                        else if (beaconValue == "bank_booth")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = bank_booth;
                        }
                        else if (beaconValue == "elevator")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = elevator;
                        }
                        else if (beaconValue == "escalator")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = escalator;
                        }
                        else if (beaconValue == "mall_sound")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = mall_sound;
                        }
                        else if (beaconValue == "street_bg")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = street_bg;

                        }
                        else if (beaconValue == "ticket_counter")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = ticket_counter;
                        }
                        else if (beaconValue == "ticket_scan_main")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = ticket_scan_main;
                        }
                        else if (beaconValue == "washroom_ambient")
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().clip = washroom_ambient;
                        }
                        else //if the item has no sound assigned to it, disable it to save on resources
                        {
                            gridManager.grid[i, j].GetComponent<AudioSource>().enabled = false;
                            gridManager.grid[i, j].GetComponent<SoundManager>().enabled = false;
                        }
                        gridManager.grid[i, j].GetComponent<SoundManager>().myRow = i;
                        gridManager.grid[i, j].GetComponent<SoundManager>().myCol = j;

                        if (parsedData.TryGetValue("srange", out string srangeValue))
                        {
                            // Try to convert the srange value to an integer
                            if (float.TryParse(srangeValue, out float srange))
                            {

                                // If successful, set the max distance with the integer value
                                //convert srange to float to make sure the function can take it 
                                float srange_f = (float)srange;
                                gridManager.grid[i, j].GetComponent<SoundManager>().setMaxDistance(srange_f);
                            }
                        }
                        ListofGridsWithAmbient.Add(gridManager.grid[i, j]);
                    }
                    else
                    {
                        //Debug.Log("No sound file");
                    }



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
        StringParser parser = new StringParser();
        Dictionary<string, string> parsedData = parser.ParseString(inputString);
        if (parsedData.TryGetValue("level1h", out string level1h))
        {
            // Store in main list
            if (!mainList.Contains(level1h))
            {
                mainList.Add(level1h);
            }
            if (parsedData.TryGetValue("level2h", out string level2h))
            {
                //Debug.Log(level2h);
                // Store in sublist
                if (!subLists.ContainsKey(level1h))
                {
                    subLists[level1h] = new List<string>();
                }
                if (!subLists[level1h].Contains(level2h))
                {
                    subLists[level1h].Add(level2h);
                    subListPositions[level2h] = Tuple.Create(row, column);
                }
                // Assuming there's a third level identifier in your parsed data
                if (parsedData.TryGetValue("level3h", out string level3h))
                {
                    // Ensure the container for this level 2 header exists
                    if (!subSubLists.ContainsKey(level2h))
                    {
                        subSubLists[level2h] = new List<string>();
                    }
                    // Add the level 3 header if it's not already present
                    if (!subSubLists[level2h].Contains(level3h))
                    {
                        subSubLists[level2h].Add(level3h);
                        // If you need to store additional data for level 3 items, consider using a more complex structure here
                    }
                }
            }
            else
            {
                Debug.Log("No level2h index found");
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