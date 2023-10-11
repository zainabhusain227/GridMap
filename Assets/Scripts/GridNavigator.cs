using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;


public class GridNavigator : MonoBehaviour
{
    public GridManager gridManager;  // Reference to your GridManager script.
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

    void Start()
    {
        // Initialize the starting position.
        UpdateGridPosition(currentRow, currentColumn);
        CalculatePositions();

    }

    void Update()
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
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
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
            StepSource.Play();

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
            StepSource.Play();
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
}