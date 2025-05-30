using UnityEngine;
using UnityEngine.UI;

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

    public void ExitGame()
    {
        Application.Quit();
    }
}
