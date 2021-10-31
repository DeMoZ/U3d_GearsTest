using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private bool _isAttacking;
    private event Action OnHit;

    public void Init(Stat[] stats, Buff[] buffs, int minBuffs, int maxBuffs, bool allowDublicate, Action hitCallback)
    {
        _stats = stats;
        _buffs = buffs;
        _minBuffs = minBuffs;
        _maxBuffs = maxBuffs;
        _allowDublicateBuffs = allowDublicate;
        OnHit += hitCallback;
    }

    private void OnDestroy() =>
        OnHit = null;

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

    public void Attack(int pIndex)
    {
        if (_isAttacking) return;

        _isAttacking = true;
        Invoke(nameof(SetIdle), 1);

        var hunters = _players.Where(p => p.PlayerType == _players[pIndex].PlayerType).ToList();
        var victims = _players.Where(p => p.PlayerType != _players[pIndex].PlayerType && p.Hp > 0).ToList();

        if (victims.Count < 1) return;

        foreach (var hunter in hunters)
            StartCoroutine(IEAttack(hunter, victims[RandomVictim(victims.Count())]));
    }

    private int RandomVictim(int randomMax) =>
        Random.Range(0, randomMax);

    private IEnumerator IEAttack(Player hunter, Player victim)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        hunter.AttackAnimation();
        CalculateHit(hunter, victim);
    }

    private void SetIdle() =>
        _isAttacking = false;

    private void CalculateHit(Player hunter, Player victim)
    {
        var hp = victim.Stats.First(s => s.title == Constants.Hp);
        var deff = victim.Stats.First(s => s.title == Constants.Deffence).value;

        var power = hunter.Stats.First(s => s.title == Constants.Damage).value;
        var sucking = hunter.Stats.First(s => s.title == Constants.Sucking).value;

        var damage = power * (100 - deff) / 100;
        var sip = damage * sucking / 100;

        victim.Hit((int)damage);
        hunter.Hit((int)-sip);
        OnHit?.Invoke();
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