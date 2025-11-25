using UnityEngine;
using FMODUnity;


public class AudioManager : MonoBehaviour
{
    public StudioEventEmitter audLobbyMusic;
    void Start()
    {
        audLobbyMusic.Play();
    }

    void Update()
    {
        
    }
}
