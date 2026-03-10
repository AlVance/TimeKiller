using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInput1 playerInput;

    public float horizontalAxis;
    public float camBasedHorizontalAxis;
    public float verticalAxis;
    public float camBasedVerticalAxis;

    public Vector2 moveDir;
    public Vector3 moveDirRelativeToCam;

    public Vector2 aimDir;
    public Vector3 aimDirRelativeToCam;

    public Vector2 shootDir;
    public Vector3 shootDirRelativeToCam;

    public bool flyPressed = false;
    public bool driftPressed = false;
    public bool aimPressed = false;
    public bool shootPressed = false;
    private void Awake()
    {
        playerInput = new PlayerInput1();
        HandleInput();
    }

    private void HandleInput()
    {
        //Move Input
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

        //Fly Input
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

        //Drift Input
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

        //Aim Input
        playerInput.PlayerControls.Aim.started += ctx =>
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                Vector3 PlayerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                tempAimDir.x -= PlayerScreenPos.x;
                tempAimDir.y -= PlayerScreenPos.y;
                aimDir = tempAimDir.normalized;
                aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);
            }
            aimPressed = true;
        };

        playerInput.PlayerControls.Aim.performed += ctx =>
        {

            if (Mouse.current.leftButton.isPressed)
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                Vector3 PlayerScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                tempAimDir.x -= PlayerScreenPos.x;
                tempAimDir.y -= PlayerScreenPos.y;
                aimDir = tempAimDir.normalized;
            }
            else
            {
                Vector2 tempAimDir = ctx.ReadValue<Vector2>();
                if (tempAimDir.x > 0.1f || tempAimDir.x < -0.1f || tempAimDir.y > 0.1f || tempAimDir.y < -0.1f)
                {
                    aimDir = tempAimDir;
                }

            }
            aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);
        };

        playerInput.PlayerControls.Aim.canceled += ctx =>
        {
            aimPressed = false;
            aimDir = Vector2.zero;
            aimDirRelativeToCam = GetV3RelativeToCamera(aimDir);
        };


        playerInput.PlayerControls.Shoot.started += ctx =>
        {
            shootPressed = true;
        };
        playerInput.PlayerControls.Shoot.performed += ctx =>
        {

        };
        playerInput.PlayerControls.Shoot.canceled += ctx =>
        {
            shootPressed = false;
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
