using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        this.transform.LookAt(Camera.main.transform);
    }
}
