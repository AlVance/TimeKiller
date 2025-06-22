using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardParent;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItemPref;
    [SerializeField] private Transform leaderboardSelfScore;


    private string leaderboardID = "Main_Leaderboard";

    [Header("Profile setup")]
    [SerializeField] private GameObject profileSetupParent;
    [SerializeField] private TMP_InputField profileNameField;
    private string playerName = "";

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync("*");
        }
        
    }

    public async void UpdateLeaderboard()
    {
        //while (Application.isPlaying && leaderboardParent.activeInHierarchy)
        //{
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, PlayerPrefs.GetFloat("MostTimeSaved"));

        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        foreach (Transform item in leaderboardContentParent) { Destroy(item.gameObject); }
        foreach (LeaderboardEntry entry in leaderboardScoresPage.Results.Take(10))
        {
            Transform leaderboardItem = Instantiate(leaderboardItemPref, leaderboardContentParent);
            leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Join("", entry.PlayerName.SkipLast(5));
            leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.Score.ToString("0.00");
            leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = (entry.Rank + 1).ToString();
        }

        var playerEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID);
        leaderboardSelfScore.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("PlayerName");
        leaderboardSelfScore.GetChild(2).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetFloat("MostTimeSaved").ToString("0.00");
        leaderboardSelfScore.GetChild(3).GetComponent<TextMeshProUGUI>().text = (playerEntry.Rank + 1).ToString();


        // await Task.Delay(500);
        //}
    }


    public void CreateProfile()
    {
        playerName = UIManager.Instance.profileNameField.text;
        AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        PlayerPrefs.SetString("PlayerName", playerName);

        //Comentar cuando pongamos este menú en la pantalla de nuevo récord
        //UpdateLeaderboard();
        CloseMenu();
    }

    public void CloseMenu()
    {
        UIManager.Instance.SetProfileScreenGOActive(false);
        UIManager.Instance.SetGoToCreditsBTNActive(true);
    }

}
