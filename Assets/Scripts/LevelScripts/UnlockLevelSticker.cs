using UnityEngine;
using System.Collections;
public class UnlockLevelSticker : MonoBehaviour
{
    [SerializeField] private string playerPrefsToUnlock;
    [SerializeField] private Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(UnlockSticker());
        }
    }

    private IEnumerator UnlockSticker()
    {
        PlayerPrefs.SetInt(playerPrefsToUnlock, 1);
        this.GetComponent<SphereCollider>().enabled = false;
        anim.SetBool("Unlock", true);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
