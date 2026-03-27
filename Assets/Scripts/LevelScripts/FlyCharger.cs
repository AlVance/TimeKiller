using UnityEngine;

public class FlyCharger : MonoBehaviour
{
    [SerializeField] private float fuelRegenerationSpeed;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(other.gameObject.GetComponent<PlayerFlyStamina>().currentFuel < other.gameObject.GetComponent<PlayerFlyStamina>().maxFlyFuel)
            {
                other.gameObject.GetComponent<PlayerFlyStamina>().currentFuel += fuelRegenerationSpeed * Time.deltaTime;
            }
            else
            {
                other.gameObject.GetComponent<PlayerFlyStamina>().currentFuel = other.gameObject.GetComponent<PlayerFlyStamina>().maxFlyFuel;
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        UIManager.Instance.SetFlyFuelSliderColor(Color.cyan);
    //    }

    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        UIManager.Instance.SetFlyFuelSliderColor(Color.white);
    //    }
    //}
}
