using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class TileBorders : IInitializable
{
    List<VisualEffect> borderList = new();
    [Inject] BorderFactory borderFactory;

    Gradient borderGradient;

    enum Side
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }

    public class DefaultBorderGradients
    {
        public static Gradient allowed;
        public static Gradient notAllowed;
    }

    class BorderInfo
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Tile tile;

        public BorderInfo(Tile tile, Side lineSide)
        {
            this.tile = tile;

            switch (lineSide)
            {
                case Side.Top:
                    startPosition = new Vector3(-Tile.Width / 2, -Tile.Height / 2, 0);
                    endPosition = new Vector3(Tile.Width / 2, -Tile.Height / 2, 0);
                    break;
                case Side.Bottom:
                    startPosition = new Vector3(-Tile.Width / 2, Tile.Height / 2, 0);
                    endPosition = new Vector3(Tile.Width / 2, Tile.Height / 2, 0);
                    break;
                case Side.Left:
                    startPosition = new Vector3(-Tile.Width / 2, -Tile.Height / 2, 0);
                    endPosition = new Vector3(-Tile.Width / 2, Tile.Height / 2, 0);
                    break;
                case Side.Right:
                    startPosition = new Vector3(Tile.Width / 2, -Tile.Height / 2, 0);
                    endPosition = new Vector3(Tile.Width / 2, Tile.Height / 2, 0);
                    break;
                default:
                    break;
            }
        }
    }

    public class BorderFactory : PlaceholderFactory<VisualEffect>
    {

    }

    public void Initialize()
    {
        VisualEffect vfx = borderFactory.Create();

        DefaultBorderGradients.allowed = vfx.GetGradient("Allowed");
        DefaultBorderGradients.notAllowed = vfx.GetGradient("Not allowed");

        GameObject.Destroy(vfx);
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
    public void DisplayBorderAroundTiles(Tile bottomLeftTile, int width, int height, Gradient gradient)
    {
        if (bottomLeftTile == null) throw new ArgumentNullException("bottomLeftTile cannot be null");
        if (bottomLeftTile.boardPosition.x < 0 || bottomLeftTile.boardPosition.x >= BoardManager.boardTileWidth ||
            bottomLeftTile.boardPosition.y < 0 || bottomLeftTile.boardPosition.y >= BoardManager.boardTileHeight)
        {
            throw new ArgumentOutOfRangeException("bottomLeftTile.boardPosition must be in range of BoardManager.tiles");
        }
        if (width < 1 || height < 1) throw new ArgumentOutOfRangeException("width and height must be greater than 0");

        borderGradient = gradient;
        Side[] sides = new Side[2];
        int posX, posY;

        for (int i = 0; i < width; i++)
        {
            posX = i + bottomLeftTile.boardPosition.x;
            if (posX >= BoardManager.boardTileWidth) break;

            if (i == 0) sides[0] = Side.Left;
            else if (i == width - 1 || posX == BoardManager.boardTileWidth - 1) sides[0] = Side.Right;
            else sides[0] = Side.None;

            for (int j = 0; j < height; j++)
            {
                posY = j + bottomLeftTile.boardPosition.y;
                if (posY >= BoardManager.boardTileHeight) break;

                if (j == 0) sides[1] = Side.Bottom;
                else if (j == height - 1 || posY == BoardManager.boardTileHeight - 1) sides[1] = Side.Top;
                else sides[1] = Side.None;

                for (int k = 0; k < 2; k++)
                {
                    if (sides[k] != Side.None)
                    {
                        DisplayBorder(new BorderInfo(BoardManager.tiles[posX, posY], sides[k]));
                    }
                }
            }
        }
    }

    void DisplayBorder(BorderInfo borderInfo)
    {
        VisualEffect vfx = borderFactory.Create();

        vfx.SetVector3("Side start pos", borderInfo.startPosition);
        vfx.SetVector3("Side end pos", borderInfo.endPosition);
        vfx.SetGradient("Colour", borderGradient);

        vfx.transform.SetParent(borderInfo.tile.transform);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localScale = new Vector3(1 / borderInfo.tile.transform.lossyScale.x,
                                               1 / borderInfo.tile.transform.lossyScale.y,
                                               1 / borderInfo.tile.transform.lossyScale.z);

        borderList.Add(vfx);
    }

    public void RemoveAllTileBorders()
    {
        foreach(var vfx in borderList)
        {
            GameObject.Destroy(vfx);
        }
        borderList.Clear();
    }
}
