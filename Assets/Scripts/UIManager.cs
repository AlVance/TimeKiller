using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game UI Variables")]
    [SerializeField] private GameObject mobileControlsUI;
    [SerializeField] private GameObject InlevelUI;

    [Header("UI Transitions")]
    [SerializeField] private GameObject endLevelScreenUI;
    [SerializeField] private GameObject GoToInBetweenBTN;
    [SerializeField] private GameObject inBetweenLevelsScreenUI;
    [SerializeField] private GameObject levelOverviewUI;
    [SerializeField] public Animator fadeAnim;

    [Header("Level data Variables")]
    [SerializeField] private TMP_Text levelCountText;
    [SerializeField] private TMP_Text levelNameText;

    [Header("Level UI Variables")]
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text levelTimeText;
    [SerializeField] public TMP_Text startLevelTimerText;
    [SerializeField] private Slider levelTimerSlider;
    [SerializeField] private Slider[] globalTimerSliders;
    [SerializeField] private TMP_Text objectivesValueText;
    [SerializeField] private Animator TimerUIAnim;

    [Header("Player UI Variables")]
    [SerializeField] private TMP_Text currentDamageText;
    [SerializeField] private TMP_Text currentSpeedText;
    [SerializeField] private TMP_Text currentRangeText;
    [SerializeField] private TMP_Text currentProjectileSizeText;
    [SerializeField] private TMP_Text currentProjectileSpeedText;
    [SerializeField] private TMP_Text currentChargeTimeText;
    [SerializeField] private TMP_Text bulletsText;
    [SerializeField] private GameObject bulletsUIContainerGO;
    [SerializeField] private GameObject bulletImage;

    [Header("Reload QTE UI Variables")]
    [SerializeField] private float maxPoint;
    [SerializeField] private GameObject ReloadQTEGO;
    [SerializeField] private GameObject valueBarGO;
    [SerializeField] private GameObject successBarGO;

    [Header("Fly UI Variables")]
    [SerializeField] private Slider flyFuelSlider;
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

    public void SetCurrentTimeText(float newTime)
    {
        currentTimeText.text = newTime.ToString("0.0");
        for (int i = 0; i < globalTimerSliders.Length; i++)
        {
            globalTimerSliders[i].value = newTime;
        }
    }

    public void SetLevelTimeText(float newLevelTime)
    {
        levelTimeText.text = newLevelTime.ToString("0.0");
        levelTimerSlider.value = newLevelTime;
    }
    public void SetLevelTimerSliderMaxValue(float newValue)
    {
        levelTimerSlider.maxValue = newValue;
    }

    public void SetStartLevelTimerText(string newtext)
    {
        startLevelTimerText.text = newtext;
    }

    public void SetNewMaxBulletsImg(int maxBullets)
    {
        int y = bulletsUIContainerGO.transform.childCount;
        for (int i = 0; i < maxBullets - y; i++)
        {
            Instantiate(bulletImage, bulletsUIContainerGO.transform);
        }
    }

    public void SetUsedBulletsImg(int lastBullets)
    {
        bulletsUIContainerGO.transform.GetChild(lastBullets - 1).gameObject.SetActive(false);
        //for (int i = 0; i < lastBullets - newBullets; i++)
        //{
        //    if (bulletsUIContainerGO.transform.GetChild(bulletsUIContainerGO.transform.childCount - 1 - i) != null)
        //        bulletsUIContainerGO.transform.GetChild(bulletsUIContainerGO.transform.childCount - 1 - i).gameObject.SetActive(false);
        //}
    }
    public void SetReloadedBulletsImg(int currentBullets, int maxBullets)
    {
        if (currentBullets > bulletsUIContainerGO.transform.childCount)
        {
            int a = currentBullets - bulletsUIContainerGO.transform.childCount;
            for (int i = 0; i < a; i++)
            {
                Instantiate(bulletImage, bulletsUIContainerGO.transform);
            }
        }
        for (int i = 0; i < currentBullets; i++)
        {
            bulletsUIContainerGO.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            bulletsUIContainerGO.transform.GetChild(i).gameObject.SetActive(true);
            if(i >= maxBullets)
            {
                bulletsUIContainerGO.transform.GetChild(i).gameObject.GetComponent<Image>().color = Color.blue;
            }
        }
        //Deactivate ammo while using it
        //if (maxBullets - currentBullets > 0)
        //{
        //    for (int i = 0; i < maxBullets - currentBullets; i++)
        //    {
        //        if (bulletsUIContainerGO.transform.GetChild(bulletsUIContainerGO.transform.childCount - 1 - i) != null) 
        //            bulletsUIContainerGO.transform.GetChild(bulletsUIContainerGO.transform.childCount - 1 - i).gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < currentBullets; i++)
        //    {
        //        bulletsUIContainerGO.transform.GetChild(i).gameObject.SetActive(true);
        //    }
        //}
        
    }

    public void SetReloadValueBar(float newValue)
    {
        valueBarGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(newValue * maxPoint, valueBarGO.GetComponent<RectTransform>().anchoredPosition.y, 0);
    }

    public void SetReloadQTEActive(bool isActive)
    {
        ReloadQTEGO.SetActive(isActive);
    }

    public void SetReloadSuccessBar(float newSuccessValue)
    {
        successBarGO.GetComponent<RectTransform>().sizeDelta = new Vector2((newSuccessValue - 0.1f) * maxPoint, successBarGO.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void SetFlyFuelSlderValue(float newValue)
    {
        flyFuelSlider.value = newValue;
    }
    public void SetFlyFuelSliderMaxValue(float newMaxValue)
    {
        flyFuelSlider.maxValue = newMaxValue;
    }

    public void SetObjectivesValueText(int currentObj, int maxObj)
    {
        objectivesValueText.text = currentObj.ToString() + "/" + maxObj.ToString();
    }

    public void SetPuntuationScreenActive(bool isActive)
    {
        endLevelScreenUI.SetActive(isActive);
    }
    public void SetTimerUIToIdle()
    {
        TimerUIAnim.SetTrigger("GoToIdle");
    }
    public void SetTimerUIToWinScreen()
    {
        TimerUIAnim.SetTrigger("GoToWinScreen");
    }
    public void SetInBetweenLevelsScreenActive(bool isActive)
    {
        inBetweenLevelsScreenUI.SetActive(isActive);
    }
    public void SetLevelOverviewActive(bool isActive)
    {
        levelOverviewUI.SetActive(isActive);
    }
    public void SetInlevelUIActive(bool isActive)
    {
        InlevelUI.SetActive(isActive);
    }
    public void SetGoToInBetweenBTNActive(bool isActive)
    {
        GoToInBetweenBTN.SetActive(isActive);
    }
    public void SetLevelNameText(string newText)
    {
        levelNameText.text = newText;
    }
    public void SetLevelCountText(int completedLevels, int maxLevels)
    {
        levelCountText.text = completedLevels.ToString() + "/" + maxLevels.ToString();
    }

    public void SetMobileGameplayControlsActive(bool isActive)
    {
        mobileControlsUI.SetActive(isActive);
    }

    public void SetFade(bool fadeState)
    {
        if(fadeState)fadeAnim.SetTrigger("FadeIn");
        else fadeAnim.SetTrigger("FadeOut");
    }
}
