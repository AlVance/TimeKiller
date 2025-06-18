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
    [SerializeField] private GameObject leaderboardParent;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItemPref;

    private string leaderboardID = "Main_Leaderboard";


    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, 70);

        leaderboardParent.SetActive(false);
    }
    int i = 0;
    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (leaderboardParent.activeInHierarchy)
            {
                leaderboardParent.SetActive(false);
            }
            else
            {
                leaderboardParent.SetActive(true);
                UpdateLeaderboard();

                try
                {

                    await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, 300);
                }
                catch (LeaderboardsException e)
                {
                    Debug.Log(e.Reason);
                }

            }
        }
    }

    async void UpdateLeaderboard()
    {
        while (Application.isPlaying && leaderboardParent.activeInHierarchy)
        {
            LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
            foreach (Transform item in leaderboardContentParent) { Destroy(item.gameObject); }
            foreach (LeaderboardEntry entry in leaderboardScoresPage.Results) {
                Transform leaderboardItem = Instantiate(leaderboardItemPref, leaderboardContentParent);
                leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.PlayerName;
                leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.Score.ToString();

            }
            await Task.Delay(500);
        }
    }
}
