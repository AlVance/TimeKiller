using UnityEngine;
using System.Collections;
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] public float startTime;

    private float m_currentTime;
    public float currentTime
    {
        get { return m_currentTime; }
        set 
        { 
            m_currentTime = value;
            UIManager.Instance.SetCurrentTimeText(m_currentTime);
        }
    }

    private float m_levelTime;
    public float levelTime
    {
        get { return m_levelTime; }
        set 
        {
            m_levelTime = value;
            UIManager.Instance.SetLevelTimeText(m_levelTime);
        }
    }

    public bool timerStarted = false;
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
        currentTime = startTime;
    }

    private void Update()
    {
        if (GameManager.Instance.levelStarted)
        {
            if (levelTime > 0) levelTime -= Time.deltaTime;
            else
            {
                levelTime = 0;
                currentTime -= Time.deltaTime;
            }

            if(currentTime <= 0)
            {
                currentTime = 0;
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        StartCoroutine(_GameOver());
    }
    private IEnumerator _GameOver()
    {
        GameManager.Instance.levelStarted = false;
        GameManager.Instance.currentPlayer.BlockPlayer(0.2f);
        GameManager.Instance.currentPlayer.ResetPlayer();
        CameraManager.Instance.ChangeCam(CameraManager.Instance.winCam);
        GameManager.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0, -180, 0);
        GameManager.Instance.currentPlayer.anim.SetBool("IsLose", true);
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.SetTimerUIToWinScreen();
        UIManager.Instance.SetGameOverScreenctive(true);
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.SetGoToStartBTNActive(true);
    }
}
