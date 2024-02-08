using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System;

public class GridNavigator : MonoBehaviour
{
    public GridManager gridManager;  // Reference to your GridManager script.
    public SoundManager soundManager;  // Reference to your SoundManager script.
    public CSVReaderFinal cvsRF; 
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
    public AudioSource TrainSource;

    public Vector3 LeftSide;
    public Vector3 RightSide;
    public Vector3 ForwardSide;
    public Vector3 BackSide;
    public Vector3 Center;

    public Vector3 LandmarkVect;
    public Vector3 AmbientVect;

    public bool indexMode = false;
    public bool subIndexMode = false;
    public bool MuteMode = false;
    public int mapItemIndex = 0;




    public string currentMapItem;
    public GameObject mapItemAudioSource;

    public AudioClip escalators;
    public AudioClip trainStation;

    public AudioClip train_station_ambient;
    public AudioClip bank_booth;
    public AudioClip elevator;
    public AudioClip escalator;
    public AudioClip mall_sound;
    public AudioClip street_bg;
    public AudioClip ticket_counter;
    public AudioClip ticket_scan_main;
    public AudioClip washroom_ambient;



    int itemRow;
    int itemCol;

    int row;
    int column; 

    private int mainListIndex = 0;
    private string selectedMainItem;

    private int sublistIndex = 0;
    private string selectedSubItem;
    private bool inSublist = false;

    private bool returnKeyPressed = false;

