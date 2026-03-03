using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class SwitchingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject switchingPlatformVisual;
    [SerializeField] private Animator switchingPlatformAnimator;
    [SerializeField] private bool switchPlatformState = false;
    [SerializeField] private Collider platformCol;

    [SerializeField] private AudioSource switchPlatformAS;
    [SerializeField] private AudioClip switchAC;

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
        switchPlatformAS.PlayOneShot(switchAC);
        switchPlatformState = !switchPlatformState;
        switchingPlatformAnimator.SetBool("SwitchingPlatformState", switchPlatformState);
        platformCol.enabled = !switchPlatformState;
    }
}
