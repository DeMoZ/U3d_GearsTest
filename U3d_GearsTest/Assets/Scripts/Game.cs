using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player[] _playersPrefabs = default;
    [SerializeField] private PanelsController _panelsController = default;
    private Data _data;
    private Player[] _players;

    private void Start()
    {
        _data = LoadGameData();
        //if (data == null){ brake scenario; return; }

        _players = MakePlayers(_data.settings.playersCount);
        ResetPlayersStats();

        foreach (var player in _players)
            player.SetBuffs(null);

        _panelsController.SubscribeAttack(PlayerAttack);
        _panelsController.SubscribeBuffs(SetBuffForPlayerType);
        _panelsController.Init(_players);
    }

    private void SetBuffForPlayerType(int pIndex)
    {
        var group = _players.Where(p => p.PlayerType == _players[pIndex].PlayerType);

        Buff[] buffs = null;

        if (_players[pIndex].Buffs == null || _players[pIndex].Buffs.Length < 1)
            buffs = RandomBuffs();

        foreach (var player in group)
            player.SetBuffs(buffs);

        ResetGame();
    }

    private void ResetGame()
    {
        ResetPlayersStats();
        _panelsController.Init(_players);
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

    private void PlayerAttack(int pIndex)
    {
        var group = _players.Where(p => p.PlayerType == _players[pIndex].PlayerType);

        foreach (var player in group)
            player.AttackAnimation();
    }

    private Buff[] RandomBuffs()
    {
        var amount = Random.Range(_data.settings.buffCountMin, _data.settings.buffCountMax);
        var rnd = new System.Random();

        if (!_data.settings.allowDuplicateBuffs)
            //if(false)
        {
            amount = _data.buffs.Length < amount ? _data.buffs.Length : amount;
            return _data.buffs.OrderBy(x => rnd.Next()).Take(amount).ToArray();
        }
        else
        {
            var buffs = new Buff[amount];
            for (var i = 0; i < buffs.Length; i++)
                buffs[i] = _data.buffs[Random.Range(0, buffs.Length)];

            return buffs;
        }
    }

    private Data LoadGameData()
    {
        var data = JsonUtility.FromJson<Data>(Resources.Load("data").ToString());
        data.settings.playersCount = data.settings.playersCount < 2 ? 2 : data.settings.playersCount;
        // bool anyIssue = true_if_any_issue_with_datafile // such as wrong ranges or no specific or invalid data
        // if (anyIssue) return null;

        return data;
    }

    private Player NextPlayer(int count) =>
        count < 2 ? _playersPrefabs[count] : _playersPrefabs[Random.Range(0, _playersPrefabs.Length)];

    private void ResetPlayersStats()
    {
        foreach (var player in _players)
            player.SetStats(_data.stats);
    }
}