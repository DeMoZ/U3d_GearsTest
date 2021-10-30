using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public enum PlayerType
{
    Cop,
    Android
}

public class Player : MonoBehaviour
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Health = Animator.StringToHash("Health");

    [SerializeField, ReadOnly] private float _health;
    [SerializeField] private PlayerType _playerType = PlayerType.Cop;
    [SerializeField] private Animator _animator = default;
    public PlayerType PlayerType => _playerType;
    public Stat[] Stats; //{ get; private set; }
    public Buff[] Buffs; //{ get; private set; }

    public void AttackAnimation()
    {
        if (_health >= 1)
            _animator.SetTrigger(Attack);
    }

    public void Hit(Stat[] inStats)
    {
        // todo calculate value based on my stats and incoming stats
        // _health += value;
        // _animator.SetFloat(Health, _health);
    }

    
    public void ApplyBuffs(Stat[] stats, Buff[] buffs)
    {
        var playerStats = Copy(stats.ToList()).ToArray();

        Buffs = buffs != null ? Copy(buffs.ToList()).ToArray() : null;

        if (Buffs == null || Buffs.Length < 1)
        {
            Stats = playerStats;
        }
        else
        {
            var buffStats = new List<BuffStat>();
            
            foreach (var buff in Buffs)
            {
                if (buff.stats != null)
                    buffStats.AddRange(buff.stats);
            }

            var resultStats = new List<Stat>();
            foreach (var stat in playerStats)
            {
                stat.value += buffStats.Sum(b => b.statId == stat.id ? b.value : 0);
                resultStats.Add(stat);
            }

            Stats = resultStats.ToArray();
        }

        _health = Stats.First(s => s.title == "жизнь").value;
        _animator.SetInteger(Health, (int)_health);
    }

    private List<T> Copy<T>(List<T> stats) where T : ICanBeCopied<T>
    {
        var copy = new List<T>();
        if (stats != null && stats.Count > 0)
            copy.AddRange(stats.Select(t => t.Copy()));

        return copy;
    }
}