using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject[] levelsGO;
    [HideInInspector] public GameObject currentLevelGO;
    private int currentLevel = 0;

    [SerializeField] private float startLevelTime = 3f;

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

        if (currentLevelGO.GetComponent<Level>().levelCamValues != null && 
            (currentLevelGO.GetComponent<Level>().levelCamValues.position != Vector3.zero || currentLevelGO.GetComponent<Level>().levelCamValues.rotation != Quaternion.Euler(Vector3.zero)))
        {
            Camera.main.gameObject.GetComponent<FollowObject>().followOffset = currentLevelGO.GetComponent<Level>().levelCamValues.position;
            Camera.main.gameObject.transform.rotation = currentLevelGO.GetComponent<Level>().levelCamValues.rotation;
        } 
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

        SetLevelPuntuationScreen();
    }

    public void SetLevelPuntuationScreen()
    {
        if (Application.isMobilePlatform) UIManager.Instance.SetMobileGameplayControlsActive(false);
        UIManager.Instance.SetPuntuationScreenActive(true);
        TimeManager.Instance.currentTime += TimeManager.Instance.levelTime;
        TimeManager.Instance.levelTime = 0;
    }

    public void GoToInbetweenLevels()
    {
        UIManager.Instance.SetPuntuationScreenActive(false);
        UIManager.Instance.SetInBetweenLevelsScreenActive(true);
        SetNextLevel();
    }
    public void GoToLevelOverview()
    {
        UIManager.Instance.SetInBetweenLevelsScreenActive(false);
        UIManager.Instance.SetLevelOverviewActive(true);
    }
    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }

}
