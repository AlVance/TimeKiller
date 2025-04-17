using UnityEngine;

public class StartSceneByTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManagement.Instance.LoadGameScene();
    }
}
