using UnityEngine;

public class LoadOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToLoad;
    [SerializeField] private bool load;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (load)
            {
                for (int i = 0; i < objectsToLoad.Length; i++)
                {
                    if (!objectsToLoad[i].activeInHierarchy) objectsToLoad[i].SetActive(true);
                }
                Debug.Log("Load");
            }
            else
            {
                for (int i = 0; i < objectsToLoad.Length; i++)
                {
                    if (objectsToLoad[i].activeInHierarchy) objectsToLoad[i].SetActive(false);
                }
                Debug.Log("Unload");
            }
            
        }
       
    }
}
