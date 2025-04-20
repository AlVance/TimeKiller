using UnityEngine;

public class GOLoaderByPlayerPrefs : MonoBehaviour
{
    [SerializeField] private string playerPrefsToUnlock = "Level_0";
    [SerializeField] private GameObject GOToActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetStickerActive();
    }

    public void SetStickerActive()
    {
        if(PlayerPrefs.GetInt(playerPrefsToUnlock) == 1)
        {
            GOToActive.SetActive(true);
        }
        else
        {
            GOToActive.SetActive(false);
        }
    }
}
