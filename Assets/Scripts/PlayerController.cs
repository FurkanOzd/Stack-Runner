using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private static readonly int DanceAnimationHash = Animator.StringToHash("Dance");
    private static readonly int RunAnimationHash = Animator.StringToHash("Run");

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 5f;
    }
}
