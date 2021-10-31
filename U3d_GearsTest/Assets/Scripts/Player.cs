using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private PlayerType _playerType = PlayerType.Cop;
    [SerializeField] private Animator _animator = default;
    [SerializeField] private Transform _hpAnchor = default;

    public PlayerType PlayerType => _playerType;
    public Transform HpAnchor => _hpAnchor;
    public Stat[] Stats { get; private set; }
    public Buff[] Buffs { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }

    public void AttackAnimation()
    {
        if (Hp >= 1)
            _animator.SetTrigger(Attack);
    }

    public void Hit(int damage)
    {
        var stat = Stats.First(s => s.title.Equals(Constants.Hp));
        Hp += damage;
        Hp = (int)Mathf.Clamp(Hp, 0, float.MaxValue);
        stat.value = Hp;
        _animator.SetInteger(Health, Hp);
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

        Hp = (int)Stats.First(s => s.title == Constants.Hp).value;
        MaxHp = Hp;
        _animator.SetInteger(Health, (int)Hp);
    }

    private List<T> Copy<T>(List<T> stats) where T : ICanBeCopied<T>
    {
        var copy = new List<T>();
        if (stats != null && stats.Count > 0)
            copy.AddRange(stats.Select(t => t.Copy()));

        return copy;
    }
}