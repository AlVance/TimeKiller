using UnityEngine;

public class WorldSelector : MonoBehaviour
{
    private LevelManager lM;
    private PlayerInput playerInput;
    private void Awake()
    {
        lM = this.GetComponent<LevelManager>();
        playerInput = new PlayerInput();
        playerInput.UI.Backward.performed += ctx =>
        {
            if (lM.isInWorldSelect) GameManager.Instance.ChangeGameMode();

        };
        playerInput.UI.Left.performed += ctx =>
        {
            if (lM.isInWorldSelect) NavigateWorlds(-1);

        };
        playerInput.UI.Right.performed += ctx =>
        {
            if (lM.isInWorldSelect) NavigateWorlds(1);

        };
    }
    private void Start()
    {
        UIManager.Instance.SetCurrentWorldText((lM.currentWorld + 1).ToString() + "/" + (lM.worlds.Length).ToString());
    }

    private void NavigateWorlds(int navigationIndex)
    {
        if (lM.currentWorld + navigationIndex < lM.worlds.Length && lM.currentWorld + navigationIndex >= 0) 
        {
            lM.currentWorld = lM.currentWorld + navigationIndex;
            UIManager.Instance.SetCurrentWorldText((lM.currentWorld + 1).ToString() + "/" + (lM.worlds.Length).ToString());
        }

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
