using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game UI Variables")]
    [SerializeField] private GameObject mobileControlsUI;

    [Header("Level UI Variables")]
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text levelTimeText;

    [Header("Player UI Variables")]
    [SerializeField] private TMP_Text currentDamageText;
    [SerializeField] private TMP_Text currentSpeedText;
    [SerializeField] private TMP_Text currentRangeText;
    [SerializeField] private TMP_Text currentProjectileSizeText;
    [SerializeField] private TMP_Text currentProjectileSpeedText;
    [SerializeField] private TMP_Text currentChargeTimeText;
    [SerializeField] private TMP_Text bulletsText;

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
        if (Application.isMobilePlatform) mobileControlsUI.SetActive(true);
        else mobileControlsUI.SetActive(false);
    }

    public void SetCurrentDamageText(string newText)
    {
        currentDamageText.text = newText;
    }
    public void SetCurrentChargeTimeText(string newText)
    {
        currentChargeTimeText.text = newText;
    }
    public void SetCurrentSpeedText(string newText)
    {
        currentSpeedText.text = newText;
    }
    public void SetCurrentRangeText(string newText)
    {
        currentRangeText.text = newText;
    }
    public void SetCurrentProjectileSpeedText(string newText)
    {
        currentProjectileSpeedText.text = newText;
    }
    public void SetCurrentProjectileSizeText(string newText)
    {
        currentProjectileSizeText.text = newText;
    }
    public void SetBulletsText(string newText)
    {
        bulletsText.text = newText;
    }

    public void SetCurrentTimeText(string newText)
    {
        currentTimeText.text = newText;
    }

    public void SetLevelTimeText(string newText)
    {
        levelTimeText.text = newText;
    }
}
