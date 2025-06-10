using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
public class PauseMenuController : MonoBehaviour
{
    private bool isOpened = false;
    [SerializeField] private Button resumeBTN;
    private PlayerInput playerInput;

    [SerializeField] private Image unscaled_img;
    private Material unscaled_mat;
    private void Start()
    {
        playerInput = new PlayerInput();
        playerInput.UI.Enable();
        playerInput.UI.Click.performed += ctx =>
        {
            OpenClosePauseMenu();

        };

        unscaled_mat = unscaled_img.material;
    }

    private void Update()
    {
        unscaled_mat.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
    private bool playerWasWorking;
    public void OpenClosePauseMenu()
    {
        if (isOpened)
        {
            Time.timeScale = 1f;
            UIManager.Instance.SetPauseMenuActive(false); 
            GameManager.Instance.playerWork = playerWasWorking;
            if (!GameManager.Instance.isInLobby)
            {
                SoundManager.Instance.MusicOnOff(playerWasWorking);
            }
            if (UIManager.Instance.currentBTN != null) UIManager.Instance.currentBTN.GetComponent<Button>().Select();
        }
        else
        {
            playerWasWorking = GameManager.Instance.playerWork;
            UIManager.Instance.SetPauseMenuActive(true);
            Time.timeScale = 0f;
            GameManager.Instance.playerWork = false;
            if (!GameManager.Instance.isInLobby)
            {
                SoundManager.Instance.MusicOnOff(false);
            }
            resumeBTN.Select();
        }
        isOpened = !isOpened;
    }

    private bool canAutodestroy = true;
    public void Autodestroy()
    {
        //OpenClosePauseMenu();
        //GameManager.Instance.currentPlayer.PlayerOffLimits(GameManager.Instance.currentLevelGO.GetComponent<Level>().playerStartTr);
        if (canAutodestroy) StartCoroutine(_Autodestroy());
    }
    private IEnumerator _Autodestroy()
    {
        canAutodestroy = false;
        OpenClosePauseMenu();
        yield return new WaitForEndOfFrame();
        GameManager.Instance.currentPlayer.PlayerOffLimits(GameManager.Instance.currentLevelGO.GetComponent<Level>().playerStartTr);
        canAutodestroy = true;
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
