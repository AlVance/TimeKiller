using UnityEngine;
using System.Collections;

public class SilenceCube : MonoBehaviour
{
    [SerializeField] private AudioSource natureSource;

    public IEnumerator Fade(bool fadeIn, AudioSource natureSource, float duration, float targetVolume)
    {
        float time = 0f;
        float startVol = natureSource.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            natureSource.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }
        yield break;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SoundManager.Instance.FadeInOut(false, 0f);
            StartCoroutine(Fade(true,natureSource,1.5f,0.02f));
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //SoundManager.Instance.LobbyMusicOnOff(false);
            //SoundManager.Instance.FadeInOut(false, 0f);
        }
    }
       
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Fade(false, natureSource, 2f, 0f));
            SoundManager.Instance.LobbyMusicOnOff(true);
            SoundManager.Instance.FadeInOut(true, 0.02f);
        }
    }
}
