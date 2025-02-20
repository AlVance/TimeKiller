using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject[] levelsGO;
    public GameObject currentLevelGO;
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
        StartCoroutine(OnLevelStarted());
    }

    private IEnumerator OnLevelStarted()
    {
        TimeManager.Instance.levelTime = currentLevelGO.GetComponent<Level>().levelTime;
        if (GameManager.Instance != null) GameManager.Instance.currentPlayer.transform.position = currentLevelGO.GetComponent<Level>().playerStartTr.position;

        yield return new WaitForSeconds(startLevelTime);
        GameManager.Instance.currentPlayer.enabled = true;
        TimeManager.Instance.timerStarted = true;
    }

    public void OnLevelEnded()
    {
        GameManager.Instance.currentPlayer.enabled = false;
        TimeManager.Instance.timerStarted = false;
        TimeManager.Instance.currentTime += TimeManager.Instance.levelTime;
        SetNextLevel();
    }

    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }

}
