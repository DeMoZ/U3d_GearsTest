using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelsController : MonoBehaviour
{
    [SerializeField] private StatView _statPrefab = default;
    [SerializeField] private Image _hpBarPrefab = default;
    [SerializeField] private PlayerPanelHierarchy[] _panels = default;
    [SerializeField] private Button[] _buffsButtons = default;

    private Image[] _hpBars = new Image[] { };
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
        CleanPanels();
        // if (_hpBars.Length != players.Length)
        // {
        //     foreach (var hpBar in _hpBars)
        //     {
        //         Destroy(hpBar.gameObject);
        //     }
// 
        //     _hpBars = new Image[players.Length];
// 
        //     for (var i = 0; i < players.Length; i++)
        //     {
        //         _hpBars[i] = Instantiate(_hpBarPrefab, transform.) players[i];
        //     }
        // }

        for (var i = 0; i < players.Length; i++)
        {
            InstantiateHp(players[i].Stats.First(s => s.title.Equals(Constants.Hp)),
                players[i].PlayerType == players[0].PlayerType ? _panels[0].statsPanel : _panels[1].statsPanel);
        }

        // in case that all the players of the group has the same buffs
        for (var i = 0; i < _panels.Length; i++)
        {
            if (i < players.Length)
            {
                InstantiateStats(players[i], _panels[i].statsPanel);
                InstantiateBuffs(players[i], _panels[i].statsPanel);
                SetBuffButtonText(i, players[i].Buffs != null && players[i].Buffs.Length > 0);
            }
        }
    }

    private void InstantiateHp(Stat hpStat, Transform parent)
    {
        var statView = Instantiate(_statPrefab, parent);
        statView.Init(hpStat.value.ToString(), Resources.Load<Sprite>($"{Constants.IconsPath}{hpStat.icon}"));
    }

    private void CleanPanels()
    {
        foreach (var panel in _panels)
        {
            foreach (Transform child in panel.statsPanel)
                Destroy(child.gameObject);
        }
    }

    private void InstantiateStats(Player player, Transform parent)
    {
        foreach (var stat in player.Stats)
        {
            if (stat.title.Equals(Constants.Hp))
                continue;

            var statView = Instantiate(_statPrefab, parent);
            statView.Init(stat.value.ToString(), Resources.Load<Sprite>($"{Constants.IconsPath}{stat.icon}"));
        }
    }

    private void InstantiateBuffs(Player player, Transform parent)
    {
        if (player.Buffs != null)
        {
            foreach (var buff in player.Buffs)
            {
                var buffView = Instantiate(_statPrefab, parent);
                buffView.Init(buff.title, Resources.Load<Sprite>($"{Constants.IconsPath}{buff.icon}"));
            }
        }
    }

    private void SetBuffButtonText(int buttonIndex, bool hasBuffs)
    {
        if (buttonIndex < _buffsButtons.Length)
            _buffsButtons[buttonIndex].GetComponentInChildren<Text>().text =
                hasBuffs ? Constants.RemoveBuffs : Constants.AddBuffs;
    }
}