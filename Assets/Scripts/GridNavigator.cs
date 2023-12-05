using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;


public class GridNavigator : MonoBehaviour
{
    public GridManager gridManager;  // Reference to your GridManager script.

    public SoundManager soundManager;  // Reference to your SoundManager script.
    public CVSReaderFinal cvsRF; 
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
    public AudioSource StepSource;
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
    public int mapItemIndex = 0;
    public string currentMapItem;
    public GameObject mapItemAudioSource;

    public AudioClip escalators;
    public AudioClip trainStation;


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
        AmbientVect= new Vector3 (0,0,0);
        //take difference between position on grid and coordinate of landmark
        StartCoroutine(ExecuteEndOfFrame());
        //soundManager = GameObject.Find("MainFrame and Controller").GetComponent<SoundManager>();


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
                uap.Saysomething("Index mode activated");
                mapItemIndex = 0;
            }
            else if (indexMode == true)
            {
                uap.Saysomething("Index mode deactivated");
                indexMode = false;
            }
        }


        if (indexMode ==false)
        {
            // Move the selection based on arrow key input.
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
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
        }
        else if(indexMode == true)
        {
            /*
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && mapItemIndex < cvsRF.mapItems.Count)
            {
                currentMapItem = cvsRF.mapItems[mapItemIndex];
                uap.Saysomething(currentMapItem);
                mapItemIndex += 1; 
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && mapItemIndex > 0)
            {
                mapItemIndex -= 1;
                currentMapItem = cvsRF.mapItems[mapItemIndex];
                uap.Saysomething(currentMapItem);

            }
            else if(UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                Vector2Int differece = ComputeDifference(new Vector2Int(currentRow, currentColumn), new Vector2Int(cvsRF.mapItemRow[mapItemIndex], cvsRF.mapItemCol[mapItemIndex]));
                Debug.Log(differece.ToString());
            }*/
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (mapItemIndex > 0)
                {
                    mapItemIndex--;
                    Debug.Log("Selected Item: " + cvsRF.mapItems[mapItemIndex]);
                    uap.Saysomething(cvsRF.mapItems[mapItemIndex]);

                }
            }

            // Move down through the list
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (mapItemIndex < cvsRF.mapItems.Count - 1)
                {
                    mapItemIndex++;
                    Debug.Log("Selected Item: " + cvsRF.mapItems[mapItemIndex]);
                    uap.Saysomething(cvsRF.mapItems[mapItemIndex]);

                }
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                Vector2Int differece = ComputeDifference(new Vector2Int(currentRow, currentColumn), new Vector2Int(cvsRF.mapItemRow[mapItemIndex], cvsRF.mapItemCol[mapItemIndex]));
                Debug.Log(differece.ToString());
                string vertical; 
                string horizontal;
                if (differece[0] <= 0)
                {
                    vertical = "down";

                }
                else
                {
                    vertical="up";
                }

                if (differece[1] <= 0) 
                {
                    horizontal = "right";
                }
                else
                {
                    horizontal ="left";
                }
                uap.Saysomething("The landmark is " + System.Math.Abs(differece[0]) + "units " + vertical + " and " + System.Math.Abs(differece[1]) + "units" + horizontal);
                mapItemAudioSource.transform.localPosition = new Vector3(differece[0], 0, differece[1]);
                if(cvsRF.mapItems[mapItemIndex] == "Escalator")
                {
                    mapItemAudioSource.GetComponent<AudioSource>().clip = escalators; 
                }
                else if(cvsRF.mapItems[mapItemIndex] == "Escalator")
                {
                    mapItemAudioSource.GetComponent<AudioSource>().clip = trainStation;
                }
                mapItemAudioSource.GetComponent<AudioSource>().Play();

            }
        }


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
            StepSource.transform.localPosition=RightSide;
            StepSource.Play();
            UpdateAllSoundManagers(currentRow, currentColumn);

        }
        else{
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
            StepSource.transform.localPosition=LeftSide;
            StepSource.Play();
            UpdateAllSoundManagers(currentRow, currentColumn);
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
            StepSource.Play();
            UpdateAllSoundManagers(currentRow, currentColumn);
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
            StepSource.Play();
            UpdateAllSoundManagers(currentRow, currentColumn);
        }
        else{
            BoundarySource.Play();
        }
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
    public void speakAmbientSounds()
    {
        /*make a list of all the cells that are assigned ambient sound, 
        for each ambientsound, calculate relative position
        if distance is less than 5 squares, speak sound.
        */
        //healthcentre coordinates
        int train_row=2;
        int train_col=10;

        //calculate relative position of landmark compared to current location
        int row_dir= train_row - currentRow;
        int col_dir= train_col - currentColumn;
        AmbientVect= new Vector3 (col_dir,0, row_dir);


        cTTS.transform.position= parentObject.position +  AmbientVect  * soundDistance;
        //gridManager.speakAroundMe(train_row, train_col, cTTS.transform);
        TrainSource.transform.localPosition=AmbientVect;
        TrainSource.Play(); //replace with train sound? idk each one has to be assigned seperately I guess
    }

    public void speakCoordinates()
    {
        gridManager.CurrentGridCoord(currentRow, currentColumn);
    }

    IEnumerator waitSeconds()
    {
        for (int i = 0; i < 8; i++)
        {

            //Debug.Log("Rows/ Cols: " + neighboringRows[i] + "/ " + neighboringColumns[i]);
            cTTS.transform.position = positions[i];
            gridManager.speakAroundMe(neighboringRows[i], neighboringColumns[i], cTTS.transform);
            yield return new WaitForSeconds(2);

        }
        //yield on a new YieldInstruction that waits for 2 seconds.
    }

    void CalculatePositions()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("ParentObject not assigned.");
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
        currentMapItem = cvsRF.mapItems[mapItemIndex];

    }
    // Function to compute the difference between two Vector2Int vectors
    private Vector2Int ComputeDifference(Vector2Int vectorA, Vector2Int vectorB)
    {
        return vectorA - vectorB;
    }

    public void UpdateAllSoundManagers(int currentRow, int currentColumn)
    {
        //for length of ambient sound list
        for (int i=0; i< cvsRF.ListofGridsWithAmbient.Count; i++)
        {
            cvsRF.ListofGridsWithAmbient[i].GetComponent<SoundManager>().UpdateSoundPosition(currentRow, currentColumn);
        }

    }
}