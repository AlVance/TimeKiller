using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    //public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject[] levelsGO;
    [HideInInspector] public GameObject currentLevelGO;

    [SerializeField] private float startLevelTime = 3f;


    [Header("LevelTransition variables")]
    [SerializeField] private float timeRewardTime;
    [SerializeField] private GameObject levelTransSceneGO;
    [SerializeField] private GameObject startLevelGO;
    private bool canGoToLevelTrans = true;
    private bool inLevelTrans = false;
    private bool isStart = false;

    [SerializeField] private Slider[] recordTimeSavedSlidersLobby;
    [SerializeField] private TMP_Text recordTimeSavedText;
    [SerializeField] private TMP_Text mostCompletedLevelsText;

    private LeaderboardManager leaderboardManager;
    
    private void Start()
    {
        isStart = true;
        UIManager.Instance.SetInlevelUIActive(false);
        GoToStartLevel();
        leaderboardManager = GetComponent<LeaderboardManager>();
    }

    private void SetNextLevel()
    {
        if(currentLevelGO != null)
        {
            foreach (Transform go in currentLevelGO.transform)
            {
                StartCoroutine(DestroyGOWithDelay(go.gameObject));
            }
            Destroy(currentLevelGO);
            
            //++GameManager.Instance.currentLevel;
            
        }
        if(GameManager.Instance.currentLevel < levelsGO.Length)
        {
            currentLevelGO = Instantiate(levelsGO[GameManager.Instance.currentLevel]);
            GameManager.Instance.currentLevelGO = currentLevelGO;
            currentLevelGO.GetComponent<Level>().lM = this;

            StartCoroutine(OnLevelStartSetUp());
        }

    }

    private IEnumerator OnLevelStartSetUp()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.currentPlayer.gameObject.transform.position = currentLevelGO.GetComponent<Level>().playerStartTr.position;
        GameManager.Instance.currentPlayer.ResetPlayer();
        TimeManager.Instance.levelTime = currentLevelGO.GetComponent<Level>().levelTime;
        UIManager.Instance.SetLevelTimerSliderMaxValue(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetLevelTimeText(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetCurrentTimeText(TimeManager.Instance.currentTime);
        UIManager.Instance.SetObjectivesValueText(currentLevelGO.GetComponent<Level>().currentObjectives, currentLevelGO.GetComponent<Level>().objectivesToFinish);



        if (currentLevelGO.GetComponent<Level>().levelCam != null)
        {
            CameraManager.Instance.ChangeCam(currentLevelGO.GetComponent<Level>().levelCam);
        }
        else
        {
            CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
        }

        yield return new WaitForEndOfFrame();
        GameManager.Instance.UnloadMemory();
        yield return new WaitForEndOfFrame();
        UIManager.Instance.SetFade(false);

        
    }
    private bool canGoToStartLevelGameplay = true;
    public void StartLevelGameplay()
    {
        if(canGoToStartLevelGameplay) StartCoroutine(_StartLevelGameplay());
    }
    private IEnumerator _StartLevelGameplay()
    {
        canGoToStartLevelGameplay = false;
        UIManager.Instance.SetStartLevelBTNActive(false);
        UIManager.Instance.SetLevelOverviewActive(false);
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(true);
        if (GameManager.Instance.isMobile) UIManager.Instance.SetMobileGameplayControlsActive(true);

        for (int i = 0; i < startLevelTime; i++)
        {
            UIManager.Instance.SetStartLevelTimerText((startLevelTime - i).ToString("0"));
            yield return new WaitForSeconds(1f);
        }
        
        GameManager.Instance.levelStarted = true;
        UIManager.Instance.SetStartLevelTimerText("GO!");
        TimeManager.Instance.timerStarted = true;
        SoundManager.Instance.MusicOnOff(true);
        yield return new WaitForSeconds(1f);
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(false);
        canGoToStartLevelGameplay = true;

    }

    public void OnLevelEnded()
    {
        GameManager.Instance.levelStarted = false;
        GameManager.Instance.currentPlayer.BlockPlayer(0.2f);
        GameManager.Instance.currentPlayer.ResetPlayer();
        SetLevelPuntuationScreen();
    }

    public void SetLevelPuntuationScreen()
    {
        StartCoroutine(_SetLevelPuntuation());
    }

    private IEnumerator _SetLevelPuntuation()
    {
        if (GameManager.Instance.isMobile) UIManager.Instance.SetMobileGameplayControlsActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetPuntuationScreenActive(true);
        CameraManager.Instance.ChangeCam(CameraManager.Instance.winCam);
        GameManager.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0,-180,0);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", true);
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.SetTimerUIToWinScreen();
        yield return new WaitForSeconds(0.8f);
        if(GameManager.Instance.explorationMode)TimeManager.Instance.levelTime = 0.1f;
        float finalTimeValue = TimeManager.Instance.currentTime + TimeManager.Instance.levelTime;
        float _rewardTime = timeRewardTime;
        while (TimeManager.Instance.currentTime < finalTimeValue)
        {
            TimeManager.Instance.currentTime += _rewardTime * Time.deltaTime;
            TimeManager.Instance.levelTime -= _rewardTime * Time.deltaTime;
            _rewardTime += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        TimeManager.Instance.currentTime = finalTimeValue;
        TimeManager.Instance.levelTime = 0;
        canGoToLevelTrans = true;
        UIManager.Instance.SetGoToInBetweenBTNActive(true);
        if(!GameManager.Instance.explorationMode)PlayerPrefs.SetInt("Level_" + (GameManager.Instance.currentLevel - 1), 1);
        if (!GameManager.Instance.explorationMode && PlayerPrefs.GetInt("CompletedLevels") < GameManager.Instance.currentLevel) PlayerPrefs.SetInt("CompletedLevels", GameManager.Instance.currentLevel);
    }
    public void GoToInbetweenLevels()
    {
        if (canGoToLevelTrans)
        {
            StartCoroutine(_GoToInbetweenLevels());
        }
    }
    private IEnumerator _GoToInbetweenLevels()
    {
        canGoToLevelTrans = false;
        inLevelTrans = true;
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetGoToStartBTNActive(false);
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        if (isStart)
        {
            GameManager.Instance.playerWork = false;
            startLevelGO.SetActive(false);      
            isStart = false;
            GameManager.Instance.isInLobby = false;
            UIManager.Instance.SetInitialSceneUIActive(false);
        }
        levelTransSceneGO.SetActive(true);
        SoundManager.Instance.MusicOnOff(false);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.currentPlayer.anim.SetBool("IsLose", false);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", false);
        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);


        if ((GameManager.Instance.currentLevel < levelsGO.Length && !GameManager.Instance.explorationMode)
            || (GameManager.Instance.currentLevel < levelsGO.Length - 1 && GameManager.Instance.explorationMode))
        {
            SetNextLevel();
            UIManager.Instance.SetLevelCountText(GameManager.Instance.currentLevel + 1, levelsGO.Length);
            UIManager.Instance.SetLevelNameText(currentLevelGO.GetComponent<Level>().levelName);
            UIManager.Instance.SetInBetweenLevelsScreenActive(true);
            UIManager.Instance.SetGoToLevelBTNActive(true);
        }
        else
        {
            SetEndGameUIStuff();
            UIManager.Instance.SetFade(false);
            
        }
        ++GameManager.Instance.currentLevel;
    }
    private void SetEndGameUIStuff()
    {
        StartCoroutine(_SetEndGameUIStuff());
    }
    private IEnumerator _SetEndGameUIStuff()
    {
        UIManager.Instance.SetMostTimeSavedSliderActive(false);
        UIManager.Instance.SetTimeSavedSliderActive(false);
        UIManager.Instance.SetEndGameUIActive(true);
        if (!GameManager.Instance.explorationMode)
        {
            yield return new WaitForSeconds(1f);
            if (!PlayerPrefs.HasKey("MostTimeSaved") || TimeManager.Instance.currentTime > PlayerPrefs.GetFloat("MostTimeSaved"))
            {

                UIManager.Instance.SetTimeSavedSlidiers(0);

                UIManager.Instance.SetTimeSavedSliderActive(true);
                yield return new WaitForSeconds(0.5f);

                float currentTimeB = 0;
                float advanceTimeB = 0.05f;
                while (currentTimeB < TimeManager.Instance.currentTime)
                {
                    UIManager.Instance.SetTimeSavedSlidiers(currentTimeB);
                    currentTimeB += advanceTimeB;
                    advanceTimeB += 0.0007f;
                    yield return new WaitForEndOfFrame();
                }
                UIManager.Instance.SetTimeSavedSlidiers(TimeManager.Instance.currentTime);

                PlayerPrefs.SetFloat("MostTimeSaved", TimeManager.Instance.currentTime);
                UIManager.Instance.SetMostTimeSavedSlidiers(0);
                yield return new WaitForSeconds(1f);
                UIManager.Instance.SetMostTimeSavedSliderActive(true);
                yield return new WaitForSeconds(0.5f);
                float currentTimeA = 0;
                float advanceTimeA = 0.05f;
                while (currentTimeA < TimeManager.Instance.currentTime)
                {
                    UIManager.Instance.SetMostTimeSavedSlidiers(currentTimeA);
                    currentTimeA += advanceTimeA;
                    advanceTimeA += 0.0007f;
                    yield return new WaitForEndOfFrame();
                }
                UIManager.Instance.SetMostTimeSavedSlidiers(TimeManager.Instance.currentTime);

                UIManager.Instance.SetNewRecordTextActive(true);
                //NEW RECORD!
            }
            else
            {
                UIManager.Instance.SetTimeSavedSlidiers(0);
                UIManager.Instance.SetTimeSavedSliderActive(true);
                yield return new WaitForSeconds(0.5f);

                float currentTimeB = 0;
                float advanceTimeB = 0.05f;
                while (currentTimeB < TimeManager.Instance.currentTime)
                {
                    UIManager.Instance.SetTimeSavedSlidiers(currentTimeB);
                    currentTimeB += advanceTimeB;
                    advanceTimeB += 0.0007f;
                    yield return new WaitForEndOfFrame();
                }
                UIManager.Instance.SetTimeSavedSlidiers(TimeManager.Instance.currentTime);

                UIManager.Instance.SetMostTimeSavedSlidiers(0);
                yield return new WaitForSeconds(1f);
                UIManager.Instance.SetMostTimeSavedSliderActive(true);
                yield return new WaitForSeconds(0.5f);

                float currentTimeA = 0;
                float advanceTimeA = 0.05f;
                while (currentTimeA < PlayerPrefs.GetFloat("MostTimeSaved"))
                {
                    UIManager.Instance.SetMostTimeSavedSlidiers(currentTimeA);
                    currentTimeA += advanceTimeA;
                    advanceTimeA += 0.0007f;
                    yield return new WaitForEndOfFrame();
                }
                UIManager.Instance.SetMostTimeSavedSlidiers(PlayerPrefs.GetFloat("MostTimeSaved"));
            }

            if (!PlayerPrefs.HasKey("PlayerName"))
            {
                yield return new WaitForSeconds(0.5f);
                UIManager.Instance.SetProfileScreenGOActive(true);
            }
        }
        else
        {
            UIManager.Instance.SetEndGameExplorationTextActive(true);
        }


        if (!PlayerPrefs.HasKey("PlayerName") && !GameManager.Instance.explorationMode)
        {
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.SetProfileScreenGOActive(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            UIManager.Instance.SetGoToCreditsBTNActive(true);
        }   
    }

    private bool canGoToCredits = true;
    public void GoToCredits()
    {
        if(canGoToCredits) StartCoroutine(_GoToCredits());
    }
    private IEnumerator _GoToCredits()
    {
        canGoToCredits = false;
        UIManager.Instance.SetGoToCreditsBTNActive(false);
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1.5f);
        SoundManager.Instance.LobbyMusicOnOff(true);
        levelTransSceneGO.SetActive(false);
        UIManager.Instance.SetCreditsScreenActive(true);
        yield return new WaitForEndOfFrame();
        UIManager.Instance.SetFade(false);
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.SetGoToStartBTNActive(true);
        canGoToCredits = true;
    }

    private bool canGoToLevelOverview = true;
    public void GoToLevelOverview()
    {
        if(canGoToLevelOverview) StartCoroutine(_GoToLevelOverview());
    }
    private IEnumerator _GoToLevelOverview()
    {
        canGoToLevelOverview = false;
        UIManager.Instance.SetGoToLevelBTNActive(false);
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        inLevelTrans = false;
        levelTransSceneGO.SetActive(false);
        UIManager.Instance.SetInBetweenLevelsScreenActive(false);
        UIManager.Instance.SetLevelOverviewActive(true);
        UIManager.Instance.SetInlevelUIActive(true);

        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetStartLevelBTNActive(true);
        canGoToLevelOverview = true;
    }

    private bool canGoToStartLevel = true;
    public void GoToStartLevel()
    {
        if(canGoToStartLevel) StartCoroutine(_GoToStartLevel());
    }
    private IEnumerator _GoToStartLevel()
    {
        canGoToStartLevel = false;
        isStart = true;
        GameManager.Instance.isInLobby = true;
        UIManager.Instance.SetFade(true);
        SoundManager.Instance.LobbyMusicOnOff(true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.currentLevel = 0;
        TimeManager.Instance.currentTime = TimeManager.Instance.startTime;
        //UIManager.Instance.SetGoToStartBTNActive(true);
        UIManager.Instance.SetCreditsScreenActive(false);
        UIManager.Instance.SetEndGameUIActive(false);
        levelTransSceneGO.SetActive(false);
        startLevelGO.SetActive(true);
        GameManager.Instance.currentLevelGO = startLevelGO;
        for (int i = 0; i < recordTimeSavedSlidersLobby.Length; i++)
        {
            recordTimeSavedSlidersLobby[i].value = PlayerPrefs.GetFloat("MostTimeSaved");
        }
        recordTimeSavedText.text = PlayerPrefs.GetFloat("MostTimeSaved").ToString("0.00");
        mostCompletedLevelsText.text = PlayerPrefs.GetInt("CompletedLevels").ToString() + "/ 10";
        leaderboardManager.UpdateLeaderboard();

        yield return new WaitForEndOfFrame();
        GOLoaderByPlayerPrefs[] SL = startLevelGO.GetComponentsInChildren<GOLoaderByPlayerPrefs>();
        foreach (GOLoaderByPlayerPrefs sl in SL)
        {
            sl.SetStickerActive();
        }
        yield return new WaitForEndOfFrame();
        if (currentLevelGO != null)
        {
            foreach (Transform go in currentLevelGO.transform)
            {
                StartCoroutine(DestroyGOWithDelay(go.gameObject));
            }
            Destroy(currentLevelGO);
            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.currentPlayer.gameObject.transform.position = startLevelGO.GetComponent<Level>().playerStartTr.position;
        GameManager.Instance.currentPlayer.ResetPlayer();
        CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
        GameManager.Instance.currentPlayer.anim.SetBool("IsLose", false);
        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);

        UIManager.Instance.SetPuntuationScreenActive(false);
        UIManager.Instance.SetGameOverScreenctive(false);
        UIManager.Instance.SetInitialSceneUIActive(true);
        if (GameManager.Instance.isMobile) UIManager.Instance.SetMobileGameplayControlsActive(true);
        canGoToLevelTrans = true;

        yield return new WaitForEndOfFrame();
        GameManager.Instance.UnloadMemory();
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
        GameManager.Instance.playerWork = true;
        canGoToStartLevel = true;
    }
    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }
}
