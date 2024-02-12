using System;
using System.Collections.Generic;

public class StringParser
{
    public Dictionary<string, string> ParseString(string input)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        // Split the input string into segments based on '/'
        string[] segments = input.Split('/');

        foreach (string segment in segments)
        {
            // Further split each segment into key and value parts based on '='
            string[] parts = segment.Split('=');
            if (parts.Length == 2) // Ensure there are exactly two parts
            {
                string key = parts[0].Trim(); // Remove any leading/trailing whitespace
                string value = parts[1].Trim();

                // Add the key-value pair to the dictionary
                // This will overwrite the value if the key already exists
                result[key] = value;
            }
        }

        return result;
    }
}
