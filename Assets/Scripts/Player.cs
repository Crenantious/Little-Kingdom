using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player
{
    [Inject] Town.Factory townFactory;
    public Town town;

    [Inject]
    public void Constrct()
    {
        town = townFactory.Create(this);
    }

    public class Factory : PlaceholderFactory<Player>
    {

    }
}
