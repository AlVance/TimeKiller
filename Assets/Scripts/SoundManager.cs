using UnityEngine;
using UnityEditor;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField]private AudioSource audioSource;
    [SerializeField]public AudioSource musicSourceON;
    [SerializeField]public AudioSource musicSourceOFF;
    [SerializeField]public AudioClip[] musicClips;

    public bool isLevelMusicMuted = false;
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
        if (isLevelMusicMuted)
        {
            MuteLevelMusic();
        }
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
        if (ToOn)
        {
            musicSourceON.volume = 0.1f;
            musicSourceOFF.volume = 0f;
        }
        else
        {
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
            //Play lobby music
        }
    }

    public void MuteLevelMusic()
    {
        if (isLevelMusicMuted)
        {
            musicSourceON.mute = true;
            musicSourceOFF.mute = true;
            isLevelMusicMuted = false;
        }
        else
        {
            musicSourceON.mute = false;
            musicSourceOFF.mute = false;
            isLevelMusicMuted = true;
        }
    }
}

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SoundManager SM = (SoundManager)target;

        if(GUILayout.Button("Mute/Unmute Level Music"))
        {
            SM.MuteLevelMusic();
        }
    }
}
