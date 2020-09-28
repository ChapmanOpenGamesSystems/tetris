using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Created by Sean Robbins
/// Date: 9/16/2020
/// 
/// This is the class that controls the behavior of each location on the grid
/// 
/// </summary>
public class GridSpace : MonoBehaviour
{
    public Tile tile;

    public SpriteRenderer TileSprite;


    [Tooltip("This bool determines if during the Update Loop this space will be cleared")]
    public bool flagRemove;

    [Tooltip("This bool flags a gridspace so that in the next update loop it will fall")]
    public int willFall;

    public bool playerControlled;
    private Tile nextTile;

    public bool readyRotate;

    /// <summary>
    /// Checks to see if the tile object of the gridspace is full,
    /// 
    /// if it contains a Tile return true,
    /// 
    /// else return false
    /// </summary>
    /// <returns></returns>
    public bool CheckTile()
    {
        return tile != null;
    }

    public void StoreSprite()
    {
        TileSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        StoreSprite();
    }

    /// <summary>
    /// This method returns true if the tile is controlled by the player
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerControlled()
    {
        if (tile != null && !tile.stable)
        {
            playerControlled = true;
            TileSprite.color = GameManager.Instance.PlayerControlledColor;
        }
        else if(tile != null && tile.stable)
        {
            playerControlled = false;
            TileSprite.color = GameManager.Instance.StableColor;
        }
        else if(tile == null)
        {
            TileSprite.color = GameManager.Instance.BackgroundColor;
        }
        return playerControlled;
    }

    public void Update()
    {
        CheckPlayerControlled();
    }

    /// <summary>
    /// This Method stabalizes the tile stored in this GridSpace as long as that tile is not null
    /// </summary>
    public void StabalizeTile()
    {
        if(tile != null)
        {
            tile.stable = true;
        }
        CheckPlayerControlled();
    }

    /// <summary>
    /// Flags the tile in the gridspace for removal
    /// </summary>
    public void FlagRemove()
    {
        flagRemove = true;
    }

    /// <summary>
    /// Flags the tile in this gridspace for movement
    /// </summary>
    public void FlagWillFall()
    {
        willFall++;
    }

    /// <summary>
    /// Flags the tile in the gridspace to stop moving
    /// </summary>
    public void StopFalling()
    {
        if(willFall > 0)
        {
            willFall--;
        }
    }

    /// <summary>
    /// Checks to see if this tile is flagged for falling
    /// </summary>
    /// <returns></returns>
    public bool CheckFalling()
    {
        return willFall > 0;
    }

    public void GainTile(Tile t)
    {
        if(CheckTile())
        {
            Debug.Log("trying to add a tile to a gridspace with a tile");
        }
        else
        {
            tile = t;
        }
    }

    public Tile GiveTile()
    {
        if(CheckTile())
        {
            flagRemove = true;
            Tile temp = tile;
            RemoveTile();
            return temp;
        }
        else
        {
            Debug.Log("Trying to Give a Tile from a Gridspace that doesnt contain one");
            return null;
        }
    }

    /// <summary>
    /// This method determines if there is a tile on this GridSpace,
    /// if it has been flagged for removal,
    /// If both of these conditions are true it removes the tile
    /// </summary>
    public void RemoveTile()
    {
        if(CheckTile() && flagRemove)
        {
            tile = null;
            willFall = 0;
            flagRemove = false;
            playerControlled = false;
        }
    }

    public void StoreTileRotation(Tile t)
    {
        nextTile = t;
    }

    public void PerformRotation()
    {
        Debug.Log("Rotating");
        tile = nextTile;
    }
}