    void Start()
    {
        // Initialize the starting position.
        UpdateGridPosition(currentRow, currentColumn);
        CalculatePositions();

        LeftSide = new Vector3 (-10,0,0);
        RightSide= new Vector3 (10,0,0);
        ForwardSide= new Vector3 (0,0,10);
        BackSide= new Vector3 (0,0,-10);
        Center= new Vector3 (0,0,0);

        LandmarkVect= new Vector3 (0,0,0);
        AmbientVect = new Vector3(0, 0, 0);
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
            else if (UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                speakAmbientSounds();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                ToggleAmbientSounds();
            }
        }
        else if(indexMode == true)
        {
            HandleInput();
            //DisplayCurrentSelection();
        }

    }   
    /*
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
            PlayAudioAndWait(StepSourceLeftRight);

            // Check if the next grid cell has a different text
            if (gridManager.grid[currentRow, currentColumn].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != currentString)
            {
                break;
            }
        }

        UpdateGridPosition(currentRow, currentColumn);
        gridManager.focusOnCurrentGrid(currentRow, currentColumn);
    }
    */
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
            //UpdateAllSoundManagers(currentRow, currentColumn);

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
            StepSourceLeftRight.transform.localPosition=LeftSide;
            StepSourceLeftRight.Play();
            //UpdateAllSoundManagers(currentRow, currentColumn);
        }
        else{
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
            //UpdateAllSoundManagers(currentRow, currentColumn);
        }
        else{
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
            //UpdateAllSoundManagers(currentRow, currentColumn);
        }
        else{
            BoundarySource.Play();
        }
    }

    // Update the grid selection based on the current row and column.
    void UpdateGridPosition(int row, int column)
    {
        numberOfSteps += 1;
        //UpdateAllSoundManagers(row, column);
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
        int landmark_row=13;
        int landmark_col=2;

        //calculate relative position of landmark compared to current location
        int row_dir= landmark_row - currentRow;
        int col_dir= landmark_col - currentColumn;
        LandmarkVect= new Vector3 (col_dir,0, row_dir);


        cTTS.transform.position= parentObject.position +  LandmarkVect  * soundDistance;
        gridManager.speakAroundMe(landmark_row, landmark_col, cTTS.transform);
        //gridManager.CurrentGridCoord(row, col);

        //gridManager.GetNeighboringAndDiagonalRowsAndColumns(currentRow, currentColumn, out neighboringRows, out neighboringColumns);
        
        /*
        foreach (int itemRows in neighboringRows)
        {
            foreach (int itemCols in neighboringColumns)
            {
                gridManager.speakAroundMe(itemRows, itemCols);
                Debug.Log("Rows/ Cols: " + itemRows + "/ " + itemCols);
            }
        }*/
        //StartCoroutine(waitSeconds());

        //hardcore coordinates for health centre for now
        //int row=13;
        //int col=2;
        //gridManager.focusOnCurrentGrid(row, col);
        //gridManager.CurrentGridCoord(row, col);
        //gridManager.GetNeighboringAndDiagonalRowsAndColumns(currentRow, currentColumn, out neighboringRows, out neighboringColumns);
        //StartCoroutine(waitSeconds());
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
            yield return new WaitForSeconds(1);

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
        yield return new WaitForEndOfFrame(); // Waits until the end of the current frame
        //currentMapItem = cvsRF.mapItems[mapItemIndex];

    }
    // Function to compute the difference between two Vector2Int vectors
    private Vector2Int ComputeDifference(Vector2Int vectorA, Vector2Int vectorB)
    {
        return vectorA - vectorB;
    }
   
    void HandleInput()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (inSublist)
            {
                // Navigate within the sublist
                sublistIndex = Mathf.Max(0, sublistIndex - 1);
                uap.Saysomething(cvsRF.subLists[selectedMainItem][sublistIndex]);
                if(sublistIndex == 0)
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
            // Enter the sublist
            if (cvsRF.subLists.ContainsKey(selectedMainItem) && cvsRF.subLists[selectedMainItem].Count > 0)
            {
                inSublist = true;
                uap.Saysomething(cvsRF.mainList[mainListIndex] + " selected. First sub index: " + (cvsRF.subLists[selectedMainItem][sublistIndex]));
            }
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
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
            uap.Saysomething("The landmark " + cvsRF.subLists[selectedMainItem][sublistIndex] + " is " + System.Math.Abs(differece[0]) + " units " + vertical + " and " + System.Math.Abs(differece[1]) + " units" + horizontal +". Press enter again to teleport to this position.");
            returnKeyPressed = true;
            mapItemAudioSource.transform.localPosition = new Vector3(differece[0], 0, differece[1]);
        }
        else if(UnityEngine.Input.GetKeyDown(KeyCode.Return) && inSublist && returnKeyPressed)
        {
            gridManager.defocusOnPreviousGrid(currentRow, currentColumn);
            gridManager.focusOnCurrentGrid(row, column);
            currentColumn = column;
            currentRow = row;
            returnKeyPressed=false;
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

    public void speakAmbientSounds()
    {
        /*make a list of all the cells that are assigned ambient sound, 
        for each ambientsound, calculate relative position
        if distance is less than 5 squares, speak sound.
        */
        //healthcentre coordinates
        int train_row = 2;
        int train_col = 10;

        //calculate relative position of landmark compared to current location
        int row_dir = train_row - currentRow;
        int col_dir = train_col - currentColumn;
        AmbientVect = new Vector3(col_dir, 0, row_dir);


        cTTS.transform.position = parentObject.position + AmbientVect * soundDistance;
        //gridManager.speakAroundMe(train_row, train_col, cTTS.transform);
        TrainSource.transform.localPosition = AmbientVect;
        TrainSource.Play(); //replace with train sound? idk each one has to be assigned seperately I guess
    }

    public void UpdateAllSoundManagers(int currentRow, int currentColumn)
    {
        //for length of ambient sound list
        for (int i = 0; i < cvsRF.ListofGridsWithAmbient.Count; i++)
        {
            cvsRF.ListofGridsWithAmbient[i].GetComponent<SoundManager>().UpdateSoundPosition(currentRow, currentColumn);
        }

    }

    public void ToggleAmbientSounds()
    {
        if (MuteMode== true)
        {
            for (int i = 0; i < cvsRF.ListofGridsWithAmbient.Count; i++)
            {
                cvsRF.ListofGridsWithAmbient[i].GetComponent<AudioSource>().Stop();
            }
            MuteMode = false;
        }
        else if (MuteMode == false)
        {
            for (int i = 0; i < cvsRF.ListofGridsWithAmbient.Count; i++)
            {
                cvsRF.ListofGridsWithAmbient[i].GetComponent<AudioSource>().Play();
            }
            MuteMode = true;
        }
    }
}
