using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class TileBorders : IInitializable
{
    [Inject] readonly Factories.Border borderFactory;
    [Inject] readonly BoardManager boardManager;
    readonly List<VisualEffect> borderList = new();

    [SerializeField] string borderSideStartPosPropertyName = "Side start pos";
    [SerializeField] string borderSideEndPosPropertyName = "Side end pos";
    [SerializeField] string borderColourPropertyName = "Colour";
    [SerializeField] string borderAllowedGradientPropertyName = "Allowed";
    [SerializeField] string borderNotAllowedGradientPropertyName = "Not allowed";
    Gradient borderGradient;

    public class DefaultBorderGradients
    {
        public static Gradient allowed;
        public static Gradient notAllowed;
    }

    class Sides
    {
        public bool left;
        public bool right;
        public bool top;
        public bool bottom;
    }

    public void Initialize()
    {
        VisualEffect vfx = borderFactory.Create();
        DefaultBorderGradients.allowed = vfx.GetGradient(borderAllowedGradientPropertyName);
        DefaultBorderGradients.notAllowed = vfx.GetGradient(borderNotAllowedGradientPropertyName);
        UnityEngine.Object.Destroy(vfx);
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
        if (bottomLeftTile == null)
            throw new ArgumentNullException("bottomLeftTile cannot be null");

        if (bottomLeftTile.boardPosition.x < 0 || bottomLeftTile.boardPosition.x >= boardManager.widthInTiles ||
            bottomLeftTile.boardPosition.y < 0 || bottomLeftTile.boardPosition.y >= boardManager.heightInTiles)
            throw new ArgumentOutOfRangeException("bottomLeftTile.boardPosition must be in range of boardManager.tiles");

        if (width < 1 || height < 1)
            throw new ArgumentOutOfRangeException("width and height must be greater than 0");

        borderGradient = gradient;
        Sides sides = new();
        int posX, posY;

        for (int i = 0; i < width; i++)
        {
            posX = i + bottomLeftTile.boardPosition.x;
            if (posX >= boardManager.widthInTiles)
                break;

            sides.left = i == 0;
            sides.right = i == width - 1 || posX == boardManager.widthInTiles - 1;

            for (int j = 0; j < height; j++)
            {
                posY = j + bottomLeftTile.boardPosition.y;
                if (posY >= boardManager.heightInTiles)
                    break;

                sides.bottom = j == 0;
                sides.top = j == height - 1 || posY == boardManager.heightInTiles - 1;

                DisplayBorders(boardManager.Tiles[posX, posY], sides);
            }
        }
    }

    public void RemoveAllTileBorders()
    {
        foreach (var vfx in borderList)
            UnityEngine.Object.Destroy(vfx);
        borderList.Clear();
    }

    void DisplayBorders(Tile tile, Sides sides)
    {
        if (sides.top)
            DisplayBorder(tile, new Vector3(-Tile.Width / 2, -Tile.Height / 2, 0),
                                new Vector3(Tile.Width / 2, -Tile.Height / 2, 0));
        if (sides.bottom)
            DisplayBorder(tile, new Vector3(-Tile.Width / 2, Tile.Height / 2, 0),
                                new Vector3(Tile.Width / 2, Tile.Height / 2, 0));
        if (sides.left)
            DisplayBorder(tile, new Vector3(-Tile.Width / 2, -Tile.Height / 2, 0),
                                new Vector3(-Tile.Width / 2, Tile.Height / 2, 0));
        if (sides.right)
            DisplayBorder(tile, new Vector3(Tile.Width / 2, -Tile.Height / 2, 0),
                                new Vector3(Tile.Width / 2, Tile.Height / 2, 0));
    }

    void DisplayBorder(Tile tile, Vector3 startPos, Vector3 endPos)
    {
        VisualEffect vfx = borderFactory.Create();

        vfx.SetVector3(borderSideStartPosPropertyName, startPos);
        vfx.SetVector3(borderSideEndPosPropertyName, endPos);
        vfx.SetGradient(borderColourPropertyName, borderGradient);

        vfx.transform.SetParent(tile.transform);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localScale = new Vector3(1 / tile.transform.lossyScale.x,
                                               1 / tile.transform.lossyScale.y,
                                               1 / tile.transform.lossyScale.z);
        borderList.Add(vfx);
    }
}
