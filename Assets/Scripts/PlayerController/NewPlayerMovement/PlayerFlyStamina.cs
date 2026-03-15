using UnityEngine;
using System.Collections;

public class PlayerFlyStamina : MonoBehaviour
{
    private PlayerMovement pMovement;

    [SerializeField] public float maxFlyFuel;
    [SerializeField] private float fuelBurnSpeed;
    public float currentFuel;
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
