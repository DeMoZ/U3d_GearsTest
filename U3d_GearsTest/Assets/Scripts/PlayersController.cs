using System.Linq;
using UnityEngine;

public class PlayersController : MonoBehaviour
{
    [SerializeField] private Player[] _playersPrefabs = default;
    public Player[] _players;
    public Player[] Players => _players;

    public Stat[] _stats;
    public Buff[] _buffs;
    public int _minBuffs;
    public int _maxBuffs;
    public bool _allowDublicateBuffs;

    public void Init(Stat[] stats, Buff[] buffs, int minBuffs, int maxBuffs, bool allowDublicate)
    {
        _stats = stats;
        _buffs = buffs;
        _minBuffs = minBuffs;
        _maxBuffs = maxBuffs;
        _allowDublicateBuffs = allowDublicate;
    }

    public void MakePlayers(int amount)
    {
        _players = new Player[amount];
        var stepAngle = 360 / amount;
        var radius = amount / 2;
        for (var i = 0; i < amount; i++)
        {
            var position = Quaternion.Euler(0, i * stepAngle, 0) * Vector3.forward * radius;
            var rotation = Quaternion.LookRotation(-position);
            _players[i] = Instantiate(NextPlayer(i), position, rotation);

            _players[i].ApplyBuffs(_stats, null);
        }
    }

    public void ResetPlayersStats()
    {
        foreach (var player in _players)
            player.ApplyBuffs(_stats, player.Buffs);
    }

    public void SetBuffForPlayerType(int pIndex)
    {
        var group = _players.Where(p => p.PlayerType == _players[pIndex].PlayerType);

        Buff[] buffs = null;

        if (_players[pIndex].Buffs == null || _players[pIndex].Buffs.Length < 1) 
            buffs = RandomBuffs();

        foreach (var player in group)
            player.ApplyBuffs(_stats, buffs);
    }

    public void PlayerAttack(int pIndex)
    {
        var attackGroup = _players.Where(p => p.PlayerType == _players[pIndex].PlayerType);

        foreach (var player in attackGroup)
            player.AttackAnimation();

        var damagedGroup = _players.Where(p => p.PlayerType != _players[pIndex].PlayerType);
        
        foreach (var player in damagedGroup)
            player.Hit(_players[pIndex].Stats);
    }

    private void CalculateHit(Stat[] hunter, Stat[] victim )
    {
        victim.FirstOrDefault(s => s.title == Constants.Deffence);
    }
    
    private Player NextPlayer(int count) =>
        count < 2 ? _playersPrefabs[count] : _playersPrefabs[Random.Range(0, _playersPrefabs.Length)];

    private Buff[] RandomBuffs()
    {
        var amount = Random.Range(_minBuffs, _maxBuffs);

        if (!_allowDublicateBuffs)
        {
            amount = _buffs.Length < amount ? _buffs.Length : amount;
            var rnd = new System.Random();
            return _buffs.OrderBy(x => rnd.Next()).Take(amount).ToArray();
        }
        else
        {
            var buffs = new Buff[amount];
            for (var i = 0; i < buffs.Length; i++)
                buffs[i] = _buffs[Random.Range(0, buffs.Length)];

            return buffs;
        }
    }
}