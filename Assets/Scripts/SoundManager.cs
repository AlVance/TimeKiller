using UnityEngine;
using UnityEditor;
using System.Collections;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField]private AudioSource audioSource;
    [SerializeField]public AudioSource musicSourceON;
    [SerializeField]public AudioSource musicSourceOFF;
    [SerializeField]public AudioSource musicLobby;
    [SerializeField]public AudioClip[] musicClips;
    [SerializeField] private float levelMusicMergeTime;

    private bool isLevelMusicMuted = false;
    private bool isMusicMuted = true;
    private AudioListener mainCamAL;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayMusic(musicSourceON);
        PlayMusic(musicSourceOFF);
        LobbyMusicOnOff(true);

        mainCamAL = Camera.main.GetComponent<AudioListener>();
    }

    public void PlayOneShootAudio(AudioClip AC, float volume = 1f)
    {
        audioSource.PlayOneShot(AC, volume);
    }
    public void PlayMusic(AudioSource musicSource)
    {
        musicSource.Play();
    }
    public void MusicOnOff(bool ToOn)
    {
        StartCoroutine(_MusicLevelOnOff(ToOn));
    }
    private IEnumerator _MusicLevelOnOff(bool _ToOn)
    {
        musicLobby.Stop();
        if (_ToOn)
        {
            float goTime = 0f;
            while (musicSourceON.volume < 0.06f)
            {
                musicSourceON.volume = Mathf.Lerp(0, 0.06f, levelMusicMergeTime / goTime);
                musicSourceOFF.volume = Mathf.Lerp(0.05f, 0f, levelMusicMergeTime / goTime);
                goTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            musicSourceON.volume = 0.06f;
            musicSourceOFF.volume = 0f;
        }
        else
        {
            float goTime = 0f;
            while (musicSourceOFF.volume < 0.05f)
            {
                musicSourceON.volume = Mathf.Lerp(0.06f, 0f, levelMusicMergeTime / goTime);
                musicSourceOFF.volume = Mathf.Lerp(0f, 0.05f, levelMusicMergeTime / goTime);
                goTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            musicSourceON.volume = 0f;
            musicSourceOFF.volume = 0.05f;
        }
    }
    public void LobbyMusicOnOff(bool On)
    {
        if (On)
        {
            musicSourceON.volume = 0f;
            musicSourceOFF.volume = 0f;
            musicLobby.Play();
        }
        else
        {
            musicLobby.Stop();
        }
    }

    public void MuteLevelMusic()
    {
        if (isMusicMuted)
        {
            musicSourceON.mute = true;
            musicSourceOFF.mute = true;
            musicLobby.mute = true;
            isMusicMuted = false;
            UIManager.Instance.SetMusicMuteText("Music On");
        }
        else 
        {
            musicSourceON.mute = false;
            musicSourceOFF.mute = false;
            musicLobby.mute = false;
            isMusicMuted = true;

            UIManager.Instance.SetMusicMuteText("Music Off");
        }
    }

    public void MuteMusic()
    {
        MuteLevelMusic();
    }

    public void MuteSound()
    {
        if (mainCamAL.enabled)
        {
            mainCamAL.enabled = false;
            UIManager.Instance.SetSoundMuteText("Sound On");
            UIManager.Instance.SetMusicBTNGOActive(false);
        }
        else
        {
            mainCamAL.enabled = true;
            UIManager.Instance.SetSoundMuteText("Sound Off");
            UIManager.Instance.SetMusicBTNGOActive(true);
        }
    }
}

//[CustomEditor(typeof(SoundManager))]
//public class SoundManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        SoundManager SM = (SoundManager)target;

//        if(GUILayout.Button("Mute/Unmute Level Music"))
//        {
//            SM.MuteLevelMusic();
//        }
//    }
//}
