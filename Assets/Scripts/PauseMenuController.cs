using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private bool isOpened = false;
    [SerializeField] private Button resumeBTN;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenClosePauseMenu();
        }
    }

    private bool playerWasWorking;
    public void OpenClosePauseMenu()
    {
        if (isOpened)
        {
            UIManager.Instance.SetPauseMenuActive(false);
            Time.timeScale = 1f;
            GameManager.Instance.playerWork = playerWasWorking;
            SoundManager.Instance.MusicOnOff(playerWasWorking);
            if (UIManager.Instance.currentBTN != null) UIManager.Instance.currentBTN.GetComponent<Button>().Select();
        }
        else
        {
            playerWasWorking = GameManager.Instance.playerWork;
            UIManager.Instance.SetPauseMenuActive(true);
            Time.timeScale = 0f;
            GameManager.Instance.playerWork = false;
            SoundManager.Instance.MusicOnOff(false);
            resumeBTN.Select();
        }
        isOpened = !isOpened;
    }

    public void Autodestroy()
    {
        OpenClosePauseMenu();
        GameManager.Instance.currentPlayer.PlayerOffLimits(GameManager.Instance.currentLevelGO.GetComponent<Level>().playerStartTr);
    }

    public void ReturnToLobby()
    {
        OpenClosePauseMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        OpenClosePauseMenu();
        Application.Quit();
    }
}
