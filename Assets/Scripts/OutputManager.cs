using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputManager : MonoBehaviour
{
    public GameObject tileSprite;

    public GridManager gameBoard;

    private GameObject[][] outputBoard;

    [Tooltip("This is the size in pixels of the sprite and is used so the output manager knows how far apart each tile should be generated")]
    public float TileSize;

    private const int PIXELS_PER_UNIT = 64;

    public int boardWidth;

    public int boardHeight;

    public int GenerateRow;
    
    

    public void Start()
    {
        Debug.Log("Start Called");
        TileSize = tileSprite.GetComponent<SpriteRenderer>().sprite.rect.height/PIXELS_PER_UNIT;
        outputBoard = new GameObject[boardHeight][];
        for(int i = 0; i < boardHeight; ++i)
        {
            outputBoard[i] = new GameObject[boardWidth];
        }
        GenerateBoard();
    }

    public void Update()
    {
        
    }

    public void GenerateBoard()
    {
        for(int i = 0; i < boardHeight; ++i)
        {
            for (int j = 0; j < boardWidth; ++j)
            {
                outputBoard[i][j] = Instantiate(tileSprite, new Vector3(j * TileSize, i * TileSize), new Quaternion(), gameObject.transform);
                gameBoard.StoreGridSpaces(i, j, outputBoard[i][j]);
            }
        }
    }
}
