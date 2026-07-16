using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  SERIALIZED FIELDS
    // ─────────────────────────────────────────────

    [SerializeField] public World[] worlds;

    [Header("Level Setup")]
    [SerializeField] private float startLevelTime = 3f;
    [SerializeField] private float timeRewardTime;
    [SerializeField] private GameObject levelTransSceneGO;
    [SerializeField] private GameObject startLevelGO;

    [Header("Audio")]
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip countdown;
    [SerializeField] private AudioClip countdownEnd;

    [Header("Lobby UI")]
    [SerializeField] private Slider[] recordTimeSavedSlidersLobby;
    [SerializeField] private TMP_Text recordTimeSavedText;
    [SerializeField] private TMP_Text mostCompletedLevelsText;

    // ─────────────────────────────────────────────
    //  PUBLIC STATE
    // ─────────────────────────────────────────────

    public int currentWorld = 0;
    public bool isInWorldSelect = false;

    [HideInInspector] public GameObject currentLevelGO;

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private WorldSelector worldSelector;
    private LeaderboardManager leaderboardManager;

    // Single transition guard — replaces six individual canGoTo* bools.
    // Set to false at the start of every transition coroutine;
    // set back to true at the end (or left true for button-driven flows).
    private bool isTransitioning = false;
    private bool canGoToLevelTrans = true;  // separate: gated by level-end score animation
    private bool inLevelTrans = false;


    private PlayerInput1 playerInput;

    // ─────────────────────────────────────────────
    //  INIT
    // ─────────────────────────────────────────────

    private void Awake()
    {
        playerInput = new PlayerInput1();
        playerInput.UI.Backward.performed += ctx =>
        {
            if (inLevelTrans && GameManager.Instance.currentLevel == 1) GameManager.Instance.ChangeGameMode();
            Debug.Log("AAA");
        };
    }

    private void Start()
    {
        leaderboardManager = GetComponent<LeaderboardManager>();
        worldSelector = GetComponent<WorldSelector>();
        worldSelector.enabled = false;

        UIManager.Instance.SetInlevelUIActive(false);
        GoToStartLevel();
    }

    // ─────────────────────────────────────────────
    //  TRANSITION GUARD
    // ─────────────────────────────────────────────

    /// <summary>
    /// Starts a coroutine only if no transition is already running.
    /// Returns true if the routine was started.
    /// </summary>
    private bool TryStartTransition(IEnumerator routine)
    {
        while (isTransitioning) return false;
        isTransitioning = true;
        StartCoroutine(routine);
        return true;
    }

    // ─────────────────────────────────────────────
    //  LEVEL LOADING
    // ─────────────────────────────────────────────

    private void SetNextLevel()
    {
        DestroyCurrentLevel();

        if (GameManager.Instance.currentLevel < worlds[currentWorld].worldLevelsGO.Length)
        {
            currentLevelGO = Instantiate(worlds[currentWorld].worldLevelsGO[GameManager.Instance.currentLevel]);
            GameManager.Instance.currentLevelGO = currentLevelGO;
            currentLevelGO.GetComponent<Level>().lM = this;
            StartCoroutine(OnLevelStartSetUp());
        }
    }

    private IEnumerator OnLevelStartSetUp()
    {
        yield return new WaitForEndOfFrame();

        // Cache the Level component — avoids 6+ repeated GetComponent calls
        Level level = currentLevelGO.GetComponent<Level>();

        GameManager.Instance.currentPlayer.transform.position = level.playerStartTr.position;

        TimeManager.Instance.levelTime = level.levelTime;
        UIManager.Instance.SetLevelTimerSliderMaxValue(level.levelTime);
        UIManager.Instance.SetLevelTimeText(level.levelTime);
        UIManager.Instance.SetCurrentTimeText(TimeManager.Instance.currentTime);
        UIManager.Instance.SetObjectivesValueText(level.currentObjectives, level.objectivesToFinish);

        if (level.levelCam != null)
        {
            CameraManager.Instance.ChangeCam(level.levelCam);
            CameraManager.Instance.levelCamera = level.levelCam;
        }
        else
        {
            CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
        }

        CameraManager.Instance.gameObject.GetComponent<CamerasFOVController>().GetLevelCams();
        GameManager.Instance.UnloadMemory();
        UIManager.Instance.SetFade(false);
    }

    // ─────────────────────────────────────────────
    //  GAMEPLAY START (countdown)
    // ─────────────────────────────────────────────

    public void StartLevelGameplay()
    {
        TryStartTransition(_StartLevelGameplay());
    }

    private IEnumerator _StartLevelGameplay()
    {
        UIManager.Instance.SetStartLevelBTNActive(false);
        UIManager.Instance.SetLevelOverviewActive(false);
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(true);

        if (GameManager.Instance.isMobile)
            UIManager.Instance.SetMobileGameplayControlsActive(true);

        for (int i = 0; i < startLevelTime; i++)
        {
            UIManager.Instance.SetStartLevelTimerText((startLevelTime - i).ToString("0"));
            SoundManager.Instance.PlayOneShootAudio(countdown);
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().UnblockPlayer();
        GameManager.Instance.levelStarted = true;
        SoundManager.Instance.PlayOneShootAudio(countdownEnd);
        UIManager.Instance.SetStartLevelTimerText("GO!");
        TimeManager.Instance.timerStarted = true;
        SoundManager.Instance.MusicOnOff(true);

        yield return new WaitForSeconds(1f);
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(false);

        isTransitioning = false;
    }

    // ─────────────────────────────────────────────
    //  LEVEL END
    // ─────────────────────────────────────────────

    public void OnLevelEnded()
    {
        GameManager.Instance.levelStarted = false;
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
        SetLevelPuntuationScreen();
    }

    public void SetLevelPuntuationScreen()
    {
        StartCoroutine(_SetLevelPuntuation());
    }

    private IEnumerator _SetLevelPuntuation()
    {
        if (GameManager.Instance.isMobile)
            UIManager.Instance.SetMobileGameplayControlsActive(false);

        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetPuntuationScreenActive(true);
        CameraManager.Instance.ChangeCam(CameraManager.Instance.winCam);
        GameManager.Instance.currentPlayer.transform.eulerAngles = new Vector3(0, -180, 0);
        SoundManager.Instance.PlayOneShootAudio(victorySound);

        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.SetTimerUIToWinScreen();
        yield return new WaitForSeconds(0.8f);

        if (GameManager.Instance.explorationMode)
            TimeManager.Instance.levelTime = 0.1f;

        yield return StartCoroutine(DrainLevelTimeIntoScore());

        canGoToLevelTrans = true;
        UIManager.Instance.SetGoToInBetweenBTNActive(true);

        if (!GameManager.Instance.explorationMode)
        {
            PlayerPrefs.SetInt("CD_Level_" + (GameManager.Instance.currentLevel - 1), 1);

            if (PlayerPrefs.GetInt("CD_CompletedLevels") < GameManager.Instance.currentLevel)
                PlayerPrefs.SetInt("CD_CompletedLevels", GameManager.Instance.currentLevel);
        }
    }

    /// <summary>
    /// Animates the remaining level time draining into the player's accumulated score.
    /// </summary>
    private IEnumerator DrainLevelTimeIntoScore()
    {
        float target = TimeManager.Instance.currentTime + TimeManager.Instance.levelTime;
        float rewardRate = timeRewardTime;

        while (TimeManager.Instance.currentTime < target)
        {
            TimeManager.Instance.currentTime += rewardRate * Time.deltaTime;
            TimeManager.Instance.levelTime -= rewardRate * Time.deltaTime;
            rewardRate += 0.01f;
            yield return new WaitForEndOfFrame();
        }

        TimeManager.Instance.currentTime = target;
        TimeManager.Instance.levelTime = 0;
    }

    // ─────────────────────────────────────────────
    //  NAVIGATION — WORLD SELECT
    // ─────────────────────────────────────────────

    public void GoToWorldSelect()
    {
        TryStartTransition(_GoToWorldSelect());
    }

    private IEnumerator _GoToWorldSelect()
    {
        isInWorldSelect = true;
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);

        if (GameManager.Instance.isInLobby)
            LeaveLobby();

        worldSelector.enabled = true;
        UIManager.Instance.SetSelectWorldScreenGOActive(true);
        UIManager.Instance.SetFade(false);

        isTransitioning = false;
    }

    // ─────────────────────────────────────────────
    //  NAVIGATION — IN-BETWEEN LEVELS
    // ─────────────────────────────────────────────

    public void GoToInbetweenLevels()
    {
        if (canGoToLevelTrans)
            TryStartTransition(_GoToInbetweenLevels());
    }

    private IEnumerator _GoToInbetweenLevels()
    {
        canGoToLevelTrans = false;
        isInWorldSelect = false;
        inLevelTrans = true;

        worldSelector.enabled = false;
        UIManager.Instance.SetSelectWorldScreenGOActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetGoToStartBTNActive(false);
        UIManager.Instance.SetFade(true);

        yield return new WaitForSeconds(1f);

        if (GameManager.Instance.isInLobby)
            LeaveLobby();

        levelTransSceneGO.SetActive(true);
        SoundManager.Instance.MusicOnOff(false);

        yield return new WaitForSeconds(1f);

        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);
        if(GameManager.Instance.currentLevel == 0) UIManager.Instance.SetModeSelectTextGOActive(true);
        else UIManager.Instance.SetModeSelectTextGOActive(false);


        if (GameManager.Instance.currentLevel < worlds[currentWorld].worldLevelsGO.Length)
        {
            SetNextLevel();
            UIManager.Instance.SetLevelCountText(GameManager.Instance.currentLevel + 1, worlds[currentWorld].worldLevelsGO.Length);
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
        isTransitioning = false;

    }

    // ─────────────────────────────────────────────
    //  NAVIGATION — LEVEL OVERVIEW
    // ─────────────────────────────────────────────

    public void GoToLevelOverview()
    {
        TryStartTransition(_GoToLevelOverview());
    }

    private IEnumerator _GoToLevelOverview()
    {
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

        isTransitioning = false;
    }

    // ─────────────────────────────────────────────
    //  NAVIGATION — START / LOBBY
    // ─────────────────────────────────────────────

    public void GoToStartLevel()
    {
        TryStartTransition(_GoToStartLevel());
    }

    private IEnumerator _GoToStartLevel()
    {
        GameManager.Instance.isInLobby = true;
        UIManager.Instance.SetFade(true);
        SoundManager.Instance.LobbyMusicOnOff(true);

        yield return new WaitForSeconds(1f);

        // Reset game state
        GameManager.Instance.currentLevel = 0;
        TimeManager.Instance.currentTime = TimeManager.Instance.startTime;
        canGoToLevelTrans = true;

        // Reset UI panels
        UIManager.Instance.SetCreditsScreenActive(false);
        UIManager.Instance.SetEndGameUIActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);
        UIManager.Instance.SetGameOverScreenctive(false);
        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);

        // Activate lobby scene
        levelTransSceneGO.SetActive(false);
        startLevelGO.SetActive(true);
        GameManager.Instance.currentLevelGO = startLevelGO;

        // Populate lobby stats
        UpdateLobbyStatsUI();
        leaderboardManager.UpdateLeaderboard();

        yield return new WaitForEndOfFrame();

        foreach (GOLoaderByPlayerPrefs sl in startLevelGO.GetComponentsInChildren<GOLoaderByPlayerPrefs>())
            sl.SetStickerActive();

        yield return new WaitForEndOfFrame();

        // Clean up any leftover level
        yield return StartCoroutine(CleanupCurrentLevelAsync());

        // Place player and restore camera
        GameManager.Instance.currentPlayer.transform.position = startLevelGO.GetComponent<Level>().playerStartTr.position;
        CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);

        UIManager.Instance.SetInitialSceneUIActive(true);

        if (GameManager.Instance.isMobile)
            UIManager.Instance.SetMobileGameplayControlsActive(true);

        yield return new WaitForEndOfFrame();
        GameManager.Instance.UnloadMemory();
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);

        GameManager.Instance.playerWork = true;
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().UnblockPlayer();

        isTransitioning = false;
    }

    // ─────────────────────────────────────────────
    //  NAVIGATION — CREDITS
    // ─────────────────────────────────────────────

    public void GoToCredits()
    {
        TryStartTransition(_GoToCredits());
    }

    private IEnumerator _GoToCredits()
    {
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

        isTransitioning = false;
    }

    // ─────────────────────────────────────────────
    //  END GAME SCREEN
    // ─────────────────────────────────────────────

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

            float currentScore = TimeManager.Instance.currentTime;
            bool isNewRecord = !PlayerPrefs.HasKey("CD_MostTimeSaved") ||
                                  currentScore > PlayerPrefs.GetFloat("CD_MostTimeSaved");

            // Animate current run score
            UIManager.Instance.SetTimeSavedSlidiers(0);
            UIManager.Instance.SetTimeSavedSliderActive(true);
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(AnimateSliderTo(currentScore, UIManager.Instance.SetTimeSavedSlidiers));

            // Animate record slider
            UIManager.Instance.SetMostTimeSavedSlidiers(0);
            yield return new WaitForSeconds(1f);
            UIManager.Instance.SetMostTimeSavedSliderActive(true);
            yield return new WaitForSeconds(0.5f);

            if (isNewRecord)
            {
                PlayerPrefs.SetFloat("CD_MostTimeSaved", currentScore);
                yield return StartCoroutine(AnimateSliderTo(currentScore, UIManager.Instance.SetMostTimeSavedSlidiers));
                UIManager.Instance.SetNewRecordTextActive(true);
            }
            else
            {
                yield return StartCoroutine(AnimateSliderTo(PlayerPrefs.GetFloat("CD_MostTimeSaved"),
                                                            UIManager.Instance.SetMostTimeSavedSlidiers));
            }

            if (!PlayerPrefs.HasKey("CD_PlayerName"))
            {
                yield return new WaitForSeconds(0.5f);
                UIManager.Instance.SetProfileScreenGOActive(true);
                yield break; // navigation continues from UI button
            }
        }
        else
        {
            UIManager.Instance.SetEndGameExplorationTextActive(true);
        }

        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetGoToCreditsBTNActive(true);
    }


    public void StartRun()
    {
        StartCoroutine(_StartRun());
    }

    private IEnumerator _StartRun()
    {
        GameManager.Instance.levelStarted = false;
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
        GameManager.Instance.currentLevel = 0;
        TimeManager.Instance.currentTime = TimeManager.Instance.startTime;
        canGoToLevelTrans = true;
        yield return new WaitForEndOfFrame();
        GoToInbetweenLevels();
    }
    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    /// <summary>
    /// Animates a UI value from 0 to targetValue with accelerating speed.
    /// Accepts any setter that takes a float — e.g. UIManager.Instance.SetTimeSavedSlidiers.
    /// Replaces the two identical while-loop blocks that were duplicated in _SetEndGameUIStuff.
    /// </summary>
    private IEnumerator AnimateSliderTo(float targetValue, System.Action<float> setter)
    {
        float current = 0f;
        float step = 0.05f;

        while (current < targetValue)
        {
            setter(current);
            current += step;
            step += 0.0007f;
            yield return new WaitForEndOfFrame();
        }

        setter(targetValue);
    }

    /// <summary>
    /// Tears down the lobby scene and marks the game as no longer in the lobby.
    /// Replaces the duplicated isStart block that appeared in _GoToWorldSelect and _GoToInbetweenLevels.
    /// </summary>
    private void LeaveLobby()
    {
        GameManager.Instance.playerWork = false;
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
        startLevelGO.SetActive(false);
        GameManager.Instance.isInLobby = false;
        UIManager.Instance.SetInitialSceneUIActive(false);
    }

    /// <summary>
    /// Destroys the current level and its children. Synchronous version for SetNextLevel.
    /// </summary>
    private void DestroyCurrentLevel()
    {
        if (currentLevelGO == null) return;

        foreach (Transform child in currentLevelGO.transform)
            StartCoroutine(DestroyWithDelay(child.gameObject));

        Destroy(currentLevelGO);
        currentLevelGO = null;
    }

    /// <summary>
    /// Coroutine version of DestroyCurrentLevel, for use inside other coroutines.
    /// </summary>
    private IEnumerator CleanupCurrentLevelAsync()
    {
        if (currentLevelGO == null) yield break;

        foreach (Transform child in currentLevelGO.transform)
            StartCoroutine(DestroyWithDelay(child.gameObject));

        Destroy(currentLevelGO);
        currentLevelGO = null;

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DestroyWithDelay(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        Destroy(go);
    }

    /// <summary>
    /// Reads lobby stats from PlayerPrefs and updates the lobby UI panel.
    /// </summary>
    private void UpdateLobbyStatsUI()
    {
        float savedTime = PlayerPrefs.GetFloat("CD_MostTimeSaved");
        int completedLevels = PlayerPrefs.GetInt("CD_CompletedLevels");

        foreach (Slider s in recordTimeSavedSlidersLobby)
            s.value = savedTime;

        recordTimeSavedText.text = savedTime.ToString("0.00");
        mostCompletedLevelsText.text = completedLevels + " / 6";
    }

    private void OnEnable()
    {
        playerInput.UI.Enable();
    }

    private void OnDisable()
    {
        playerInput.UI.Disable();
    }
}


