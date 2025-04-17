using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerController currentPlayer;
    public GameObject currentLevelGO;
    public int currentLevel = 0;
    private bool m_levelStarted = false;
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
    }


}
