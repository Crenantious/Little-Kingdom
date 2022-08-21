using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TileBorders
{
    [Inject] readonly Factories.Border borderFactory;
    [Inject] readonly BoardManager boardManager;
    readonly List<TileBorder> borders = new();

    Gradient borderGradient;

    public class Sides
    {
        public bool left;
        public bool right;
        public bool top;
        public bool bottom;
    }

    /// <summary>
    /// Displays particles surrounding the given area
    /// <br/>
    /// Should the given area exceed the board size particles will be displayed at the board's border
    /// </summary>
    /// <param name="bottomLeftTile">The origin of the area</param>
    /// <param name="width">Width of the area in tiles</param>
    /// <param name="height">Height of the area in tiles</param>
    /// <param name="errorColour">If the partical colour should be the error colour or the non-error colour</param>
    public void DisplayBorderAroundTiles(int bottomLeftTileX, int bottomLeftTileY, int width, int height, Gradient gradient)
    {
        if (width < 1 || height < 1)
            throw new ArgumentOutOfRangeException("width and height must be greater than 0");

        borderGradient = gradient;
        Sides sides = new();
        int posX, posY;

        for (int i = 0; i < width; i++)
        {
            posX = i + bottomLeftTileX;
            if (posX < 0)
                continue;
            if (posX >= boardManager.widthInTiles)
                break;

            sides.left = i == 0 || posX == 0;
            sides.right = i == width - 1 || posX == boardManager.widthInTiles - 1;

            for (int j = 0; j < height; j++)
            {
                posY = j + bottomLeftTileY;

                if (posY < 0)
                    continue;
                if (posY >= boardManager.heightInTiles)
                    break;

                sides.bottom = j == 0 || posY == 0;
                sides.top = j == height - 1 || posY == boardManager.heightInTiles - 1;

                DisplayBorders(boardManager.Tiles[posX, posY], sides);
            }
        }
    }

    public void RemoveAllTileBorders()
    {
        foreach (var border in borders)
            border.Dispose();
        borders.Clear();
    }

    void DisplayBorders(Tile tile, Sides sides)
    {
        if (sides.bottom)
            DisplayBorder(tile, new Vector2(0, -Tile.Height / 2),
                                0);
        if (sides.top)
            DisplayBorder(tile, new Vector2(0, Tile.Height / 2),
                                180);
        if (sides.left)
            DisplayBorder(tile, new Vector2(-Tile.Width / 2, 0),
                                90);
        if (sides.right)
            DisplayBorder(tile, new Vector2(Tile.Width / 2, 0),
                                270); 
        //if (sides.bottom)
        //    DisplayBorder(tile, new Vector3(-Tile.Width / 2, 0, -Tile.Height / 2),
        //                        new Vector3(Tile.Width / 2, 0, -Tile.Height / 2));
        //if (sides.top)
        //    DisplayBorder(tile, new Vector3(-Tile.Width / 2, 0, Tile.Height / 2),
        //                        new Vector3(Tile.Width / 2, 0, Tile.Height / 2));
        //if (sides.left)
        //    DisplayBorder(tile, new Vector3(-Tile.Width / 2, 0, -Tile.Height / 2),
        //                        new Vector3(-Tile.Width / 2, 0, Tile.Height / 2));
        //if (sides.right)
        //    DisplayBorder(tile, new Vector3(Tile.Width / 2, 0, -Tile.Height / 2),
        //                        new Vector3(Tile.Width / 2, 0, Tile.Height / 2));
    }

    void DisplayBorder(Tile tile, Vector2 positionOnTile, float localYRotation)
    {
        var border = borderFactory.Create(tile,
                                          new Vector3(positionOnTile.x, 0, positionOnTile.y),
                                          localYRotation,
                                          borderGradient);
        borders.Add(border);
    }
}
