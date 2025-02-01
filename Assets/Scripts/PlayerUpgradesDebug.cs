using UnityEngine;
using TMPro;

public class PlayerUpgradesDebug : MonoBehaviour
{
    [SerializeField] private PlayerController PC;

    [SerializeField] private int damageUpgrade;
    [SerializeField] private float rangeUpgrade;
    [SerializeField] private float projectileSizeUpgrade;
    [SerializeField] private float projectileSpeedUpgrade;
    [SerializeField] private float speedUpgrade;
    [SerializeField] private float chargeTimeUpgrade;
    [SerializeField] private int maxBulletsUpgrade;


    [SerializeField] private TMP_Text currentDamageText;
    [SerializeField] private TMP_Text currentSpeedText;
    [SerializeField] private TMP_Text currentRangeText;
    [SerializeField] private TMP_Text currentProjectileSizeText;
    [SerializeField] private TMP_Text currentProjectileSpeedText;
    [SerializeField] private TMP_Text currentChargeTimeText;
    [SerializeField] private TMP_Text bulletsText;

    private void Start()
    {
        currentDamageText.text = "Damage: " + PC.projectileDamage;
        currentSpeedText.text = "Speed: " + PC.moveSpeed;
        currentRangeText.text = "Range: " + PC.projectileRange;
        currentProjectileSpeedText.text = "PSpeed " + PC.projectileSpeed;
        currentProjectileSizeText.text = "PSize " + PC.projectileSize;
        currentChargeTimeText.text = "ChargeTime " + PC.shootChargeTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpgradeDamage();
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpgradeSpeed();
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpgradeRange();
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpgradePSpeed();
        if (Input.GetKeyDown(KeyCode.Alpha5)) UpgradePSize();
        if (Input.GetKeyDown(KeyCode.Alpha6)) UpgradeChargeTime();
        if (Input.GetKeyDown(KeyCode.Alpha7)) UpgradeMaxBullets();
    }

    private void UpgradeDamage()
    {
        PC.projectileDamage +=damageUpgrade;
    }

    private void UpgradeSpeed()
    {
        PC.moveSpeed += speedUpgrade;
    }

    private void UpgradeRange()
    {
        PC.projectileRange += rangeUpgrade;
    }

    private void UpgradePSpeed()
    {
        PC.projectileSpeed+=projectileSpeedUpgrade;
    }

    private void UpgradePSize()
    {
        PC.projectileSize+=projectileSizeUpgrade;
    }

    private void UpgradeChargeTime()
    {
        PC.shootChargeTime -= chargeTimeUpgrade;
    }

    private void UpgradeMaxBullets()
    {
        PC.maxBullets += maxBulletsUpgrade;
    }
}
