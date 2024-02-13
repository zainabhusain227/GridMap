using System;
using System.Collections.Generic;
using System.IO;

public static class CsvFilesContentHolder
{
    public static Dictionary<string, string> CsvContents = new Dictionary<string, string>();

    public static void LoadCsvFilesContents(string folderPath)
    {
        // Ensure the path is relative to the application's root directory.
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath);

        try
        {
            // Check if the directory exists
            if (Directory.Exists(fullPath))
            {
                // Iterate over all CSV files in the folder
                foreach (var file in Directory.GetFiles(fullPath, "*.csv"))
                {
                    string fileName = Path.GetFileName(file);
                    string fileContent = File.ReadAllText(file);

                    // Store the content in the dictionary
                    CsvContents[fileName] = fileContent;
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {fullPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Specify the relative path to the folder from the application's root directory.
        string folderPath = "assets/Spatial Snapshot Unity Spreadsheet Map";

        // Load CSV files content
        CsvFilesContentHolder.LoadCsvFilesContents(folderPath);

        // Example usage: Access and print the content of a specific CSV file
        string fileName = "example.csv";
        if (CsvFilesContentHolder.CsvContents.ContainsKey(fileName))
        {
            Console.WriteLine($"Contents of {fileName}:");
            Console.WriteLine(CsvFilesContentHolder.CsvContents[fileName]);
        }
        else
        {
            Console.WriteLine($"File not found: {fileName}");
        }
    }
}
