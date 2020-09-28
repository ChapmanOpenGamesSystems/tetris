using System.Collections;
using System.Collections.Generic;
using UnityEngine;





/// <summary>
/// Created by Sean Robbins
/// Date: 9/16/2020
/// 
/// This is the class that manages how the tiles move on the Grid
/// 
/// </summary>

public class GridManager : MonoBehaviour
{
    #region Attributes
    // grid[i][j], i is row , j is column

    // index [0][0] is the lower left corner of the grid


    [Tooltip("The 2d array of tiles that the map is played on")]
    public GridSpace[][] grid;

    [Tooltip("This variable is true when the player is in control of falling tiles, If it is true update loop does not check board tile status")]
    public bool PlayerPhase;

    [Tooltip("This variable tells the update loop when there are stable tiles in need of removal")]
    public bool TilesToRemove;

    [Tooltip("This variable tells the update loop when there are stable tiles that need to fall")]
    public bool TilesFalling;

    [Tooltip("This variable stores the row index for the center player controlled tile that is used as a pivot")]
    public int playerTilesCenterRow;

    [Tooltip("this variable stores the col index for the center player controlled tile that is used as a pivot")]
    public int playerTilesCenterCol;

    [Tooltip("this bool variable tells the flags when the player controlled tiles should cease moving and become stable")]
    public bool playerTilesStabalized;

    [Tooltip("This List of Coordinate Objects contains the locations of the player controlled tiles in the array")]
    public List<Coordinate> playerTiles;

    public OutputManager outputStream;

    public float timer;

    public float normalTime;

    public float fastTime;

    public bool canRotate;

    public bool shapeCanMove;

    public int currentRotation;

    #endregion




    #region Main Methods

    public void Awake()
    {
        timer = 0;
        grid = new GridSpace[outputStream.boardHeight][];
        for (int i = 0; i < grid.Length; ++i)
        {
            grid[i] = new GridSpace[outputStream.boardWidth];
        }
        PlayerPhase = true;
    }

