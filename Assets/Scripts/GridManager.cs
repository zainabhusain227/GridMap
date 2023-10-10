using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject[,] grid;    // 2D array to store cube game objects.
    public GameObject cubePrefab;  // The cube prefab you want to use.
    public int rows = 5;           // Number of rows in the grid.
    public int columns = 5;        // Number of columns in the grid.
    public string coord;        // Number of columns in the grid.
    public UAP_AccessibilityManager uap;

    public GameObject cTTS;



    public void Start()
    {
        CreateGrid();
        //StartCoroutine(resetGridPosition());
    }

    public void CreateGrid()
    {
        grid = new GameObject[rows, columns];

        // Calculate the size of each cube based on the canvas size and grid dimensions.
        float canvasWidth = GetComponent<RectTransform>().rect.width;
        float canvasHeight = GetComponent<RectTransform>().rect.height;
        float cubeSize = Mathf.Min(canvasWidth / columns, canvasHeight / rows);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Create a cube GameObject.
                GameObject cube = Instantiate(cubePrefab);
                cube.transform.SetParent(transform);  // Set the grid as the parent.

                // Set the position and size of the cube.
                RectTransform rectTransform = cube.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new UnityEngine.Vector2(cubeSize, cubeSize);
                rectTransform.anchoredPosition = new UnityEngine.Vector2(j * cubeSize, -i * cubeSize);

                // Attach a Text component to the cube.
                Text textComponent = cube.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = "Text Variable";  // Set your text variable here.
                }

                grid[i, j] = cube;  // Store the cube in the grid array.
            }
        }
    }
    public void SetText(int row, int column, string newText)
    {
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            grid[row, column].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newText;
            /*
            TextMeshProUGUI textMesh = grid[row, column].GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = newText;
            }
            */
        }
    }
    public void CurrentGridCoord(int row, int column)
    {
        grid[row, column].gameObject.GetComponent<Image>().color = Color.red;
        coord= column.ToString() + " " + row.ToString();
        uap.Saysomething(coord);
    }
    public void focusOnCurrentGrid(int row, int column)
    {
        grid[row, column].gameObject.GetComponent<Image>().color = Color.red;
        uap.Saysomething(grid[row, column].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }
    public void defocusOnPreviousGrid(int row, int column)
    {
        grid[row, column].gameObject.GetComponent<Image>().color = Color.white;
    }
    public void referenceModeFocusOnCurrentGrid(int row, int column)
    {
        grid[row, column].gameObject.GetComponent<Image>().color = Color.yellow;
        uap.Saysomething(grid[row, column].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }
    public void referenceModeDefocusOnCurrentGrid(int row, int column)
    {
        grid[row, column].gameObject.GetComponent<Image>().color = Color.white;
    }
    // Function to get neighboring and diagonal rows and columns around a given row and column.
    public void GetNeighboringAndDiagonalRowsAndColumns(int currentRow, int currentColumn, out int[] neighboringRows, out int[] neighboringColumns)
    {
        int[] rowOffsets = { -1, -1, -1, 0, 1, 1, 1, 0 }; // Up, Up-Right, Right, Down-Right, Down, Down-Left, Left, Up-Left
        int[] colOffsets = { -1, 0, 1, 1, 1, 0, -1, -1 }; // Up, Up-Right, Right, Down-Right, Down, Down-Left, Left, Up-Left

        int numNeighbors = rowOffsets.Length;

        neighboringRows = new int[numNeighbors];
        neighboringColumns = new int[numNeighbors];

        for (int i = 0; i < numNeighbors; i++)
        {
            neighboringRows[i] = currentRow + rowOffsets[i];
            neighboringColumns[i] = currentColumn + colOffsets[i];
        }
    }
    public void speakAroundMe(int row, int cols, Transform position)
    {
        if (row >= 0 && row < rows && cols >= 0 && cols < columns)
        {
            //uap.Saysomething(grid[row, cols].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            uap.Saysomething3D(grid[row, cols].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, position);
            Debug.Log(grid[row, cols].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        }

    }
    IEnumerator resetGridPosition()
    {
        {
            yield return new WaitForSeconds(1);
            //this.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            //this.gameObject.transform.position = new Vector3(800, 800, 0);

        }
        //yield on a new YieldInstruction that waits for 2 seconds.
    }

}
