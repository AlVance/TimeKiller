using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game UI Variables")]
    [SerializeField] private GameObject mobileControlsUI;
    [SerializeField] private GameObject ProggressionBTNsInputPromts;
    [SerializeField] private GameObject InlevelUI;
    [SerializeField] private GameObject InitialScenelUI;
    [SerializeField] private GameObject TimersGO;
    [SerializeField] private GameObject ExplorationModeTagGO;

    [Header("UI Transitions")]
    [SerializeField] private GameObject endLevelScreenUI;
    [SerializeField] private GameObject GoToInBetweenBTN;
    [SerializeField] private GameObject GoToLevelBTN;
    [SerializeField] private GameObject StartLevelBTN;
    [SerializeField] private GameObject GoToStartBTN;
    [SerializeField] private GameObject inBetweenLevelsScreenUI;
    [SerializeField] private GameObject levelOverviewUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] public Animator fadeAnim;
    [SerializeField] private Animator ProgressionBTNsAnim;

    [Header("Level data Variables")]
    [SerializeField] private TMP_Text levelCountText;
    [SerializeField] private TMP_Text levelNameText;

    [Header("End level UI Stuff")]
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private Slider[] timeSavedSliders;
    [SerializeField] private GameObject timeSavedSliderGO;
    [SerializeField] private TMP_Text timeSavedText;
    [SerializeField] private Slider[] mostTimeSavedSliders;
    [SerializeField] private TMP_Text mostTimeSavedText;
    [SerializeField] private GameObject mostTimeSavedSliderGO;
    [SerializeField] private GameObject newRecordText;
    [SerializeField] private GameObject endExplorationModeTextGO;


    [Header("Credits UI Stuff")]
    [SerializeField] private GameObject GoToCreditsBTN;
    [SerializeField] private GameObject creditsUI;
    [SerializeField] private TMP_Text creditsDownText;

    [Header("Level UI Variables")]
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text levelTimeText;
    [SerializeField] public TMP_Text startLevelTimerText;
    [SerializeField] public Animator startLevelTimerAnim;
    [SerializeField] private Slider levelTimerSlider;
    [SerializeField] private Slider[] globalTimerSliders;
    [SerializeField] private TMP_Text objectivesValueText;
    [SerializeField] private Animator TimerUIAnim;
    [SerializeField] private Animator TimerParentUIAnim;

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
    [SerializeField] private Image flyFuelSliderImage;

    [Header("Pause Menu Variables")]
    [SerializeField] private GameObject pauseMenuGO;
    [SerializeField] private GameObject musicBTNGO;
    [SerializeField] private TMP_Text musicBTNText;
    [SerializeField] private TMP_Text soundBTNText;
    [SerializeField] private GameObject exitGameBTNGO;
    public GameObject currentBTN;

    [Header("Leaderboard variables")]
    [SerializeField] private GameObject profileSetupParentGO;
    [SerializeField] public TMP_InputField profileNameField;

    [Header("Debug UI Varables")]
    [SerializeField] private TMP_Text fpsText;

    [Header("Misc Variables")]
    [SerializeField] private GameObject startLvlExplorationScreen;
    [SerializeField] private GameObject startLvlTKScreen;
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
        if (GameManager.Instance.isMobile) 
        {
            //ProggressionBTNsInputPromts.SetActive(false);
            //mobileControlsUI.SetActive(true);
        } 
        else 
        {
            ProggressionBTNsInputPromts.SetActive(true);
            mobileControlsUI.SetActive(false);
        }
#if PLATFORM_WEBGL
        exitGameBTNGO.SetActive(false);
