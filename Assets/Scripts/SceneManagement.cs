using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance { get; private set; }

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
        SceneSetUp();
    }
    public void LoadStartScene()
    {
        StartCoroutine(_LoadScene("InitialScene"));
    }
    public void LoadGameScene()
    {
        StartCoroutine(_LoadScene("BaseScene"));
        StartCoroutine(_LoadBASEScene());
    }
    private IEnumerator _LoadBASEScene()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.SetInitialSceneUIActive(false);
    }
    private IEnumerator _LoadScene(string sceneName)
    {
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForEndOfFrame();
        SceneSetUp();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
    }

    private void SceneSetUp()
    {
        if (SceneManager.GetActiveScene().name == "InitialScene")
        {
            UIManager.Instance.SetInlevelUIActive(false);
            UIManager.Instance.SetInitialSceneUIActive(true);
            GameManager.Instance.playerWork = true;
        }
    }
}
