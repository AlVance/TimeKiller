using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInput playerInput;

    public float horizontalAxis;
    public float camBasedHorizontalAxis;
    public float verticalAxis;
    public float camBasedVerticalAxis;

    public Vector2 moveDir;
    public Vector3 moveDirRelativeToCam;

    public bool flyPressed = false;
    public bool driftPressed = false;
    private void Awake()
    {
        playerInput = new PlayerInput();
        HandleInput();
    }

    private void HandleInput()
    {
        playerInput.PlayerControls.Move.started += ctx =>
        {
            
        };
        playerInput.PlayerControls.Move.performed += ctx =>
        {
            horizontalAxis = ctx.ReadValue<Vector2>().x;
            verticalAxis = ctx.ReadValue<Vector2>().y;
            moveDir = ctx.ReadValue<Vector2>().normalized;

            moveDirRelativeToCam = GetV3RelativeToCamera(moveDir).normalized;
        };
        playerInput.PlayerControls.Move.canceled += ctx =>
        {
            moveDir = Vector2.zero;
            moveDirRelativeToCam = GetV3RelativeToCamera(moveDir);
        };

        playerInput.PlayerControls.Reload.started += ctx =>
        {
            flyPressed = true;
        };

        playerInput.PlayerControls.Reload.performed += ctx =>
        {

        };

        playerInput.PlayerControls.Reload.canceled += ctx =>
        {
            flyPressed = false;
        };

        playerInput.PlayerControls.Drift.started += ctx =>
        {
            driftPressed = true;
        };

        playerInput.PlayerControls.Drift.performed += ctx =>
        {

        };

        playerInput.PlayerControls.Drift.canceled += ctx =>
        {
            driftPressed = false;
        };
    }

    private Vector3 GetV3RelativeToCamera(Vector2 baseDir)
    {
        Vector3 camForward;
        Vector3 camRight;
        if (CameraManager.Instance != null)
        {
            camForward = CameraManager.Instance.currentCam.transform.forward;
            camRight = CameraManager.Instance.currentCam.transform.right;
        }
        else
        {
            camForward = Camera.main.transform.forward;
            camRight = Camera.main.transform.right;
        }
        
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 forwardRelativeVertical = baseDir.y * camForward;
        Vector3 rightRelativeVertical = baseDir.x * camRight;

        Vector3 cameraRelativeDir = (forwardRelativeVertical + rightRelativeVertical).normalized;
        return cameraRelativeDir;
    }

    private void OnEnable()
    {
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }
}
