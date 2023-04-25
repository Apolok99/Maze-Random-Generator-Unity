
![Maze](https://user-images.githubusercontent.com/62213937/234410011-0cd4ac6d-ca16-4387-8e48-5c4abe584539.PNG)

# Maze Random Generator Unity with Prim's Algorithm
 Simple Unity project that creates a random maze with boxes as walls using a version of Prim's Algorithm.

## Installation

Download the project and add it to the Unity Hub. The only requirement is to have the Unity version **2021.3.21f1**.
    
## Project Layout
In this Unity project, the following folders have the following content:

- **Code**
    - **Scripts**
        - **MazeGenerator.cs** : Script that handles the creation of the random maze every time the project is played.
- **Prefabs**
    - **Wall:** A rectangular cuboid (a bulit-in 3D box enlarged) with a third party texture.
- **Scenes**
    - **Demo:** Simple scene where you can see the algorithm in action.
- **Third Party**
    - **Ciathyza:** [Gridbox Prototype Materials](https://assetstore.unity.com/packages/2d/textures-materials/gridbox-prototype-materials-129127)

## Demo

In the demo there is a camera above the random maze generated to see the results.

## Prim's Algorithm Modifications
Prim's algorithm is a greedy algorithm for finding a minimum spanning tree for a weighted undirected graph. If we applie this idea to a randomly weighted grid graph, the algorithm can be modified to generate a random maze. In this modification, the graph with vertices it changes to a "cell/square" in the maze. By randomizing the weights between cells, the minimum spanning tree will resemble a maze.

**The final algorithm has the following structure:**

(The maze consists of a 2 dimensional array of cells, where a cell has 2 states: `Wall` or `Passage`)
1. Begin with all cells being `Walls`
2. A random cell is selected `(x, y)` and set it to be a `passage` and the initial cell of the maze
3. Get the frontiers cells of the initial cell and add them to a set `s` that contains all frontier cells. The frontier cells of a cell are all cells walls within an exact distance of two, diagonals excluded

![Frontier](https://user-images.githubusercontent.com/62213937/234407861-16f18ae1-b189-4ccd-87cb-158a0fdade57.png)

4. While there's frontier cells in `s`:
- Selecte a random cell `(x, y)` from `s`, make it a `passage` and remove it from `s`
- Get his neighbours (`passage` cells at an exact distance of two, diagonals excluded) and store it in a set `ns`

![Neighbours](https://user-images.githubusercontent.com/62213937/234407941-5e6f3db1-cbe0-41cb-9b0b-1e98aeff9bf0.png)

- Make the cell between `(x, y)` and a random neighbour `(nx, ny)` a `passage`

![Connect](https://user-images.githubusercontent.com/62213937/234407995-e02a7bf0-2e2b-4187-ad06-82b24a95cf54.png)

(Images from [Arne Stenkrona](https://github.com/ArneStenkrona/MazeFun))

- Add the frontier cells of `(x, y)` (if any) to `s`

## Limitations
The maze generated has some peculiar characteristics.
1. **Outer edge wall:** The edge of the maze will never have a passage, meaning that the maze has neither exit nor entrance (the script must be modified for this).
2. **Maze size and initial cell:** Depending on the initial cell and the size of the maze, an entire column or row may be in a wall state. This is because the frontier cells are at a distance of two, and if a path is generated in the row or column before the outer wall this path will never reach that row or column.
3. **No loop paths:** The algorithm only creates short dead ends.

## Acknowledgements
 - [Maze Generation: Prim's Algorithm](https://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm)



## License

This project is licensed under the [MIT](https://choosealicense.com/licenses/mit/) License - see the LICENSE.md file for details 

