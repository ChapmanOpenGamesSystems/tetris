using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by Sean Robbins
/// Date: 9/16/2020
/// 
/// This is the class that creates the objects used to store the locations of the player controlled tiles
/// 
/// </summary>
public class Coordinate : MonoBehaviour
{
    public int row;

    public int col;

    public int startRow;

    public int startCol;

    public bool isCenter;

    public Tile tile;

    public SpriteRenderer spriteRenderer;

    public bool hasMoved;

    public bool canNotMove;

    [Tooltip("The desired end angle of its next rotation, used to determine how it will rotate")]
    public int rotateState;

    public void Start()
    {
        startRow = row;
        startCol = col;
    }

    public void MoveDown()
    {
        row -= 1;
    }

    public void MoveLeft()
    {
        col -= 1;
    }

    public void MoveRight()
    {
        col += 1;
    }
}
