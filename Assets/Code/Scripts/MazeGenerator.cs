using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class that creates a random maze within a 2D array
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    // <--------- CONSTANTS --------->

    /// <summary>
    /// Boolean that represents a wall on the boolean maze grid
    /// </summary>
    const bool Wall = true;

    /// <summary>
    /// Boolean that represents a passage on the boolean maze grid
    /// </summary>
    const bool Passage = false;

    // <--------- ATTRIBUTES --------->

    // ------- PUBLIC ATTRIBUTES ------- 

    /// <summary>
    /// Prefab asset that represents a wall
    /// </summary>
    public GameObject _wallPrefab;

    /// <summary>
    /// Width of the maze grid
    /// </summary>
    public int _mazeWidth = 21;

    /// <summary>
    /// Height of the maze grid
    /// </summary>
    public int _mazeHeight = 21;

    // ------- PRIVATE ATTRIBUTES -------

    /// <summary>
    /// 2D boolean array that represents the structure of the maze
    /// </summary>
    private bool[,] _mazeGrid;

    /// <summary>
    /// Structure of the maze made of walls (GameObject)
    /// </summary>
    private GameObject[,] _mazeStructure;


    // <---------- METHODS ---------->

    // -------- UNITY METHODS --------

    // Start is called before the first frame update
    void Start()
    {
        // At the start, generate the random maze
        GenerateNewRandomMaze();
    }

    // -------- PRIVATE METHODS ---------

    /// <summary>
    /// Generates a new random maze
    /// </summary>
    private void GenerateNewRandomMaze()
    {
        // Allocate in memory the two representations of the maze (boolean grid and gameObject structure)
        _mazeGrid = new bool[_mazeWidth, _mazeHeight];
        _mazeStructure = new GameObject[_mazeWidth, _mazeHeight];

        // Initialize the maze with walls
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                _mazeGrid[x, y] = Wall;
                _mazeStructure[x, y] = Instantiate(_wallPrefab, new Vector3(x * 5f, 0f, y * 5f), Quaternion.identity, GetComponent<Transform>());
            }
        }

        // A random cell is selected to be the origin of the generated maze, the initial passage cell
        int startCellX = Random.Range(3, _mazeWidth - 3);
        int startCellY = Random.Range(3, _mazeHeight - 3);
        _mazeGrid[startCellX, startCellY] = Passage;

        // Create a list to store all of the frontier cells and calculte the first ones of the intial passage
        HashSet<(int, int)> frontierCells = GetNeighborCells(startCellX, startCellY, true);

        // While there's frontier cells, continue creating new passages
        while (frontierCells.Any())
        {
            // Select a random frontier cell and change it into a passage
            int randomIndex = Random.Range(0, frontierCells.Count);
            (int, int) randomFrontierCell = frontierCells.ElementAt(randomIndex);
            int randomFrontierCellX = randomFrontierCell.Item1;
            int randomFrontierCellY = randomFrontierCell.Item2;
            _mazeGrid[randomFrontierCellX, randomFrontierCellY] = Passage;

            // Obtain the list of passage cells which can be connected to the selected frontier cell
            HashSet<(int, int)> candidateCells = GetNeighborCells(randomFrontierCellX, randomFrontierCellY, false);
            if (candidateCells.Any()) // safety statement
            {
                // Select a random passage to connect with the frontier cell
                int randomIndexCandidate = Random.Range(0, candidateCells.Count);
                (int, int) randomCellConnection = candidateCells.ElementAt(randomIndexCandidate);
                int randomCellConnectionX = randomCellConnection.Item1;
                int randomCellConnectionY = randomCellConnection.Item2;

                // Calculate which cell is inbetween the frontier cell and the passage
                (int, int) cellBetween;
                if (randomFrontierCellX < randomCellConnectionX)
                    cellBetween = (randomFrontierCellX + 1, randomFrontierCellY);
                else if (randomFrontierCellX > randomCellConnectionX)
                    cellBetween = (randomFrontierCellX - 1, randomFrontierCellY);
                else
                {
                    if (randomFrontierCellY < randomCellConnectionY)
                        cellBetween = (randomFrontierCellX, randomFrontierCellY + 1);
                    else
                        cellBetween = (randomFrontierCellX, randomFrontierCellY - 1);
                }

                // Make the cell in between a passage to connect the frontier cell and passage cell
                _mazeGrid[cellBetween.Item1, cellBetween.Item2] = Passage;
            }

            // Remove the frontier cell that has been converted to a passage 
            frontierCells.Remove(randomFrontierCell);

            // Calculate the frontier cells from the previous frontier cell selected
            frontierCells.UnionWith(GetNeighborCells(randomFrontierCellX, randomFrontierCellY, true));
        }

        // Deactivate the GameObjects of the walls that are a passage
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int y = 0; y < _mazeHeight; y++)
            {
                if (_mazeGrid[x, y] == Passage)
                    _mazeStructure[x, y].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Obtain a group of a certain neighboring cell type of a given cell depending on the boolean parameter
    /// </summary>
    /// <remarks> Types of cell: Frontier cells = a wall cell at a distance of 2 units on a single axis from another cell. | 
    /// Possible passages = a passage cell at a distance of 2 units on a single axis from another cell. </remarks>
    /// <param name="x">Coordinate X of the cell</param>
    /// <param name="y">Coordinate Y of the cell</param>
    /// <param name="checkFrontier">True if the hash set will contain the frontier cells; false for possible passages</param>
    /// <returns>Hash set of tuples (X and Y coordinates) representing the frontier cells or possible passages</returns>
    private HashSet<(int, int)> GetNeighborCells(int x, int y, bool checkFrontier)
    {
        HashSet<(int, int)> newNeighborCells = new HashSet<(int, int)>();

        // Check if the given cell can have a neighbor cell in the different axis
        // A neighbor cell can't be outside the boundaries of the 2d array or belong to the outer edge.
        // It also makes a different check depending on whether we are looking for the frontier cells or the possible passage cells 
        if (x > 2)
        {
            (int, int) cellToCheck = (x - 2, y);

            if (checkFrontier ? _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Wall : _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Passage)
            {
                newNeighborCells.Add(cellToCheck);
            }
        }
        if (x < _mazeWidth - 3)
        {
            (int, int) cellToCheck = (x + 2, y);
            if (checkFrontier ? _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Wall : _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Passage)
            {
                newNeighborCells.Add(cellToCheck);
            }
        }

        if (y > 2)
        {
            (int, int) cellToCheck = (x, y - 2);
            if (checkFrontier ? _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Wall : _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Passage)
            {
                newNeighborCells.Add(cellToCheck);
            }
        }
        if (y < _mazeHeight - 3)
        {
            (int, int) cellToCheck = (x, y + 2);
            if (checkFrontier ? _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Wall : _mazeGrid[cellToCheck.Item1, cellToCheck.Item2] == Passage)
            {
                newNeighborCells.Add(cellToCheck);
            }
        }

        return newNeighborCells;
    }
}