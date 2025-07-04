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

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.UI.Click.performed += ctx =>
        {
            OpenClosePauseMenu();

        };
    }
    private void Start()
    {
        unscaled_mat = unscaled_img.material;
    }

    private void Update()
    {
        if(isOpened) unscaled_mat.SetFloat("_UnscaledTime", Time.unscaledTime);
    }

    private Vector3 playerLV = Vector3.zero;
    private bool playerWasWorking = true;
    public void OpenClosePauseMenu()
    {
        StartCoroutine(_OpenCloseMenu());        
    }

    private bool canOpenMenu = true;
    private IEnumerator _OpenCloseMenu()
    {
        if (canOpenMenu)
        {
            canOpenMenu = false;
            if (isOpened)
            {
                Time.timeScale = 1f;
                yield return new WaitForEndOfFrame();
                GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                
                UIManager.Instance.SetPauseMenuActive(false);
                GameManager.Instance.playerWork = playerWasWorking;
                GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().linearVelocity = playerLV;
                if (!GameManager.Instance.isInLobby)
                {
                    SoundManager.Instance.MusicOnOff(true);
                }
                if (UIManager.Instance.currentBTN != null) UIManager.Instance.currentBTN.GetComponent<Button>().Select();
            }
            else
            {
                playerWasWorking = GameManager.Instance.playerWork;
                UIManager.Instance.SetPauseMenuActive(true);
                playerLV = GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().linearVelocity;
                GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                GameManager.Instance.currentPlayer.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GameManager.Instance.playerWork = false;
                if (!GameManager.Instance.isInLobby)
                {
                    SoundManager.Instance.MusicOnOff(false);
                }
                if (resumeBTN != null) resumeBTN.Select();
                yield return new WaitForEndOfFrame();
                Time.timeScale = 0f;                
            }
            isOpened = !isOpened;
            yield return new WaitForEndOfFrame();
            canOpenMenu = true;
        }        
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

    private bool canReturnToLobby = true;
    public void ReturnToLobby()
    {
        if (canReturnToLobby) StartCoroutine(_ReturnToLobby());
    }
    private IEnumerator _ReturnToLobby()
    {
        canReturnToLobby = false;
        OpenClosePauseMenu();
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForEndOfFrame();
        canReturnToLobby = true;
    }


    private bool canExitGame = true;
    public void ExitGame()
    {
        if (canExitGame) StartCoroutine(_ExitGame());
    }
    private IEnumerator _ExitGame()
    {
        canExitGame = false;
        OpenClosePauseMenu();
        yield return new WaitForEndOfFrame();
        Application.Quit();
        canExitGame = true;
    }

    private void OnEnable()
    {
        playerInput.UI.Enable();
    }

    private void OnDisable()
    {
        playerInput.UI.Disable();
    }
}
