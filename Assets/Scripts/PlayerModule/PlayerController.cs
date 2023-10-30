using System;
using DG.Tweening;
using GameManagementModule;
using PathBlocksModule;
using Signals;
using UnityEngine;
using Zenject;
using Task = System.Threading.Tasks.Task;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private float _slideDuration;
    [SerializeField]
    private float _finalBlockMoveDuration;

    [SerializeField]
    private LayerMask _failLayer;

    [Inject]
    private SignalBus _signalBus;

    private static readonly int DanceAnimationHash = Animator.StringToHash("Dance");
    private static readonly int RunAnimationHash = Animator.StringToHash("Run");

    private bool _isReadyToMove;

    private void Start()
    {
        ListenEvents();
    }

    private void ListenEvents()
    {
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedSignal gameStateChangedSignal)
    {
        _isReadyToMove = gameStateChangedSignal.GameState == GameState.Playing;

        if (_isReadyToMove)
        {
            _animator.Play(RunAnimationHash);
        }
    }

    private void Update()
    {
        if (_isReadyToMove)
        {
            transform.position += transform.forward * (Time.deltaTime * _moveSpeed);
        }
    }

    public void UpdateMoveDirection(float positionX)
    {
        transform.DOMoveX(positionX, _slideDuration);
    }

    public void MoveToFinishBlock(Block blockToMove)
    {
        Vector3 blockPosition = blockToMove.transform.position;
        Vector3 movePosition = new Vector3(blockPosition.x, transform.position.y,
            blockPosition.z + blockToMove.transform.localScale.z / 2);
        
        transform.DOMove(movePosition, _finalBlockMoveDuration).OnComplete(() =>
        {
            PlaySuccessAnimationAsync();
        });
    }

    private async void PlaySuccessAnimationAsync()
    {
        const float animationLength = 2f;
        
        _animator.Play(DanceAnimationHash);
        
        await Task.Delay(TimeSpan.FromSeconds(animationLength));
        
        _signalBus.Fire(new GameStateChangedSignal(GameState.Success));
    }
}
