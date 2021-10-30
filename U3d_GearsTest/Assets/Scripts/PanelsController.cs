using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelsController : MonoBehaviour
{
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
                InstantiateStats(players[i], _panels[i].statsPanel);
                InstantiateBuffs(players[i], _panels[i].statsPanel);
                SetBuffButtonText(i, players[i].Buffs != null && players[i].Buffs.Length > 0);
            }
        }
    }

    private void CleanPanel(int i)
    {
        foreach (Transform child in _panels[i].statsPanel)
            Destroy(child.gameObject);
    }

    private void InstantiateStats(Player player, Transform parent)
    {
        foreach (var stat in player.Stats)
        {
            var statView = Instantiate(_statPrefab, parent);
            statView.Init(stat.value.ToString(), Resources.Load<Sprite>($"{Constants.IconsPath}{stat.icon}"));
        }
    }

    private void InstantiateBuffs(Player player, Transform parent)
    {
        // if (player.Buffs != null)
        //     Debug.Log($"buffs for player {player.Buffs.Length}");
        
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
            _buffsButtons[buttonIndex].GetComponentInChildren<Text>().text = hasBuffs ? 
                Constants.RemoveBuffs:
                Constants.AddBuffs;
    }
}