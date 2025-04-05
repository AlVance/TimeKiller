using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject[] levelsGO;
    [HideInInspector] public GameObject currentLevelGO;
    private int currentLevel = 0;

    [SerializeField] private float startLevelTime = 3f;


    [Header("LevelTransition variables")]
    [SerializeField] private float timeRewardTime;
    [SerializeField] private GameObject levelTransSceneGO;
    private bool canGoToLevelTrans = true;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    private void Start()
    {
        GoToInbetweenLevels();
    }
    private void SetNextLevel()
    {
        if(currentLevelGO != null)
        {
            ++currentLevel;
            if (currentLevel >= levelsGO.Length) currentLevel = 0; //TEMP LOOP

            foreach(Transform go in currentLevelGO.transform)
            {
                StartCoroutine(DestroyGOWithDelay(go.gameObject));
            }
            Destroy(currentLevelGO);
        }
        
        currentLevelGO = Instantiate(levelsGO[currentLevel]);
        currentLevelGO.GetComponent<Level>().lM = this;

        StartCoroutine(OnLevelStartSetUp());
    }

    private IEnumerator OnLevelStartSetUp()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.currentPlayer.gameObject.transform.position = currentLevelGO.GetComponent<Level>().playerStartTr.position;
        GameManager.Instance.currentPlayer.ResetPlayer();
        //GameManager.Instance.currentPlayer.enabled = false;
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

        UIManager.Instance.SetFade(false);
    }
    public void StartLevelGameplay()
    {
        StartCoroutine(_StartLevelGameplay());
    }
    private IEnumerator _StartLevelGameplay()
    {
        UIManager.Instance.SetLevelOverviewActive(false);
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(true);
        if (Application.isMobilePlatform) UIManager.Instance.SetMobileGameplayControlsActive(true);

        for (int i = 0; i < startLevelTime; i++)
        {
            UIManager.Instance.SetStartLevelTimerText((startLevelTime - i).ToString("0"));
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.startLevelTimerText.gameObject.SetActive(false);
        GameManager.Instance.levelStarted = true;
    }

    public void OnLevelEnded()
    {
        GameManager.Instance.levelStarted = false;
        GameManager.Instance.currentPlayer.ResetPlayer();
        SetLevelPuntuationScreen();
    }

    public void SetLevelPuntuationScreen()
    {
        StartCoroutine(_SetLevelPuntuation());
    }

    private IEnumerator _SetLevelPuntuation()
    {
        if (Application.isMobilePlatform) UIManager.Instance.SetMobileGameplayControlsActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetPuntuationScreenActive(true);
        CameraManager.Instance.ChangeCam(CameraManager.Instance.winCam);
        GameManager.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0,-180,0);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", true);
        //UIManager.Instance.SetTimerUIToIdle();
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.SetTimerUIToWinScreen();
        yield return new WaitForSeconds(0.8f);
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
        //TimeManager.Instance.currentTime += TimeManager.Instance.levelTime;
        TimeManager.Instance.levelTime = 0;
        canGoToLevelTrans = true;
        UIManager.Instance.SetGoToInBetweenBTNActive(true);
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
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        levelTransSceneGO.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", false);
        UIManager.Instance.SetTimerUIToIdle();
        SetNextLevel();
        UIManager.Instance.SetInlevelUIActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);
        UIManager.Instance.SetLevelCountText(currentLevel + 1, levelsGO.Length);
        UIManager.Instance.SetLevelNameText(currentLevelGO.GetComponent<Level>().levelName);
        UIManager.Instance.SetInBetweenLevelsScreenActive(true);
  
    }
    public void GoToLevelOverview()
    {
        StartCoroutine(_GoToLevelOverview());
    }
    private IEnumerator _GoToLevelOverview()
    {
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        levelTransSceneGO.SetActive(false);
        UIManager.Instance.SetInBetweenLevelsScreenActive(false);
        UIManager.Instance.SetLevelOverviewActive(true);
        UIManager.Instance.SetInlevelUIActive(true);
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
    }
    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }

}
