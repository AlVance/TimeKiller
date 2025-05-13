using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SwitchingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject switchingPlatformVisual;
    [SerializeField] private Animator switchingPlatformAnimator;
    [SerializeField] private bool switchPlatformState = false;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (switchPlatformState)
        {
            switchingPlatformAnimator.SetBool("SwitchingPlatformState", switchPlatformState);
        }
        GameManager.Instance.currentPlayer.OnStartFlyEvent.AddListener(ChangePlatformState);
    }

    private void ChangePlatformState()
    {
        switchPlatformState = !switchPlatformState;
        switchingPlatformAnimator.SetBool("SwitchingPlatformState", switchPlatformState);
    }
}