// ─────────────────────────────────────────────
//  DATA
// ─────────────────────────────────────────────

[System.Serializable]
public class World
{
    [SerializeField] public GameObject[] worldLevelsGO;
}
/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public World[] worlds;
    public int currentWorld = 0;
    private WorldSelector worldSelector;
    //[SerializeField] private GameObject[] levelsGO;
    [HideInInspector] public GameObject currentLevelGO;

    [SerializeField] private float startLevelTime = 3f;
    [SerializeField] private AudioClip VictorySound;
    [SerializeField] private AudioClip Countdown;
    [SerializeField] private AudioClip CountdownEnd;


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
        worldSelector = this.gameObject.GetComponent<WorldSelector>();
        worldSelector.enabled = false;
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
             
        }
        if(GameManager.Instance.currentLevel < worlds[currentWorld].worldLevelsGO.Length)
        {
            currentLevelGO = Instantiate(worlds[currentWorld].worldLevelsGO[GameManager.Instance.currentLevel]);
            GameManager.Instance.currentLevelGO = currentLevelGO;
            currentLevelGO.GetComponent<Level>().lM = this;

            StartCoroutine(OnLevelStartSetUp());
        }

    }

    private IEnumerator OnLevelStartSetUp()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.currentPlayer.gameObject.transform.position = currentLevelGO.GetComponent<Level>().playerStartTr.position;
        //GameManager.Instance.currentPlayer.ResetPlayer();
        TimeManager.Instance.levelTime = currentLevelGO.GetComponent<Level>().levelTime;
        UIManager.Instance.SetLevelTimerSliderMaxValue(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetLevelTimeText(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetCurrentTimeText(TimeManager.Instance.currentTime);
        UIManager.Instance.SetObjectivesValueText(currentLevelGO.GetComponent<Level>().currentObjectives, currentLevelGO.GetComponent<Level>().objectivesToFinish);



        if (currentLevelGO.GetComponent<Level>().levelCam != null)
        {
            CameraManager.Instance.ChangeCam(currentLevelGO.GetComponent<Level>().levelCam);
            CameraManager.Instance.levelCamera = currentLevelGO.GetComponent<Level>().levelCam;
        }
        else
        {
            CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
        }
        CameraManager.Instance.gameObject.GetComponent<CamerasFOVController>().GetLevelCams();

        GameManager.Instance.UnloadMemory();
        
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
            SoundManager.Instance.PlayOneShootAudio(Countdown);

            yield return new WaitForSeconds(1f);
        }
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().UnblockPlayer();
        GameManager.Instance.levelStarted = true;
        SoundManager.Instance.PlayOneShootAudio(CountdownEnd);
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
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
        //GameManager.Instance.currentPlayer.ResetPlayer();
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
        //GameManager.Instance.currentPlayer.anim.SetBool("IsWin", true);
        SoundManager.Instance.PlayOneShootAudio(VictorySound);
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

    private bool canGoToWorldSelect = true;
    [HideInInspector]public bool isInWorldSelect = false;
    public void GoToWorldSelect()
    {
        if (canGoToWorldSelect)
        {
            StartCoroutine(_GoToWorldSelect());
        }
    }
    private IEnumerator _GoToWorldSelect()
    {
        canGoToWorldSelect = false;
        isInWorldSelect = true;
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        if (isStart)
        {
            GameManager.Instance.playerWork = false;
            GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
            startLevelGO.SetActive(false);
            isStart = false;
            GameManager.Instance.isInLobby = false;
            UIManager.Instance.SetInitialSceneUIActive(false);
        }
        worldSelector.enabled = true;
        UIManager.Instance.SetSelectWorldScreenGOActive(true);
        UIManager.Instance.SetFade(false);
        canGoToWorldSelect = true;
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
        isInWorldSelect = false;
        worldSelector.enabled = false;
        inLevelTrans = true;
        UIManager.Instance.SetSelectWorldScreenGOActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetGoToStartBTNActive(false);
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        if (isStart)
        {
            GameManager.Instance.playerWork = false;
            GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().BlockPlayer();
            startLevelGO.SetActive(false);      
            isStart = false;
            GameManager.Instance.isInLobby = false;
            UIManager.Instance.SetInitialSceneUIActive(false);
        }
        levelTransSceneGO.SetActive(true);
        SoundManager.Instance.MusicOnOff(false);
        yield return new WaitForSeconds(1f);
        //GameManager.Instance.currentPlayer.anim.SetBool("IsLose", false);
        //GameManager.Instance.currentPlayer.anim.SetBool("IsWin", false);
        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);


        if ((GameManager.Instance.currentLevel < worlds[currentWorld].worldLevelsGO.Length))
        {
            SetNextLevel();
            UIManager.Instance.SetLevelCountText(GameManager.Instance.currentLevel + 1, worlds[currentWorld].worldLevelsGO.Length);
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
        //GameManager.Instance.currentPlayer.ResetPlayer();
        CameraManager.Instance.ChangeCam(CameraManager.Instance.basePlayerCam);
        //GameManager.Instance.currentPlayer.anim.SetBool("IsLose", false);
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
        GameManager.Instance.currentPlayer.GetComponent<PlayerBlock>().UnblockPlayer();
        canGoToStartLevel = true;
    }
    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }
}

[System.Serializable]
public class World
{
    [SerializeField] public GameObject[] worldLevelsGO;
}
*/