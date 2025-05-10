using UnityEngine;
using UnityEngine.Events;

public class EntityColliderController : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerStayEvent;
    public UnityEvent OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayEvent.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent.Invoke();
    }
}
