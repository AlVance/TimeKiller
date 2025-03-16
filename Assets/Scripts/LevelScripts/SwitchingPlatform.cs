using Unity.VisualScripting;
using UnityEngine;

public class SwitchingPlatform : MonoBehaviour
{

    [SerializeField] private GameObject switchingPlatformVisual;
    [SerializeField] private Animator switchingPlatformAnimator;


    private void Update()
    {
        switchingPlatformAnimator.SetBool("SwitchingPlatformState", GameManager.Instance.currentPlayer.switchPlatformState);
    }



}
