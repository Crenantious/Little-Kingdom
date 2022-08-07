using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] Player.Factory playerFactory;
    [Inject] BoardManager boardManager;

    public int numberOfPlayers = 3;
    public Event townPlacedEvent;
    public List<Player> players = new();
    void Start()
    {
        boardManager.GenerateBoard();
        townPlacedEvent.Subscribe(TownPlaced);
        CreatePlayers(numberOfPlayers);

    }

    public void CreatePlayers(int numberOfPlayers)
    {
        if (numberOfPlayers <= 0) return;
        this.numberOfPlayers = numberOfPlayers;
        CreatePlayer();
    }

    void CreatePlayer()
    {
        players.Add(playerFactory.Create());
    }

    void TownPlaced()
    {
        if (players.Count < numberOfPlayers) CreatePlayer();
    }
}
