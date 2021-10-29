using System.Collections;
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

    public Stat[] Stats { get; private set; }
    public Buff[] Buffs { get; private set; }

    public void SetStats(IEnumerable stats)
    {
        Stats = stats as Stat[];
        _health = Stats.Sum(s => s.title == "жизнь" ? s.value : 0);
        _animator.SetInteger(Health, (int)_health);
    }

    public void SetBuffs(IEnumerable buffs)
    {
        Buffs = buffs as Buff[];
    }

    public void AttackAnimation()
    {
        if (_health >= 1)
            _animator.SetTrigger(Attack);
    }

    public void Hit(float value)
    {
        _health += value;
        _animator.SetFloat(Health, _health);
    }
}