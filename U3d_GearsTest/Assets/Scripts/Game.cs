using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private PlayersController _playersController;
    [SerializeField] private PanelsController _panelsController = default;

    private void Start()
    {
        var data = LoadGameData();

        //if (data.Data == null){ brake scenario; return; }

        _playersController.Init(data.stats, data.buffs,
            data.settings.buffCountMin, data.settings.buffCountMax, data.settings.allowDuplicateBuffs);
        _playersController.MakePlayers(data.settings.playersCount);

        _panelsController.SubscribeAttack(_playersController.PlayerAttack);
        _panelsController.SubscribeBuffs(SetBuffForPlayerType);
        _panelsController.Init(_playersController.Players);
    }

    private void SetBuffForPlayerType(int pIndex)
    {
        _playersController.SetBuffForPlayerType(pIndex);
        ResetGame();
    }

    private void ResetGame()
    {
        _playersController.ResetPlayersStats();
        _panelsController.Init(_playersController.Players);
    }

    private Data LoadGameData()
    {
        var data = JsonUtility.FromJson<Data>(Resources.Load("data").ToString());
        data.settings.playersCount = data.settings.playersCount < 2 ? 2 : data.settings.playersCount;
        // bool anyIssue = true_if_any_issue_with_datafile // such as wrong ranges or no specific or invalid data
        // if (anyIssue) return null;

        return data;
    }
}