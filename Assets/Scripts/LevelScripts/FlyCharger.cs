using UnityEngine;

public class FlyCharger : MonoBehaviour
{
    [SerializeField] private float fuelRegenerationSpeed;
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(other.gameObject.GetComponent<PlayerController>().currentFuel < other.gameObject.GetComponent<PlayerController>().maxFuel)
            {
                other.gameObject.GetComponent<PlayerController>().currentFuel += fuelRegenerationSpeed * Time.deltaTime;
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().currentFuel = other.gameObject.GetComponent<PlayerController>().maxFuel;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            UIManager.Instance.SetFlyFuelSliderColor(Color.cyan);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            UIManager.Instance.SetFlyFuelSliderColor(Color.white);
        }
    }
}
