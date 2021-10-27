using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player[] _playersPrefabs;

    private Data _data;
    private Player[] _players;

    void Start()
    {
        _data = LoadGameData();
        _players = MakePlayers(_data.settings.playersCount > 1 ? _data.settings.playersCount : 2);

        /*PopulateStats();
        PopulateBuffs();*/
    }

    private Player[] MakePlayers(int amount)
    {
        var players = new Player[amount];
        var stepAngle = 360 / amount;
        var radius = amount / 2;
        for (var i = 0; i < amount; i++)
        {
            var position = Quaternion.Euler(0, i * stepAngle, 0) * Vector3.forward * radius;
            var rotation = Quaternion.LookRotation(-position);
            players[i] = Instantiate(NextPlayer(i), position, rotation);
        }

        return players;
    }

    private Data LoadGameData() =>
        JsonUtility.FromJson<Data>(Resources.Load("data").ToString());

    private Player NextPlayer(int count)
    {
        return count < 2 ? _playersPrefabs[count] : _playersPrefabs[Random.Range(0, _playersPrefabs.Length)];
    }
}