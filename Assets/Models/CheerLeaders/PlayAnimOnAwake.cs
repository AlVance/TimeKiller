using UnityEngine;

public class PlayAnimOnAwake : MonoBehaviour
{
    [SerializeField] private AnimationClip animClipToPlay;
    [SerializeField] private Animation anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.clip = animClipToPlay;
        anim.Play();
    }
}
