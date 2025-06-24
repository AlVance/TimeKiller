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
using System.Collections;
using UnityEngine.Networking;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderBoardGO;
    [SerializeField] private GameObject noConectionGO;
    [SerializeField] private GameObject leaderboardParent;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItemPref;
    [SerializeField] private Transform leaderboardSelfScore;
    [SerializeField] private GameObject ownRecordGO;
    [SerializeField] private GameObject notRegisteredTextGO;

    private string leaderboardID = "Main_Leaderboard";

    [Header("Profile setup")]
    [SerializeField] private GameObject profileSetupParent;
    [SerializeField] private TMP_InputField profileNameField;
    private string playerName = "";

    private void Awake()
    {
        StartCoroutine(CheckInternetConnection());
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        InitialiteServicies();
    }
    async void InitialiteServicies()
    {
        StartCoroutine(CheckInternetConnection());
        if (!hasInternetConnection)
        {
            Debug.Log("Network Reachability Not Reachable");
        }
        else
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); 
            if (!PlayerPrefs.HasKey("PlayerName"))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("*");
            }
            else
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerPrefs.GetString("PlayerName"));
            }
        }
        StartCoroutine(CheckInternetConnection());
    }

    public void UpdateLeaderboard()
    {
        StartCoroutine(_UpdateLeaderBoard());
    }
    private IEnumerator _UpdateLeaderBoard()
    {
        StartCoroutine(CheckInternetConnection());
        yield return new WaitForSeconds(1.5f);
        __UpdateLeaderboard();
    }
    public async void __UpdateLeaderboard()
    {
        StartCoroutine(CheckInternetConnection());
        if (hasInternetConnection)
        {
            leaderBoardGO.SetActive(true);
            noConectionGO.SetActive(false);
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

            if (PlayerPrefs.HasKey("PlayerName"))
            {
                ownRecordGO.SetActive(true);
                notRegisteredTextGO.SetActive(false);
            }
            else
            {
                ownRecordGO.SetActive(false);
                notRegisteredTextGO.SetActive(true);
            }
            // await Task.Delay(500);
            //}
        }
        else
        {
            Debug.Log("Network Reachability Not Reachable");
            leaderBoardGO.SetActive(false);
            noConectionGO.SetActive(true);
        }
        StartCoroutine(CheckInternetConnection());
    }


    public void CreateProfile()
    {
        StartCoroutine(CheckInternetConnection());
        playerName = UIManager.Instance.profileNameField.text;
        if (hasInternetConnection)
        {
            AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        PlayerPrefs.SetString("PlayerName", playerName);
        UIManager.Instance.profileNameField.text = "";
        CloseMenu();
        StartCoroutine(CheckInternetConnection());
    }

    public void CloseMenu()
    {
        UIManager.Instance.SetProfileScreenGOActive(false);
        UIManager.Instance.SetGoToCreditsBTNActive(true);
    }

    private bool hasInternetConnection = true;

    IEnumerator CheckInternetConnection()
    {
        UnityWebRequest www = new UnityWebRequest("https://api.ipify.org?format=json"); // Or your server
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            hasInternetConnection = false;
        }
        else
        {
            hasInternetConnection = true;
        }
    }

}
