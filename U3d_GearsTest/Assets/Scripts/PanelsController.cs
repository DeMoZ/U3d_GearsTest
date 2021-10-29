using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelsController : MonoBehaviour
{
    private const string RemoveBuffs = "Убрать бафы";
    private const string AddBuffs = "Добавить бафы";
    
    [SerializeField] private StatView _statPrefab = default;
    [SerializeField] private PlayerPanelHierarchy[] _panels = default;
    [SerializeField] private Button[] _buffsButtons = default;

    private Player[] _players;
    private event Action<int> OnAttack;
    private event Action<int> OnBuff;

    private void Start()
    {
        for (var i = 0; i < _panels.Length; i++)
        {
            var pIndex = i;
            _panels[i].attackButton.onClick.AddListener(() => { OnAttack?.Invoke(pIndex); });
        }

        for (var i = 0; i < _buffsButtons.Length; i++)
        {
            var bIndex = i;
            _buffsButtons[i].onClick.AddListener(() => { OnBuff?.Invoke(bIndex); });
        }
    }

    private void OnDestroy()
    {
        OnAttack = null;
        OnBuff = null;
    }

    public void SubscribeAttack(Action<int> attackCallback) =>
        OnAttack += attackCallback;

    public void SubscribeBuffs(Action<int> buffCallback) => 
        OnBuff += buffCallback;

    public void Init(Player[] players)
    {
        _players = players;

        for (var i = 0; i < _panels.Length; i++)
        {
            if (i < players.Length)
            {
                CleanPanel(i);
                InstantiateStats(players, i);
                InstantiateBuffs(players, i);
            }
        }
    }

    private void CleanPanel(int i)
    {
        foreach (Transform child in _panels[i].statsPanel)
            Destroy(child.gameObject);
    }

    private void InstantiateStats(Player[] players, int i)
    {
        foreach (var stat in players[i].Stats)
        {
            var statView = Instantiate(_statPrefab, _panels[i].statsPanel);
            statView.Init(stat.title, Resources.Load<Sprite>($"icons/{stat.icon}"));
        }
    }

    private void InstantiateBuffs(Player[] players, int i)
    {
        if (players[i].Buffs != null)
        {
            SetBuffButtonText(i, true);
            foreach (var buff in players[i].Buffs)
            {
                var buffView = Instantiate(_statPrefab, _panels[i].statsPanel);
                buffView.Init(buff.title, Resources.Load<Sprite>($"icons/{buff.icon}"));
            }
        }
        else
        {
            SetBuffButtonText(i, false);
        }
    }

    private void SetBuffButtonText(int buttonIndex, bool hasBuffs)
    {
        if (buttonIndex < _buffsButtons.Length)
            _buffsButtons[buttonIndex].GetComponentInChildren<Text>().text = hasBuffs ? RemoveBuffs : AddBuffs;
    }
}