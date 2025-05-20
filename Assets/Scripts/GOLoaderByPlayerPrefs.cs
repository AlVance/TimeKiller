using UnityEngine;

public class GOLoaderByPlayerPrefs : MonoBehaviour
{
    [SerializeField] private string playerPrefsToLoad = "Level_0";
    [SerializeField] private GameObject GOToActive;
    [SerializeField] private bool load = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetStickerActive();
    }

    public void SetStickerActive()
    {
        if (load)
        {
            if (PlayerPrefs.GetInt(playerPrefsToLoad) == 1)
            {
                GOToActive.SetActive(true);
            }
            else
            {
                GOToActive.SetActive(false);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt(playerPrefsToLoad) == 1)
            {
                GOToActive.SetActive(false);
            }
            else
            {
                GOToActive.SetActive(true);
            }
        }
        
    }
}
