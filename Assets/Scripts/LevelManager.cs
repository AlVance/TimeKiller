using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject[] levelsGO;
    public GameObject currentLevelGO;
    private int currentLevel = 0;

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
    }

    private IEnumerator DestroyGOWithDelay(GameObject GO)
    {
        yield return new WaitForEndOfFrame();
        Destroy(GO);
    }

}
