using UnityEngine;
using System.Collections;

public class PlayerFlyStamina : MonoBehaviour
{
    private PlayerMovement pMovement;
    [SerializeField] private float m_maxFlyFuel;
    public float maxFlyFuel
    {
        get { return m_maxFlyFuel; }
        set
        {
            m_maxFlyFuel = value;
            if(UIManager.Instance != null) UIManager.Instance.SetFlyFuelSliderMaxValue(m_maxFlyFuel);
        }
    }
    [SerializeField] private float fuelBurnSpeed;
    [SerializeField] public float m_currentFuel;
    public float currentFuel
    {
        get { return m_currentFuel; }
        set
        {
            m_currentFuel = value;
            if (UIManager.Instance != null) UIManager.Instance.SetFlyFuelSlderValue(m_currentFuel);
        }
    }
    [SerializeField] private float fuelRecoverSpeed;
    [SerializeField] private float recoverCDTime;
    private bool canRecover = true;
    [SerializeField] public float minFuelToFly;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFuel = maxFlyFuel;
        pMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pMovement.state != PlayerMovement.MovementStates.Flying)
        {
            if (canRecover)
            {
                if (currentFuel < maxFlyFuel)
                {
                    currentFuel += fuelRecoverSpeed * Time.deltaTime;
                }
                else if (currentFuel > maxFlyFuel)
                    currentFuel = maxFlyFuel;
            }
        }
        else
        {
            if(currentFuel > 0)
            {
                currentFuel -= fuelBurnSpeed * Time.deltaTime;
            }
            else if(currentFuel < 0)
            {
                currentFuel = 0;
                StartCoroutine(_RecoverDC());
            }
        }
    }

    private IEnumerator _RecoverDC()
    {
        canRecover = false;
        yield return new WaitForSeconds(recoverCDTime);
        canRecover = true;
    }
}