    public void Update()
    {
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        if(PlayerPhase)
        {

            if (GameManager.Instance.horizontalMovement != 0 && GameManager.Instance.prevHorizontalMovement == 0 && !playerTilesStabalized)
            {
                MovePlayerTiles();
            }

            if(GameManager.Instance.rotate)
            {
                Debug.Log("Rotate Called");
                RotateTiles();
                GameManager.Instance.rotate = false;
            }


            //This section calls the method that determines how the 
            //player controlled tiles move through the grid
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                if ((timer += Time.deltaTime) > fastTime)
                {
                    UpdatePlayerControlledTiles();
                    timer = 0;
                }
            }
            else
            {
                if ((timer += Time.deltaTime) > normalTime)
                {
                    UpdatePlayerControlledTiles();
                    timer = 0;
                }
            }
        }
        else
        {
            //If there are no player controlled tiles on the grid 
            //check for filled rows in the rest of the grid
            UpdateStaticGridSpaces();
            UpdateStaticGridSpaces();
            GameManager.Instance.PlayerTurn = true;
            PlayerPhase = true;
        }
    }

    public void StoreGridSpaces(int i, int j, GameObject r)
    {
        grid[i][j] = r.GetComponent<GridSpace>();
    }


    /// <summary>
    /// This method checks to see if the given coordinates (rowIndex,colIndex) point to a
    /// valid location on the arrayand can thus be accessed
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public bool isValidTile(int i, int j)
    {
        if (i > 0 && i < grid.Length)
        {
            if (j >= 0 && j < grid[i].Length)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Player Controlled Tile Methods

    /// <summary>
    /// This Method checks to see if there is any player tiles in play,
    /// If there are none it generates new player tiles,
    /// if there are tiles in play it checks if they are stabalized,
    /// and if they are not stable it has them moved
    /// </summary>
    public void UpdatePlayerControlledTiles()
    {
        if(playerTiles == null || playerTiles.Count == 0)
        {
            GenerateNewPlayerTiles();
        }
        else if(playerTilesStabalized)
        {
            Stabalize();
        }
        else
        {
            CheckPlayerTiles();
        }
    }

    /// <summary>
    /// This method generates a new set of player 
    /// controlled tiles at the top of the grid 
    /// </summary>
    public void GenerateNewPlayerTiles()
    {
        PrefabShape tempShape = Instantiate(GameManager.Instance.GetShapeToGenerate()).GetComponent<PrefabShape>();
        if(playerTiles != null)
        {
            playerTiles.Clear();
        }

        playerTiles = tempShape.shapeInfo;
        canRotate = tempShape.enableRotate;
        currentRotation = tempShape.rotateState;
        
        foreach (Coordinate c in playerTiles)
        {
            if(c.isCenter)
            {
                c.row = outputStream.GenerateRow;
                c.col = outputStream.boardWidth / 2;
            }
            else
            {
                c.row = outputStream.GenerateRow + c.row;
                c.col = (outputStream.boardWidth / 2) + c.col;
            }
            grid[c.row][c.col].GainTile(c.tile);
        }
    }



    /// <summary>
    /// This method stabalizes all the player tiles when 
    /// they have been determined to be in their final positions
    /// 
    /// After completing this it empties the list of player controlled tile 
    /// coordinates in preperation for the next round of play
    /// </summary>
    public void Stabalize()
    {
        foreach (Coordinate c in playerTiles)
        {
            if(isValidTile(c.row,c.col))
            {
                grid[c.row][c.col].StabalizeTile();
                if(c.row == outputStream.GenerateRow)
                {
                    GameManager.Instance.Quit();
                }
            }
        }
        playerTiles.Clear();
        playerTiles = null;
        playerTilesStabalized = false;
        PlayerPhase = false;
    }

    #region MovePlayerTiles Methods

    /// <summary>
    /// This Method controlls the order in which movement is done on player controlled tiles
    /// 
    /// They move horizontally first based on player input and then vertically
    /// </summary>
    public void MovePlayerTiles()
    {
        shapeCanMove = true;
        if(playerTiles == null)
        {
            return;
        }
        else
        {
            foreach(Coordinate c in playerTiles)
            {
                if (CheckHorizontalTile(c.row, c.col) <= 0)
                {
                    c.canNotMove = true;
                    shapeCanMove = false;
                }
                if (CheckPlayerTile(c.row, c.col) <= 0)
                {
                    c.canNotMove = true;
                    shapeCanMove = false;
                }
                else
                {
                    c.canNotMove = false;
                }
                c.hasMoved = false;
            }
        }
        if (GameManager.Instance.horizontalMovement != 0)
        {
            foreach (Coordinate c in playerTiles)
            {
                if (GameManager.Instance.horizontalMovement != 0 && GameManager.Instance.prevHorizontalMovement == 0)
                {
                    MovePlayerTileHorizontal(c);
                }
                else
                {
                    MovePlayerTileDown(c);
                }
            }
        }
        else
        {
            foreach(Coordinate c in playerTiles)
            {
                MovePlayerTileDown(c);
            }
        }
    }

    /// <summary>
    /// This method is called in order to move a specific player tile down one row
    /// Before movement it checks to see if the selected movement is valid and then moves
    /// the tile and updates the coordinate list
    /// </summary>
    /// <param name="c"></param>
    public void MovePlayerTileDown(Coordinate c)
    {
        if (CheckPlayerTile(c.row, c.col) == 2 && !c.hasMoved)
        {
            for (int i = 0; i < playerTiles.Count; ++i)
            {
                if (c.row - 1 == playerTiles[i].row && c.col == playerTiles[i].col)
                {
                    MovePlayerTileDown(playerTiles[i]);
                }
            }
        }
        if (isValidTile(c.row - 1, c.col) && !c.hasMoved)
        {
            grid[c.row - 1 ][c.col].GainTile(grid[c.row][c.col].GiveTile());
            c.row = c.row - 1;
            c.hasMoved = true;
        }
    }

    /// <summary>
    /// This method moves a single player tile horizontally based on player input
    /// It checks if the move is valid first, then moves the tile and updates the 
    /// coordinate list
    /// </summary>
    /// <param name="c"></param>
    public void MovePlayerTileHorizontal(Coordinate c)
    {
        if (GameManager.Instance.horizontalMovement > 0)
        {
            if (CheckHorizontalTile(c.row, c.col) == 2 && !c.hasMoved && shapeCanMove)
            {
                for (int i = 0; i < playerTiles.Count; ++i)
                {
                    if (c.row == playerTiles[i].row && c.col + 1 == playerTiles[i].col)
                    {
                        MovePlayerTileHorizontal(playerTiles[i]);
                    }
                }
            }
            if (isValidTile(c.row, c.col + 1) && CheckHorizontalTile(c.row,c.col) > 0 && !c.hasMoved && shapeCanMove)
            {
                grid[c.row][c.col + 1].GainTile(grid[c.row][c.col].GiveTile());
                c.col = c.col + 1;
                c.hasMoved = true;
            }
        }
        else
        {
            if (CheckHorizontalTile(c.row, c.col) == 2 && !c.hasMoved && shapeCanMove)
            {
                for (int i = 0; i < playerTiles.Count; ++i)
                {
                    if (c.row == playerTiles[i].row && c.col - 1 == playerTiles[i].col)
                    {
                        MovePlayerTileHorizontal(playerTiles[i]);
                    }
                }
            }
            if (isValidTile(c.row, c.col - 1) && CheckHorizontalTile(c.row, c.col) > 0 && !c.hasMoved && shapeCanMove)

            {
                grid[c.row][c.col - 1].GainTile(grid[c.row][c.col].GiveTile());
                c.col = c.col - 1;
                c.hasMoved = true;
            }
        }
    }


    public void RotateTiles()
    {
        if(!canRotate)
        {
            return;
        }
        int centerRow = 0;
        int centerCol = 0;
        for(int i = 0; i < playerTiles.Count; ++i)
        {
            if(playerTiles[i].isCenter)
            {
                centerRow = playerTiles[i].row;
                centerCol = playerTiles[i].col;
                continue;
            }
            else
            {
                int startPosRow = playerTiles[i].row;
                int startPosCol = playerTiles[i].col;
                Vector2 temp = new Vector2(playerTiles[i].row - centerRow, playerTiles[i].col - centerCol);
                switch (playerTiles[i].rotateState)
                {
                    case 0:
                        temp = new Vector2(-temp.y, -temp.x);
                        playerTiles[i].rotateState = 90;
                        Debug.Log("Case 0");
                        break;
                    case 90:
                        temp = new Vector2(-temp.y,temp.x);
                        playerTiles[i].rotateState = 180;
                        Debug.Log("Case 90");

                        break;
                    case 180:
                        temp = new Vector2(-temp.y,-temp.x);
                        playerTiles[i].rotateState = 270;
                        Debug.Log("Case 180");

                        break;
                    case 270:
                        temp = new Vector2(-temp.y,temp.x);
                        playerTiles[i].rotateState = 0;
                        Debug.Log("Case 270");
                        break;
                }
                playerTiles[i].row = centerRow + (int)temp.x;
                playerTiles[i].col = centerCol + (int)temp.y;
                Debug.Log("CenterRow" + centerRow);
                Debug.Log("CenterCol" + centerCol);
                Debug.Log("StartPosRow" + startPosRow);
                Debug.Log("StartPosCol" + startPosCol);
                Debug.Log("PlayerTileRow" + playerTiles[i].row);
                Debug.Log("PlayerTileCol" + playerTiles[i].col);

                grid[playerTiles[i].row][playerTiles[i].col].StoreTileRotation(grid[startPosRow][startPosCol].GiveTile());
            }
        }
        foreach(Coordinate c in playerTiles)
        {
            if(!c.isCenter)
            {
                grid[c.row][c.col].PerformRotation();
            }
        }
    }

    #endregion



    #region CheckTile Methods

    /// <summary>
    /// This method checks to see if the player tiles are stabalized in there current positions,
    /// If they are it flags this so that in the next update the method above will recognize this,
    /// If they are not stable it triggers their next movement
    /// </summary>
    public void CheckPlayerTiles()
    {
        foreach (Coordinate c in playerTiles)
        {
            if (CheckPlayerTile(c.row, c.col) <= 0)
            {
                playerTilesStabalized = true;
            }
        }
        if (!playerTilesStabalized)
        {
            MovePlayerTiles();
        }

    }
    public int CheckHorizontalTile(int i, int j)
    {
        if (isValidTile(i, j))
        {
            if (grid[i][j].CheckPlayerControlled())
            {
                if (isValidTile(i , j + (int )GameManager.Instance.horizontalMovement))
                {
                    if (grid[i][j + (int)GameManager.Instance.horizontalMovement].CheckPlayerControlled())
                    {
                        return Target_Tile_Player_Controlled;
                    }
                    if (!grid[i][j + (int)GameManager.Instance.horizontalMovement].CheckPlayerControlled() && grid[i][j + (int)GameManager.Instance.horizontalMovement].CheckTile())
                    {
                        return TILE_CANNOT_FALL;
                    }
                    else
                    {

                        return TILE_CAN_FALL;
                    }
                }
                else
                {

                    return TILE_CANNOT_FALL;
                }
            }
            else
            {
                return TILE_NOT_PLAYER_CONTROLED;
            }
        }
        return TILE_NOT_VALID;
    }

    [Tooltip("This constant is the code for if a tile is not player controlled, returned by the CheckPlayerTile method")]
    public const int TILE_NOT_PLAYER_CONTROLED = -1;

    [Tooltip("This constant is the code for if a tile is not valid, returned by the CheckPlayerTile method")]
    public const int TILE_NOT_VALID = -2;

    [Tooltip("This constant is the code for if a tile cannot move down, returned by the CheckPlayerTile method")]
    public const int TILE_CANNOT_FALL = 0;

    [Tooltip("This constant is the code for if a tile can move down, returned by the CheckPlayerTile method")]
    public const int TILE_CAN_FALL = 1;

    [Tooltip("This constant is the code for if a tile can move down, returned by the CheckPlayerTile method")]
    public const int Target_Tile_Player_Controlled = 2;

    /// <summary>
    /// This method takes a row and column coordinate set in the form (int rowIndex, int ColIndex) and
    /// returns an Interger Code based on what the tile given has a status of
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public int CheckPlayerTile(int i, int j)
    {
        if ( isValidTile(i,j))
        {
            if(grid[i][j].CheckPlayerControlled())
            {
                if(isValidTile(i-1,j))
                {
                    if (grid[i - 1][j].CheckPlayerControlled())
                    {
                        return Target_Tile_Player_Controlled;
                    }
                    if (!grid[i - 1][j].CheckPlayerControlled() && grid[i - 1][j].CheckTile())
                    {
                        return TILE_CANNOT_FALL;
                    }
                    else
                    {
                        
                        return TILE_CAN_FALL;
                    }
                }
                else
                {
                    return TILE_CANNOT_FALL;
                }
            }
            else
            {
                return TILE_NOT_PLAYER_CONTROLED;
            }
        }
        return TILE_NOT_VALID;
    }

    #endregion

    #endregion


    #region Stable Tile Methods

    /// <summary>
    /// This method updates every stable tile on the grid
    /// it first checks if rows should be removed
    /// It will change the GameManager variable PlayerTurn to true when it has evaluated everything
    /// </summary>
    public void UpdateStaticGridSpaces()
    {
        if (TilesToRemove)
        {
            RemoveTiles();
            MoveStaticTilesDown();
        }
        
        else
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (CheckRowFilled(i))
                {
                    FlagRowRemoval(i);
                }
            }
        }
    }

    /// <summary>
    /// This Method parses through the grid and removes everytile that has been flagged for removal
    /// After it finishes this it sets the flag to have the static tile positions updated on the array
    /// by flagging TilesFalling to True
    /// </summary>
    public void RemoveTiles()
    {
        for (int i = 0; i < grid.Length; ++i)
        {
            foreach (GridSpace g in grid[i])
            {
                g.RemoveTile();
            }
        }

        TilesToRemove = false;
        TilesFalling = true;
    }

    /// <summary>
    /// This method parses through the grid and moves every tile flagged for movement down
    /// It parses from the bottom of the array at row 0 up and moves each tile flagged for falling the maximum amount it
    /// can before moving on to the next tile
    /// </summary>
    public void MoveStaticTilesDown()
    {
        for (int i = 0; i < grid.Length; ++i)
        {
            for(int j = 0; j < grid[i].Length; ++j)
            {
                MoveTile(i, j);
            }
        }
    }

    /// <summary>
    /// This Recursive Method moves a tile down one space every time it is called
    /// it is blocked from moving down by a tile in the row below it
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void MoveTile(int i, int j)
    {
        if (i == 0)
        {
            grid[i][j].StopFalling();
        }
        else if (grid[i][j].CheckFalling())
        {
            if (grid[i - 1][j].CheckTile())
            {
                grid[i][j].StopFalling();
            }
            else
            {
                grid[i - 1][j].GainTile(grid[i][j].GiveTile());
            }
        }
    }

   

    /// <summary>
    /// This is a method to check if a row of the grid array is filled
    /// and thus needs to be deleted
    /// 
    /// It parses down the array checking if each grid space is empty, If one is
    /// then it evaluates the row as incomplete and returns false
    /// 
    /// If it does not find any GridSpaces with null Tile Objects then it assumes
    /// that the whole row is filled and returns true
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool CheckRowFilled(int rowIndex)
    {
        foreach(GridSpace g in grid[rowIndex])
        {
            if(!g.CheckTile())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// This method parses down a row of the Grid array based on a passed in index,
    /// It flags every gridspace object in the row for removal in the Update Loop
    /// After it completes it calls FlagWillFall using the rowIndex passed in
    /// </summary>
    /// <param name="rowIndex"></param>
    public void FlagRowRemoval(int rowIndex)
    {
        foreach(GridSpace g in grid[rowIndex])
        {
            if(g.CheckTile())
            {
                g.FlagRemove();
            }
            else
            {
                Debug.Log("Error: Row not full, but called for removal");
            }
        }
        TilesToRemove = true;
        FlagWillFall(rowIndex);
    }

    /// <summary>
    /// Flags every gridspace containing a tile above the given row index for 
    /// falling in the next update loop
    /// </summary>
    /// <param name="rowIndex"></param>
    public void FlagWillFall(int rowIndex)
    {
        for (int i = rowIndex; i < grid.Length; ++i)
        {
            if(i == rowIndex)
            {
                continue;
            }
            foreach(GridSpace g in grid[i])
            {
                if(g.CheckTile())
                {
                    g.FlagWillFall();
                }
            }
        }
    }

    #endregion
}
