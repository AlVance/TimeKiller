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
            NavigateWorlds(-1);

        };
        playerInput.UI.Right.performed += ctx =>
        {
            NavigateWorlds(1);

        };
    }
    private void Start()
    {
        UIManager.Instance.SetCurrentWorldText(lM.currentWorld.ToString() + "/" + (lM.worlds.Length -1).ToString());
    }

    private void NavigateWorlds(int navigationIndex)
    {
        if (lM.currentWorld + navigationIndex < lM.worlds.Length && lM.currentWorld + navigationIndex >= 0) 
        {
            lM.currentWorld = lM.currentWorld + navigationIndex;
            UIManager.Instance.SetCurrentWorldText(lM.currentWorld.ToString() + "/" + (lM.worlds.Length - 1).ToString());
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
