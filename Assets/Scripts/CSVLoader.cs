using UnityEngine;
using System.IO;
using UnityEditor; // This is crucial for accessing Unity's AssetDatabase and other editor functionalities

public class CSVLoader : MonoBehaviour
{
    // Store the loaded file paths here
    public string[] csvFilePaths;

    // Assuming this method is called to load the CSV files' paths
    public void LoadCSVFilePaths()
    {
        string folderPath = "Assets/Maps/"; // Change this to your specific folder path
        LoadCSVFilesFromFolder(folderPath);
    }

    private void LoadCSVFilesFromFolder(string folderPath)
    {
        // Get all file paths from the folder
        string[] filePaths = Directory.GetFiles(folderPath, "*.csv");

        // Initialize the csvFilePaths array with the number of found csv files
        csvFilePaths = new string[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            // Convert the full path to a relative path that Unity uses
            string relativePath = filePaths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
            csvFilePaths[i] = relativePath;
        }

        // Optionally, print the paths to the console to verify
        foreach (string path in csvFilePaths)
        {
            Debug.Log(path);
        }
    }
}
