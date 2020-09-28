using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by Sean Robbins
/// Date: 9/16/2020
/// 
/// This is the class that manages what is stored in each tile object
/// Will be mainly used to store image information about the tiles
/// 
/// </summary>
public class Tile : MonoBehaviour
{
    [Tooltip("If a tile is stabe it has stopped moving on the grid and is contributing to a row completeness")]
    public bool stable;
}
