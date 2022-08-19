
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Town : MonoBehaviour
{
    public List<Building> buildings = new();
    public Event mouseDownUpInteractable;
    public Event townPlacedEvent;
    public new Camera camera;
    public int distanceToPlaceFromOtherTowns = 1;
    public int widthInTiles = 2;
    public int heightInTiles = 2;
    public Player player;

    [Inject] readonly TileBorders tileBorders;
    [Inject] readonly BoardManager boardManager;
    [Inject] readonly InputManager inputManager;

    bool canPlace = false;
    bool isPlaced = false;
    Tile currentTile;

    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
        camera = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Building building))
                buildings.Add(building);
        }
        mouseDownUpInteractable.Subscribe(Place);
    }

    private void Update()
    {
        if (!isPlaced)
        {
            Tile tile = inputManager.GetTileClosestToMouse();
            if (tile && tile != currentTile)
            {
                currentTile = tile;
                transform.position = new Vector3((currentTile.boardPosition.x + 0.5f) * Tile.Width,
                                                 0,
                                                 (currentTile.boardPosition.y + 0.5f) * Tile.Height);

                CheckIfCanPlace();
                Gradient gradient = canPlace ? TileBorders.DefaultBorderGradients.allowed : TileBorders.DefaultBorderGradients.notAllowed;
                tileBorders.RemoveAllTileBorders();
                tileBorders.DisplayBorderAroundTiles(GetBottomLeftTileOfPlacementArea(),
                                                     distanceToPlaceFromOtherTowns * 2 + widthInTiles,
                                                     distanceToPlaceFromOtherTowns * 2 + heightInTiles,
                                                     gradient);
            }
            //Place();
        }
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

    Tile GetBottomLeftTileOfPlacementArea()
    {
        int x = currentTile.boardPosition.x - distanceToPlaceFromOtherTowns;
        int y = currentTile.boardPosition.y - distanceToPlaceFromOtherTowns;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        return boardManager.Tiles[x, y];
    }

    void AskToPlace()
    {
        if (!canPlace)
            return;
        // Show a UI options message asking to player to confirm the placement
        throw new System.NotImplementedException();
    }

    void AskToPlaceCallback(bool place)
    {
        if (place)
            Place();
    }

    void Place()
    {
        for (int i = 0; i < widthInTiles; i++)
        {
            for (int j = 0; j < heightInTiles; j++)
                boardManager.Tiles[currentTile.boardPosition.x + i, currentTile.boardPosition.y + j].town = this;
        }
        townPlacedEvent.Invoke(new EventInfo());
        isPlaced = true;
    }
}
