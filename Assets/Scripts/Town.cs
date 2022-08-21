using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Town : MonoBehaviour
{
    public List<Building> buildings = new();
    public Event townPlacedEvent;
    public new Camera camera;
    public int distanceToPlaceFromOtherTowns = 1;
    public int widthInTiles = 2;
    public int heightInTiles = 2;
    public Player player;

    [Inject] readonly TileBorders tileBorders;
    [Inject] readonly BoardManager boardManager;
    [Inject] readonly InputManager inputManager;
    [Inject] readonly UIOptionsMessageManager optionsMessageManager;

    bool canPlace = false;
    bool isPlaced = false;
    bool isAskingToPlace = false;
    Tile currentTile;

    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
        camera = Camera.main;
    }

    void Start()
    {
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    if (transform.GetChild(i).TryGetComponent(out Building building))
        //        buildings.Add(building);
        //}
        References.MouseReleasedOnGameObject.Subscribe(AskToPlace);
    }

    void Update()
    {
        if (!isPlaced && !isAskingToPlace)
            CheckIfMouseNearNewTile();
    }

    void CheckIfMouseNearNewTile()
    {
        Tile tile = inputManager.GetTileClosestToMouse();
        if (tile && tile != currentTile)
            MoveToNewTile(tile);
    }

    void MoveToNewTile(Tile tile)
    {
        currentTile = tile;
        transform.position = new Vector3((currentTile.boardPosition.x + 0.5f) * Tile.Width,
                                         0,
                                         (currentTile.boardPosition.y + 0.5f) * Tile.Height);
        CheckIfCanPlace();
        DisplayBorders();
    }

    void DisplayBorders()
    {
        Gradient gradient = canPlace ? TileBorder.DefaultGradients.allowed : TileBorder.DefaultGradients.notAllowed;
        tileBorders.RemoveAllTileBorders();
        tileBorders.DisplayBorderAroundTiles(currentTile.boardPosition.x - distanceToPlaceFromOtherTowns,
                                             currentTile.boardPosition.y - distanceToPlaceFromOtherTowns,
                                             distanceToPlaceFromOtherTowns * 2 + widthInTiles,
                                             distanceToPlaceFromOtherTowns * 2 + heightInTiles,
                                             gradient);
    }

    void CheckIfCanPlace()
    {
        canPlace = true;

        for (int i = -distanceToPlaceFromOtherTowns; i < widthInTiles + distanceToPlaceFromOtherTowns; i++)
        {
            if (!canPlace)
                break;

            for (int j = -distanceToPlaceFromOtherTowns; j < heightInTiles + distanceToPlaceFromOtherTowns; j++)
            {
                Tile tile = boardManager.TryGetTileAtPosition(currentTile.boardPosition.x + i, currentTile.boardPosition.y + j);
                if (tile && tile.town)
                {
                    canPlace = false;
                    break;
                }
            }
        }
    }

    void AskToPlace()
    {
        if (!canPlace)
            return;
        optionsMessageManager.Display("Place town here?",
                                      new List<string>() { "Yes", "No" },
                                      AskToPlaceCallback);
        isAskingToPlace = true;
    }

    void AskToPlaceCallback(int option)
    {
        if (option == 0)
            Place();
        isAskingToPlace = false;
    }

    void Place()
    {
        for (int i = 0; i < widthInTiles; i++)
        {
            for (int j = 0; j < heightInTiles; j++)
                boardManager.Tiles[currentTile.boardPosition.x + i, currentTile.boardPosition.y + j].town = this;
        }

        tileBorders.RemoveAllTileBorders();
        References.MouseReleasedOnGameObject.Unsubscribe(AskToPlace);
        townPlacedEvent.Invoke(new EventInfo());
        isPlaced = true;
    }
}
