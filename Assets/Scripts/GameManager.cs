using UnityEngine;
using UnityEditor;

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
    public bool explorationMode
    {
        get {return m_explorationMode;}
        set
        {
            m_explorationMode = value;
            UIManager.Instance.SetExplorationTagActive(value);
        }
    }

    public bool levelStarted 
    { 
        get { return m_levelStarted; }
        set {
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


}

[CustomEditor(typeof(GameManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager GM = (GameManager)target;

        if(GUILayout.Button("Set/Quit Exploration Mode"))
        {
            GM.explorationMode = !GM.explorationMode;
        }
    }
}
