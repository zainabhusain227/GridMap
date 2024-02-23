using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GridNavigator : MonoBehaviour
{
    public GridManager gridManager;  // Reference to your GridManager script.
    public CSVReader cvsRF;
    public int currentRow = 0;      // Current row index.
    public int currentColumn = 0;   // Current column index.
    public int numberOfSteps = 0;

    public UAP_AccessibilityManager uap;


    public int[] neighboringRows;
    public int[] neighboringColumns;

    public GameObject cTTS;

    public Transform parentObject; // The parent object around which you want to calculate positions.

    public Vector3[] positions = new Vector3[8]; // Array to store the calculated positions.

    public float soundDistance = 300f;
    public AudioSource StepSourceLeftRight;
    public AudioSource StepSourceUpDown;

    public AudioSource BoundarySource;

    public Vector3 LeftSide;
    public Vector3 RightSide;
    public Vector3 ForwardSide;
    public Vector3 BackSide;
    public Vector3 Center;

    public Vector3 LandmarkVect;

    public bool indexMode = false;
    public bool subIndexMode = false;
    public int mapItemIndex = 0;




    public string currentMapItem;
    public GameObject mapItemAudioSource;

    public CSVLoader csvLoader;
    //int itemRow;
    //int itemCol;

    int row;
    int column;

    private int mainListIndex = 0;
    private string selectedMainItem;

    private int sublistIndex = 0;
    private string selectedSubItem;
    private bool inSublist = false;
    // Class-level variables to manage subsublist navigation
    private bool inSubSublist = false;
    private int subSublistIndex = 0;
    private bool returnKeyPressed = false;

    void Start()
    {
        uap = GameObject.Find("Accessibility Manager").GetComponent<UAP_AccessibilityManager>();
        csvLoader = GameObject.FindWithTag("CSVLoader").GetComponent<CSVLoader>();
        // Initialize the starting position.
        currentRow = csvLoader.zoomInRow;
        currentColumn = csvLoader.zoomInColumn;
        UpdateGridPosition(currentRow, currentColumn);
        CalculatePositions();

        LeftSide = new Vector3(-10, 0, 0);
        RightSide = new Vector3(10, 0, 0);
        ForwardSide = new Vector3(0, 0, 10);
        BackSide = new Vector3(0, 0, -10);
        Center = new Vector3(0, 0, 0);

        LandmarkVect = new Vector3(0, 0, 0);
        //take difference between position on grid and coordinate of landmark
        StartCoroutine(ExecuteEndOfFrame());


    }
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.I))
        {
            if (indexMode == false)
            {
                indexMode = true;
                uap.Saysomething("Index mode activated, first index: " + cvsRF.mainList[0]);
                mapItemIndex = 0;
            }
            else if (indexMode == true)
            {
                uap.Saysomething("Index mode deactivated");
                indexMode = false;
            }
        }


        if (indexMode == false)
        {
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
            {
                JumpRight();
                Debug.Log("Jumping right");
            }
            else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
            {
                JumpLeft();
                Debug.Log("Jumping left");

            }
            else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                JumpUp();

                Debug.Log("Jumping up");

            }
            else if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                JumpDown();
                Debug.Log("Jumping down");

            }
            // Move the selection based on arrow key input.
            else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveUp();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                speakAroundMe();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                speakCoordinates();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                speakLandmarks();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                //Debug.Log(cvsRF.csvLoader.loadedCSV.name);
                uap.Saysomething("you are in map" + cvsRF.csvLoader.loadedCSV.name);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                //Debug.Log(cvsRF.csvLoader.loadedCSV.name);
                uap.Saysomething("Help Menu. Press enter to zoom into an area. Press backspace to zoom out of an area. Press M to hear the name of the current map");
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                string originalText = gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                StringParser parser = new StringParser();
                Dictionary<string, string> parsedData = parser.ParseString(originalText);
                string[] inParts;
                string[] outParts;

                if (parsedData.TryGetValue("warpfile", out string warpFile))
                {
                    int inRow = 0, inCol = 0, outRow = 0, outCol = 0;

                    if (parsedData.TryGetValue("zoom_in_csv", out string inRowCol))
                    {
                        inParts = inRowCol.Split('-');
                        if (int.TryParse(inParts[0], out int firstNumber) && int.TryParse(inParts[1], out int secondNumber))
                        {
                            Debug.Log("Moving to: " + firstNumber + " | " + secondNumber);
                            inRow = firstNumber;
                            inCol = secondNumber;
                        }
                    }
                    if (parsedData.TryGetValue("zoom_out_csv", out string outRowCol))
                    {
                        outParts = outRowCol.Split('-');
                        if (int.TryParse(outParts[0], out int firstNumber) && int.TryParse(outParts[1], out int secondNumber))
                        {
                            Debug.Log("Previous position was: " + firstNumber + " | " + secondNumber);
                            outRow = firstNumber;
                            outCol = secondNumber;

                        }
                    }
                    loadMap(warpFile, inRow, inCol, outRow, outCol);

                }
                else
                {
                    Debug.Log("This cell does not have a warpfile to jump to");
                }

            }
            else if((UnityEngine.Input.GetKeyDown(KeyCode.Backspace)) && (csvLoader.returnCurrentCSV() != null))
            {
                returnToPreviousMap();
            }
        }
        else if (indexMode == true)
        {
            HandleInput();
        }
    }
        void JumpRight()
        {
            string currentString = gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text; // Store current item
            Debug.Log(currentString);
            StepSourceLeftRight.transform.localPosition = RightSide;  // Change step sound position

            if (numberOfSteps != 1)
            {
                gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            }

            // Check if moving to the right exceeds the grid boundaries
            while (currentColumn < gridManager.columns - 1)
            {
                currentColumn++;

                float delay = 0.1f * currentColumn; // Adjust the delay as needed
                Invoke("PlayAudioLR", delay);

                // Check if the next grid cell has a different text
                if (gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != currentString)
                {
                    break;
                }
            }

            UpdateGridPosition(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(currentRow, currentColumn);
        }

        void JumpLeft()
        {
            string currentString = gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            Debug.Log(currentString);
            StepSourceLeftRight.transform.localPosition = LeftSide;

            if (numberOfSteps != 1)
            {
                gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            }

            // Check if moving to the left exceeds the grid boundaries
            while (currentColumn > 0)
            {
                currentColumn--;
                float delay = 0.1f * Math.Abs(currentColumn); // Adjust the delay as needed
                Invoke("PlayAudioLR", delay);
                // Check if the next grid cell has a different text
                if (gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != currentString)
                {
                    break;
                }
            }

            UpdateGridPosition(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(currentRow, currentColumn);

            // Change step sound position
            //StepSourceLeftRight.Play();
        }

        void JumpDown()
        {
            string currentString = gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            Debug.Log(currentString);
            StepSourceUpDown.transform.localPosition = ForwardSide;

            if (numberOfSteps != 1)
            {
                gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            }

            // Check if moving up exceeds the grid boundaries
            while (currentRow < gridManager.rows - 1)
            {
                currentRow++;
                float delay = 0.2f * Math.Abs(currentRow); // Adjust the delay as needed
                Invoke("PlayAudioUD", delay);
                // Check if the next grid cell has a different text
                if (gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != currentString)
                {
                    break;
                }
            }

            UpdateGridPosition(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(currentRow, currentColumn);

            // Change step sound position
            //StepSourceUpDown.Play();
        }

        void JumpUp()
        {
            string currentString = gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            Debug.Log(currentString);
            StepSourceUpDown.transform.localPosition = BackSide;

            if (numberOfSteps != 1)
            {
                gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            }

            // Check if moving down exceeds the grid boundaries
            while (currentRow > 0)
            {
                currentRow--;
                float delay = 0.2f * Math.Abs(currentRow); // Adjust the delay as needed
                Invoke("PlayAudioUD", delay);
                // Check if the next grid cell has a different text
                if (gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != currentString)
                {
                    break;
                }
            }

            UpdateGridPosition(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(currentRow, currentColumn);

            // Change step sound position
            //StepSourceUpDown.Play();
        }
        // Move selection to the right.
        void MoveRight()
        {
            if (currentColumn < gridManager.columns - 1)
            {
                if (numberOfSteps != 1)
                {
                    gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
                }

                currentColumn++;

                UpdateGridPosition(currentRow, currentColumn);
                gridManager.focusOnCurrentGrid(currentRow, currentColumn);
                //change step sound position
                StepSourceLeftRight.transform.localPosition = RightSide;
                StepSourceLeftRight.Play();

            }
            else
            {
                BoundarySource.Play();
            }
        }
        // Move selection to the left.
        void MoveLeft()
        {
            if (currentColumn > 0)
            {
                if (numberOfSteps != 1)
                {
                    gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
                }

                currentColumn--;

                UpdateGridPosition(currentRow, currentColumn);
                gridManager.focusOnCurrentGrid(currentRow, currentColumn);
                StepSourceLeftRight.transform.localPosition = LeftSide;
                StepSourceLeftRight.Play();
            }
            else {
                BoundarySource.Play();
            }
        }

        // Move selection up.
        void MoveUp()
        {
            if (currentRow > 0)
            {
                if (numberOfSteps != 1)
                {
                    gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
                }

                currentRow--;

                UpdateGridPosition(currentRow, currentColumn);
                gridManager.focusOnCurrentGrid(currentRow, currentColumn);
                StepSourceUpDown.Play();
            }
            else {
                BoundarySource.Play();
            }

        }

        // Move selection down.
        void MoveDown()
        {
            if (currentRow < gridManager.rows - 1)
            {
                if (numberOfSteps != 1)
                {
                    gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
                }

                currentRow++;
                UpdateGridPosition(currentRow, currentColumn);
                gridManager.focusOnCurrentGrid(currentRow, currentColumn);
                StepSourceUpDown.Play();
            }
            else {
                BoundarySource.Play();
            }
        }
        void loadMap(string mapID, int inRow, int inColumn, int outRow, int outColumn)
        {
            csvLoader.changeCSVFile(mapID);
            csvLoader.setMapPosition_In(inRow, inColumn);
            csvLoader.setMapPosition_Out(outRow, outColumn);
            uap.stopTalking();
            SceneManager.LoadScene(1); // loads the reset scene 

        }
        void returnToPreviousMap()
        {
            uap.Saysomething("Going back");
            csvLoader.setCurrentCSV(csvLoader.returnPreviousCSV());
            csvLoader.clearPreviousCSV();
            csvLoader.setMapPosition_In(csvLoader.zoomOutRow, csvLoader.zoomOutColumn);
            //csvLoader.changeCSVFile
           // csvLoader.previousCSV = null;
            //csvLoader.setMapPosition_In(csvLoader.zoomOutRow, csvLoader.zoomOutColumn);
            uap.stopTalking();
            SceneManager.LoadScene(1); // loads the reset scene 
        }

        // Update the grid selection based on the current row and column.
        void UpdateGridPosition(int row, int column)
        {
            numberOfSteps += 1;
            // Deselect the previous cube if needed (e.g., change its color).
            // Implement this based on your game's requirements.

            // Select the current cube (e.g., change its color).
            // Implement this based on your game's requirements.

            // Optionally, you can perform other actions related to grid navigation here.

            // Print the current grid position for testing.
            //Debug.Log("Current Grid Position: Row " + row + ", Column " + column);

        }
        public void speakAroundMe()
        {
            gridManager.GetNeighboringAndDiagonalRowsAndColumns(currentRow, currentColumn, out neighboringRows, out neighboringColumns);
            /*
            foreach (int itemRows in neighboringRows)
            {
                foreach (int itemCols in neighboringColumns)
                {
                    gridManager.speakAroundMe(itemRows, itemCols);
                    Debug.Log("Rows/ Cols: " + itemRows + "/ " + itemCols);
                }
            }*/
            StartCoroutine(waitSeconds());
        }

        public void speakLandmarks()
        {
            //healthcentre coordinates
            int landmark_row = 13;
            int landmark_col = 2;

            //calculate relative position of landmark compared to current location
            int row_dir = landmark_row - currentRow;
            int col_dir = landmark_col - currentColumn;
            LandmarkVect = new Vector3(col_dir, 0, row_dir);


            cTTS.transform.position = parentObject.position + LandmarkVect * soundDistance;
            gridManager.speakAroundMe(landmark_row, landmark_col, cTTS.transform);
        }

        public void speakCoordinates()
        {
            gridManager.CurrentGridCoord(currentRow, currentColumn);
        }

        IEnumerator waitSeconds()
        {
            for (int i = 0; i < 20; i++)
            {

                Debug.Log("Rows/ Cols: " + neighboringRows[i] + "/ " + neighboringColumns[i]);
                Debug.Log("i = " + i);
                //cTTS.transform.position = positions[i];
                gridManager.speakAroundMe(neighboringRows[i], neighboringColumns[i], cTTS.transform);
                yield return new WaitForSeconds(2);

            }
            //yield on a new YieldInstruction that waits for 2 seconds.
        }

        void CalculatePositions()
        {
            if (parentObject == null)
            {
                Debug.LogWarning("Parent Object not assigned.");
                return;
            }

            // Calculate positions relative to the parentObject.
            positions[0] = parentObject.position + Vector3.forward * soundDistance + Vector3.left * soundDistance; // Front Left
            positions[1] = parentObject.position + Vector3.forward * soundDistance; // Front
            positions[2] = parentObject.position + Vector3.forward * soundDistance + Vector3.right * soundDistance; // Front Right
            positions[3] = parentObject.position + Vector3.right * soundDistance; // Right
            positions[4] = parentObject.position + Vector3.back * soundDistance + Vector3.right * soundDistance; // Back Right
            positions[5] = parentObject.position + Vector3.back * soundDistance; // Back
            positions[6] = parentObject.position + Vector3.back * soundDistance + Vector3.left * soundDistance; // Back Left
            positions[7] = parentObject.position + Vector3.left * soundDistance; // Left

            // You can use the 'positions' array as needed.
            for (int i = 0; i < positions.Length; i++)
            {
                //Debug.Log("Position " + i + ": " + positions[i]);
            }
        }
        private System.Collections.IEnumerator ExecuteEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

        }
        // Function to compute the difference between two Vector2Int vectors
        private Vector2Int ComputeDifference(Vector2Int vectorA, Vector2Int vectorB)
        {
            return vectorA - vectorB;
        }
    /*
    void newHandleInput()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (inSubSublist)
            {
                // Navigate within the subsublist
                subSublistIndex = Mathf.Max(0, subSublistIndex - 1);
                // Assuming you have a way to access the current subsublist, similar to how subLists is accessed
                string currentSubSubListItem = GetCurrentSubSubListItem();
                uap.Saysomething(currentSubSubListItem);
                // Optionally, play boundary sound if at the start of the subsublist
            }
            else if (inSublist)
            {
                // Navigate within the sublist
                sublistIndex = Mathf.Max(0, sublistIndex - 1);
                uap.Saysomething(cvsRF.subLists[selectedMainItem][sublistIndex]);
                if (sublistIndex == 0)
                {
                    BoundarySource.Play();
                }
            }
            else
            {
                // Navigate within the main list
                mainListIndex = Mathf.Max(0, mainListIndex - 1);
                selectedMainItem = cvsRF.mainList[mainListIndex];
                uap.Saysomething(selectedMainItem);
                if (mainListIndex == 0)
                {
                    BoundarySource.Play();
                }
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (inSubSublist)
            {
                // Navigate within the subsublist
                // Similar navigation logic as above, adapted for subsublist
            }
            else if (inSublist)
            {
                // Navigate within the sublist
                if (cvsRF.subLists.ContainsKey(selectedMainItem) && sublistIndex < cvsRF.subLists[selectedMainItem].Count - 1)
                {
                    sublistIndex++;
                    uap.Saysomething(cvsRF.subLists[selectedMainItem][sublistIndex]);
                }
                else
                {
                    BoundarySource.Play();
                }
            }
            else
            {
                // Navigate within the main list
                if (mainListIndex < cvsRF.mainList.Count - 1)
                {
                    mainListIndex++;
                    selectedMainItem = cvsRF.mainList[mainListIndex];
                    uap.Saysomething(selectedMainItem);
                }
                else
                {
                    BoundarySource.Play();
                }
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (inSubSublist)
            {
                // Logic to handle deeper navigation if there's another level beyond subsublist
                // This is where you might handle actions within the deepest navigation level
            }
            else if (inSublist)
            {
                // Enter the subsublist
                EnterSubSublist(); // You'll need to implement this method
            }
            else if (!inSublist)
            {
                // Enter the sublist
                EnterSublist(); // Adapt your existing logic to enter the sublist here
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (inSubSublist)
            {
                // Exit the subsublist to the parent sublist
                ExitSubSublist(); // You'll need to implement this method
            }
            else if (inSublist)
            {
                // Exit the sublist to the main list
                ExitSublist(); // Adapt your existing logic to exit the sublist here
            }
            // Additional logic for navigating back from the sublist to the main list if needed
        }
        // Additional input handling as before
    }
    */
    void HandleInput()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Up pressed");
            if (inSublist)
            {
                // Navigate within the sublist
                sublistIndex = Mathf.Max(0, sublistIndex - 1);
                uap.Saysomething(cvsRF.subLists[selectedMainItem][sublistIndex]);
                if (sublistIndex == 0)
                {
                    BoundarySource.Play();
                }
            }
            else
            {
                // Navigate within the main list
                mainListIndex = Mathf.Max(0, mainListIndex - 1);
                selectedMainItem = cvsRF.mainList[mainListIndex];
                uap.Saysomething(selectedMainItem);
                if (mainListIndex == 0)
                {
                    BoundarySource.Play();
                }
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down pressed");

            if (inSublist)
            {
                // Navigate within the sublist
                if (cvsRF.subLists.ContainsKey(selectedMainItem) && sublistIndex < cvsRF.subLists[selectedMainItem].Count - 1)
                {
                    sublistIndex++;
                    uap.Saysomething(cvsRF.subLists[selectedMainItem][sublistIndex]);
                }
                else
                {
                    BoundarySource.Play();
                }
            }
            else
            {
                // Navigate within the main list
                if (mainListIndex < cvsRF.mainList.Count - 1)
                {
                    mainListIndex++;
                    selectedMainItem = cvsRF.mainList[mainListIndex];
                    uap.Saysomething(selectedMainItem);
                }
                else
                {
                    BoundarySource.Play();
                }
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right pressed");

            // Enter the sublist
            if (cvsRF.subLists.ContainsKey(selectedMainItem) && cvsRF.subLists[selectedMainItem].Count > 0)
            {
                inSublist = true;
                uap.Saysomething(cvsRF.mainList[mainListIndex] + " selected. First sub index: " + (cvsRF.subLists[selectedMainItem][sublistIndex]));
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left pressed");

            // Go back to the main list
            inSublist = false;
            sublistIndex = 0;
            uap.Saysomething(selectedMainItem); // Optional: Say something when going back to the main list
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && inSublist && !returnKeyPressed)
        {
            Tuple<int, int> tuple = cvsRF.subListPositions[cvsRF.subLists[selectedMainItem][sublistIndex]];

            // Access individual elements of the tuple
            row = tuple.Item1;
            column = tuple.Item2;

            // Now you can use 'row' and 'column' as needed
            ;
            Vector2Int differece = ComputeDifference(new Vector2Int(currentRow, currentColumn), new Vector2Int(row, column));
            Debug.Log(differece.ToString());
            string vertical;
            string horizontal;
            if (differece[0] <= 0)
            {
                vertical = "down";

            }
            else
            {
                vertical = "up";
            }

            if (differece[1] <= 0)
            {
                horizontal = "right";
            }
            else
            {
                horizontal = "left";
            }
            uap.Saysomething("The landmark " + cvsRF.subLists[selectedMainItem][sublistIndex] + " is " + System.Math.Abs(differece[0]) + " units " + vertical + " and " + System.Math.Abs(differece[1]) + " units" + horizontal + ". Press enter again to teleport to this position.");
            returnKeyPressed = true;
            mapItemAudioSource.transform.localPosition = new Vector3(differece[0], 0, differece[1]);
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && inSublist && returnKeyPressed)
        {
            gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(row, column);
            currentColumn = column;
            currentRow = row;
            returnKeyPressed = false;
            indexMode = false;
            uap.Saysomething("Teleporting to : " + cvsRF.subLists[selectedMainItem][sublistIndex]);

        }
    }
    void DisplayCurrentSelection()
        {
            if (inSublist)
            {
                Debug.Log($"Selected Sublist Item: {cvsRF.subLists[selectedMainItem][sublistIndex]}");
            }
            else
            {
                Debug.Log($"Selected Main Item: {selectedMainItem}");
            }
        }
        private void PlayAudioAndWait(AudioSource audioSource)
        {
            // Play the audio clip
            audioSource.Play();

            // Use Invoke to schedule stopping the audio after the clip length
            Invoke("StopAudio", audioSource.clip.length);
        }

        private void StopAudio()
        {
            // Stop the audio after the clip length
            StepSourceLeftRight.Stop();
        }
        // Function to play audio
        void PlayAudioLR()
        {
            PlayAudioAndWait(StepSourceLeftRight);
        }
        void PlayAudioUD()
        {
            PlayAudioAndWait(StepSourceUpDown);
        }
    }