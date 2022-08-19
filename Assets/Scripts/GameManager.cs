using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    public int numberOfPlayers = 3;
    public Event townPlacedEvent;
    public List<Player> players = new();

    [Inject] readonly Factories.Player playerFactory;
    [Inject] readonly BoardManager boardManager;
    [Inject] readonly UIPlayerInfoPanels uiPlayerInfoPanels;
    [Inject] readonly UIOptionsMessageManager optionsMessageManager;

    void Start()
    {
        boardManager.GenerateBoard();
        townPlacedEvent.Subscribe(TownPlaced);
        CreatePlayers(numberOfPlayers);
    }

    public void CreatePlayers(int numberOfPlayers)
    {
        if (numberOfPlayers <= 0)
            return;
        this.numberOfPlayers = numberOfPlayers;
        CreatePlayer();
    }

    void CreatePlayer()
    {
        players.Add(playerFactory.Create());
        uiPlayerInfoPanels.AddPanel(players[^1]);
        foreach (Resource resource in References.resources)
            players[^1].AddResource(resource, 50);
    }

    void TownPlaced()
    {
        if (players.Count < numberOfPlayers)
            CreatePlayer();
    }
}
