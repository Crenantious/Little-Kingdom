using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BoardManager : MonoBehaviour
{
    public int widthInTiles = 8;
    public int heightInTiles = 8;
    public Tile[,] Tiles { get; private set; }

    [Inject] TileBorders tileBorders;

    // Must be a serializeableList for the custom Inspector to handle
    public List<TileTypeValue> tilePercentages = new();
    public Transform tilesParent;
    [Inject] Tile.Factory tileFactory;


    /// <summary>
    /// Used to associate a value with a TileType
    /// </summary>
    [Serializable]
    public class TileTypeValue
    {
        public TileType tileType;
        public int value;

        public TileTypeValue(TileType tileType, int value)
        {
            this.tileType = tileType;
            this.value = value;
        }
    }

    /// <returns>
    /// The amount of each type of tile to be created.
    /// </returns>
    List<TileTypeValue> GetTileAmounts()
    {
        int totalTiles = widthInTiles * heightInTiles;
        int remainingTiles = totalTiles;

        List<TileTypeValue> tileAmounts = new();

        for (int i = 0; i < tilePercentages.Count; i++)
        {
            int amount = Mathf.RoundToInt(totalTiles * (float)tilePercentages[i].value / 100);

            // The second check is a cop out. Due to rounding the total amount of assigned tiles
            // might not be accurate and so this will assign the remaining tiles to the last type
            // if the total tile coverage is 100% or more. Should create an  algorithm to handle better
            if (amount > remainingTiles || i == tilePercentages.Count - 1 && GetTotalTileTypeValue(tilePercentages) >= 100) amount = remainingTiles;

            remainingTiles -= amount;
            tileAmounts.Add(new TileTypeValue(tilePercentages[i].tileType, amount));

            if (remainingTiles <= 0) break;
        }
        return tileAmounts;
    }

    /// <returns>The sum of all <see cref="TileTypeValue.value"/> of the given list</returns>
    int GetTotalTileTypeValue(List<TileTypeValue> tileTypeValues)
    {
        int totalValue = 0;
        for (int i = 0; i < tileTypeValues.Count; i++)
        {
            totalValue += tileTypeValues[i].value;
        }
        return totalValue;
    }

    /// <summary>
    /// Takes a number, that represents the index of the cumulative <see cref="TileTypeValue.value"/> of the given list, 
    /// <br></br>
    /// and converts it into the index of the list that contains the cumulativeValue.
    /// </summary>
    /// <param name="cumulativeValue"></param>
    /// <returns>The index of the list that, when cumulated, contains the cumulativeValue.</returns>
    int CumulativeValueToIndex(List<TileTypeValue> tileAmounts, int cumulativeValue)
    {
        int cumulativeTiles = 0;
        for (int i = 0; i < tileAmounts.Count; i++)
        {
            if (cumulativeTiles + tileAmounts[i].value > cumulativeValue) return i;
            cumulativeTiles += tileAmounts[i].value;
        }
        return -1;
    }

    /// <summary>
    /// Generates a square grid of randomly ordered tiles with amounts given by <see cref="tilePercentages"/>.
    /// </summary>
    public void GenerateBoard()
    {
        Tiles = new Tile[widthInTiles, heightInTiles];
        List<TileTypeValue> tileAmounts = GetTileAmounts();
        int remainingTiles = GetTotalTileTypeValue(tileAmounts);

        if (remainingTiles < widthInTiles * heightInTiles)
        {
            throw new Exception("Not enough tiles assigned to fill the board");
        }

        for (int i = 0; i < widthInTiles; i++)
        {
            for (int j = 0; j < heightInTiles; j++)
            {
                int tileNumber = UnityEngine.Random.Range(0, remainingTiles);
                int tileTypeIndex = CumulativeValueToIndex(tileAmounts, tileNumber);
                if (tileTypeIndex == -1) throw new IndexOutOfRangeException("tileNumber is not found in tileAmounts");

                Tile tile = tileFactory.Create(tileAmounts[tileTypeIndex].tileType);

                tile.transform.SetParent(tilesParent);
                tile.transform.position = new Vector3(i * Tile.Width, 0, j * Tile.Height);

                Tiles[i, j] = tile;
                tile.boardPosition = new Vector2Int(i, j);

                tileAmounts[tileTypeIndex].value--;
                remainingTiles--;
                if (tileAmounts[tileTypeIndex].value == 0) tileAmounts.RemoveAt(tileTypeIndex);
            }
        }
    }

    public void DisplayBorderAroundTiles(Tile bottomLeftTile, int width, int height, Gradient gradient)
    {
        tileBorders.DisplayBorderAroundTiles(bottomLeftTile, width, height, gradient);
    }

    public void RemoveAllTileBorders()
    {
        tileBorders.RemoveAllTileBorders();
    }

    public Tile TryGetTileAtPosition(int x, int y)
    {
        if (x < 0 || x > widthInTiles || y < 0 || y > heightInTiles) return null;
        return Tiles[x, y];
    }
}
