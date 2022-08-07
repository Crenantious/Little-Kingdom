
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

    public int widthInTiles = 2;
    public int heightInTiles = 2;

    public Player player;
    bool canPlace = false;
    bool isPlaced = false;

    Tile currentTile;
    [Inject] TileBorders tileBorders;
    [Inject] BoardManager boardManager;
    public int distanceToPlaceFromOtherTowns = 1;

    public class Factory : PlaceholderFactory<Player, Town>
    {

    }

    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
        camera = Camera.main;
    }


    void Start()
    {
        Building building;
        for(int i=0;i<transform.childCount;i++)
        {
            if (transform.GetChild(i).TryGetComponent(out building))
            {
                buildings.Add(building);
            }
        }

        mouseDownUpInteractable.Subscribe(Place);
    }

    private void Update()
    {
        if(!isPlaced)
        {
            Tile tile = GetTileClosestToMouse();
            if(tile != null && tile != currentTile)
            {
                currentTile = tile;
                transform.position = new Vector3((currentTile.boardPosition.x + 0.5f) * Tile.Width,
                                                 0,
                                                 (currentTile.boardPosition.y + 0.5f) * Tile.Height);

                CheckIfCanPlace();
                Gradient gradient = canPlace ? TileBorders.DefaultBorderGradients.allowed : TileBorders.DefaultBorderGradients.notAllowed;
                tileBorders.RemoveAllTileBorders();
                tileBorders.DisplayBorderAroundTiles(GetBottomLeftTileOfPlacementArea(), 
                                                     distanceToPlaceFromOtherTowns*2+widthInTiles, 
                                                     distanceToPlaceFromOtherTowns*2+heightInTiles,
                                                     gradient);
            }
        }
    }

    Tile GetTileClosestToMouse()
    {
        Tile tile = null;

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            int x = Mathf.RoundToInt(hit.point.x / Tile.Width);
            int y = Mathf.RoundToInt(hit.point.z / Tile.Height);

            x = Mathf.Clamp(x, 0, boardManager.widthInTiles - 2);
            y = Mathf.Clamp(y, 0, boardManager.heightInTiles - 2);

            tile = boardManager.TryGetTileAtPosition(x, y);
        }

        return tile;
    }

    void CheckIfCanPlace()
    {
        canPlace = true;

        for (int i = -distanceToPlaceFromOtherTowns; i < widthInTiles + distanceToPlaceFromOtherTowns; i++)
        {
            if (!canPlace) break;

            for (int j = -distanceToPlaceFromOtherTowns; j < heightInTiles + distanceToPlaceFromOtherTowns; j++)
            {
                Tile tile = boardManager.TryGetTileAtPosition(currentTile.boardPosition.x + i, currentTile.boardPosition.y + j);
                if (tile != null && tile.town != null)
                {
                    Debug.Log(tile.town);
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
    void Place()
    {
        if (!canPlace) return;

        for (int i = 0; i < widthInTiles; i++)
        {
            for (int j = 0; j < heightInTiles; j++)
            {
                boardManager.Tiles[currentTile.boardPosition.x + i, currentTile.boardPosition.y + j].town = this;
            }
        }

        townPlacedEvent.Invoke(new EventInfo());
        isPlaced = true;
    }
}
