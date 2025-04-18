using UnityEngine;
using System.Collections;

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
    private void Start()
    {
        isStart = true;
        UIManager.Instance.SetInlevelUIActive(false);
        GoToStartLevel();
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

        UIManager.Instance.SetFade(false);
    }
    public void StartLevelGameplay()
    {
        StartCoroutine(_StartLevelGameplay());
    }
    private IEnumerator _StartLevelGameplay()
    {
        UIManager.Instance.SetStartLevelBTNActive(false);
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
        if (Application.isMobilePlatform) UIManager.Instance.SetMobileGameplayControlsActive(false);
        UIManager.Instance.SetGoToInBetweenBTNActive(false);
        UIManager.Instance.SetPuntuationScreenActive(true);
        CameraManager.Instance.ChangeCam(CameraManager.Instance.winCam);
        GameManager.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0,-180,0);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", true);
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
        }
        levelTransSceneGO.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.currentPlayer.anim.SetBool("IsLose", false);
        GameManager.Instance.currentPlayer.anim.SetBool("IsWin", false);
        UIManager.Instance.SetTimerUIToIdle();
        UIManager.Instance.SetInlevelUIActive(false);
        UIManager.Instance.SetPuntuationScreenActive(false);
        
        if (GameManager.Instance.currentLevel < levelsGO.Length)
        {
            SetNextLevel();
            UIManager.Instance.SetLevelCountText(GameManager.Instance.currentLevel + 1, levelsGO.Length);
            UIManager.Instance.SetLevelNameText(currentLevelGO.GetComponent<Level>().levelName);
            UIManager.Instance.SetInBetweenLevelsScreenActive(true);
            UIManager.Instance.SetGoToLevelBTNActive(true);
        }
        else
        {
            UIManager.Instance.SetFade(false);
            yield return new WaitForSeconds(1f);
            UIManager.Instance.SetGoToStartBTNActive(true);
        }
        ++GameManager.Instance.currentLevel;
    }
    public void GoToLevelOverview()
    {
        StartCoroutine(_GoToLevelOverview());
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
    }

    public void GoToStartLevel()
    {
        StartCoroutine(_GoToStartLevel());
    }
    private IEnumerator _GoToStartLevel()
    {
        isStart = true;
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        GameManager.Instance.currentLevel = 0;
        TimeManager.Instance.currentTime = TimeManager.Instance.startTime;
        //UIManager.Instance.SetGoToStartBTNActive(true);
        levelTransSceneGO.SetActive(false);
        startLevelGO.SetActive(true);
        yield return new WaitForEndOfFrame();
        if(currentLevelGO != null)
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
        if (Application.isMobilePlatform) UIManager.Instance.SetMobileGameplayControlsActive(true);
        canGoToLevelTrans = true;
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
        GameManager.Instance.playerWork = true;
    }
    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }
}
