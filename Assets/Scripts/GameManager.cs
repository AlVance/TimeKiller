using UnityEngine;
//using UnityEditor;
using System.Collections;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerController currentPlayer;
    public GameObject currentLevelGO;
    public int currentLevel = 0;
    private bool m_levelStarted = false;
    public bool isMobile = false;
    [SerializeField] private MobileDetector MD;
    private bool m_explorationMode = false;
    public bool isInLobby = false;
    [SerializeField] private bool startAsExplorationMode = true;
    public bool explorationMode
    {
        get { return m_explorationMode; }
        set
        {
            m_explorationMode = value;
            UIManager.Instance.SetExplorationTagActive(value);
            if (value) UIManager.Instance.SetGameModeText("Exploration Mode");
            else UIManager.Instance.SetGameModeText("Time Killer Mode");
        }
    }

    public bool levelStarted
    {
        get { return m_levelStarted; }
        set
        {
            m_levelStarted = value;
            playerWork = value;
        }
    }
    public bool playerWork = false;

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

        levelStarted = false;

        isMobile = MD.IsRunningOnMobile();
    }

    private void Start()
    {

#if PLATFORM_STANDALONE_WIN
        Application.targetFrameRate = 90;
#elif PLATFORM_WEBGL
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
#endif

        explorationMode = startAsExplorationMode;
    }
    public void UnloadMemory()
    {
        StartCoroutine(_UnloadRoutine());
    }

    IEnumerator _UnloadRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return null;
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return new WaitForEndOfFrame();
    }

    public void ChangeGameMode()
    {
        explorationMode = !explorationMode;

        if (explorationMode) PlayerPrefs.SetInt("GameMode", 0);
        else PlayerPrefs.SetInt("GameMode", 1);
    }
}


//[CustomEditor(typeof(GameManager))]
//public class SoundManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        GameManager GM = (GameManager)target;

//        if(GUILayout.Button("Set/Quit Exploration Mode"))
//        {
//            GM.explorationMode = !GM.explorationMode;
//        }
//    }
//}