#endif
    }

    private void Update()
    {
        fpsText.text = (1.0f / Time.smoothDeltaTime).ToString("0") + " fps";
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
        startLevelTimerAnim.SetTrigger("On");
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
     public void SetFlyFuelSliderColor(Color newColor)
    {
        flyFuelSliderImage.color = newColor;
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
    public void SetParentTimerCritical(bool isCritical)
    {
        TimerParentUIAnim.SetBool("IsCritical", isCritical);
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
        if (GameManager.Instance.explorationMode)
        {
            TimersGO.SetActive(false);
            //ExplorationModeTagGO.SetActive(isActive);
        }
        else
        {
            TimersGO.SetActive(isActive);
        }
    }
    public void SetInitialSceneUIActive(bool isActive)
    {
        InitialScenelUI.SetActive(isActive);
    }
    public void SetGoToInBetweenBTNActive(bool isActive)
    {
        ActivateProgressionBTN(GoToInBetweenBTN, isActive);
    }
    public void SetGoToLevelBTNActive(bool isActive)
    {
        ActivateProgressionBTN(GoToLevelBTN, isActive);

    }
    public void SetStartLevelBTNActive(bool isActive)
    {
        ActivateProgressionBTN(StartLevelBTN, isActive);
    }
    public void SetGoToStartBTNActive(bool isActive)
    {
        ActivateProgressionBTN(GoToStartBTN, isActive);
    }
    public void SetGoToCreditsBTNActive(bool isActive)
    {
        ActivateProgressionBTN(GoToCreditsBTN, isActive);
    }
    private void ActivateProgressionBTN(GameObject BTN, bool isActive)
    {
        if (isActive)
        {
            BTN.SetActive(isActive);
            ProgressionBTNsAnim.SetBool("OnScreen", isActive);
            BTN.GetComponent<Button>().Select();
            currentBTN = BTN;
        }
        else 
        {
            StartCoroutine(SetBtnsInnactive(BTN));
            currentBTN = null;
        } 
    }
    private IEnumerator SetBtnsInnactive(GameObject a)
    {
        ProgressionBTNsAnim.SetBool("OnScreen", false);
        yield return new WaitForSeconds(0.5f);
        a.SetActive(false);
    }

    public void SetGameOverScreenctive(bool isActive)
    {
        gameOverUI.SetActive(isActive);
    }
    public void SetCreditsScreenActive(bool isActive)
    {
        creditsUI.SetActive(isActive);
        if (GameManager.Instance.explorationMode) creditsDownText.text = "You're ready to become a real Time Killer!";
        else creditsDownText.text = "You're a real Time Killer <3";
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

    public void SetEndGameUIActive(bool isActive)
    {
        endGameUI.SetActive(isActive);
        if(isActive == false)
        {
            SetTimeSavedSliderActive(false);
            SetMostTimeSavedSliderActive(false);
            newRecordText.SetActive(false);
            SetEndGameExplorationTextActive(false);
        }
    }

    public void SetTimeSavedSlidiers(float newValue)
    {
        for (int i = 0; i < timeSavedSliders.Length; i++)
        {
            timeSavedSliders[i].value = newValue;
        }
        timeSavedText.text = newValue.ToString("0.00");
    }
    public void SetMostTimeSavedSlidiers(float newValue)
    {
        for (int i = 0; i < timeSavedSliders.Length; i++)
        {
            mostTimeSavedSliders[i].value = newValue;
        }
        mostTimeSavedText.text = newValue.ToString("0.00");
    }

    public void SetTimeSavedSliderActive(bool isActive)
    {
        timeSavedSliderGO.SetActive(isActive);
    }

    public void SetMostTimeSavedSliderActive(bool isActive)
    {
        mostTimeSavedSliderGO.SetActive(isActive);
    }
    public void SetNewRecordTextActive(bool isActive)
    {
        newRecordText.SetActive(isActive);
    }

    public void SetPauseMenuActive(bool isActive)
    {
        pauseMenuGO.SetActive(isActive);
    }

    public void SetMusicMuteText(string newText)
    {
        musicBTNText.text = newText;
    }

    public void SetSoundMuteText(string newText)
    {
        soundBTNText.text = newText;
    }

    public void SetMusicBTNGOActive(bool isActive)
    {
        musicBTNGO.SetActive(isActive);
    }

    public void SetExplorationTagActive(bool isActive)
    {
        ExplorationModeTagGO.SetActive(isActive);
        if (isActive)
        {
            startLvlExplorationScreen.SetActive(true);
            startLvlTKScreen.SetActive(false);
        }
        else
        {
            startLvlExplorationScreen.SetActive(false);
            startLvlTKScreen.SetActive(true);
        }
    }

    public void SetEndGameExplorationTextActive(bool isActive)
    {
        endExplorationModeTextGO.SetActive(isActive);
    }

    public void SetProfileScreenGOActive(bool isActive)
    {
        profileSetupParentGO.SetActive(isActive);
        if (isActive) profileNameField.Select();
        if (!isActive && currentBTN != null) currentBTN.GetComponent<Button>().Select();
    }

    public void RepleaceNameIFSpaces()
    {
        profileNameField.text = profileNameField.text.Replace(" ", "");
    }
}
