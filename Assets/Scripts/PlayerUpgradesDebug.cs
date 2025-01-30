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
        PC.SetPlayerDamage(damageUpgrade);
        currentDamageText.text = "Damage: " + PC.projectileDamage;
    }

    private void UpgradeSpeed()
    {
        PC.SetPlayerSpeed(speedUpgrade);
        currentSpeedText.text = "Speed: " + PC.moveSpeed;
    }

    private void UpgradeRange()
    {
        PC.SetPlayerRange(rangeUpgrade);
        currentRangeText.text = "Range: " + PC.projectileRange;
    }

    private void UpgradePSpeed()
    {
        PC.SetPlayerProjectileSpeed(projectileSpeedUpgrade);
        currentProjectileSpeedText.text = "PSpeed " + PC.projectileSpeed;
    }

    private void UpgradePSize()
    {
        PC.SetPlayerProjectileSize(projectileSizeUpgrade);
        currentProjectileSizeText.text = "PSize " + PC.projectileSize;
    }

    private void UpgradeChargeTime()
    {
        PC.SetPlayerChargeTime(chargeTimeUpgrade);
        currentChargeTimeText.text = "ChargeTime " + PC.shootChargeTime;
    }

    private void UpgradeMaxBullets()
    {
        PC.SetPlayerMaxBullets(maxBulletsUpgrade);
        bulletsText.text = PC.currentBullets + "/" + PC.maxBullets;
    }
}
