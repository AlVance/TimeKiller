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

    public void LoadGameScene()
    {
        StartCoroutine(_LoadScene("BaseScene"));
    }
    private IEnumerator _LoadScene(string sceneName)
    {
        UIManager.Instance.SetFade(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetFade(false);
    }
}
