using UnityEngine;
using UnityEngine.Events;

public class StartSceneByTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEvents;
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEvents.Invoke();
    }
}
