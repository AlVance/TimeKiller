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
        SetNextLevel();
    }
    public void SetNextLevel()
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

        StartCoroutine(OnLevelStarted());
    }

    private IEnumerator OnLevelStarted()
    {
        yield return new WaitForEndOfFrame();

        GameManager.Instance.currentPlayer.enabled = false;
        TimeManager.Instance.levelTime = currentLevelGO.GetComponent<Level>().levelTime;
        UIManager.Instance.SetLevelTimerSliderMaxValue(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetLevelTimeText(currentLevelGO.GetComponent<Level>().levelTime);
        UIManager.Instance.SetCurrentTimeText(TimeManager.Instance.currentTime);
        GameManager.Instance.currentPlayer.gameObject.transform.position = currentLevelGO.GetComponent<Level>().playerStartTr.position;
        GameManager.Instance.currentPlayer.ResetPlayer();

        UIManager.Instance.startLevelTimerText.gameObject.SetActive(true);

        for (int i = 0; i < startLevelTime; i++)
        {
            UIManager.Instance.SetStartLevelTimerText((startLevelTime - i).ToString("0"));
            yield return new WaitForSeconds(1f);
        }

        UIManager.Instance.startLevelTimerText.gameObject.SetActive(false);
        GameManager.Instance.currentPlayer.enabled = true;
        TimeManager.Instance.timerStarted = true;
    }

    public void OnLevelEnded()
    {
        GameManager.Instance.currentPlayer.enabled = false;
        TimeManager.Instance.timerStarted = false;
        TimeManager.Instance.currentTime += TimeManager.Instance.levelTime;
        UIManager.Instance.SetCurrentTimeText(TimeManager.Instance.currentTime);
        SetNextLevel();
    }

    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }

}
