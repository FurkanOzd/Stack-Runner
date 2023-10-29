using System;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _slideDuration;

    private static readonly int DanceAnimationHash = Animator.StringToHash("Dance");
    private static readonly int RunAnimationHash = Animator.StringToHash("Run");

    private void Update()
    {
        transform.position += transform.forward * (Time.deltaTime * _moveSpeed);
    }

    public void UpdateMoveDirection(float positionX)
    {
        transform.DOMoveX(positionX, _slideDuration);
    }
}
