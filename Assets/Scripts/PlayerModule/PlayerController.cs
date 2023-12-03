using System;
using Cinemachine;
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

    [SerializeField]
    private Rigidbody _rigidbody;
    
    [SerializeField]
    public CinemachineVirtualCamera _rotatorCamera;

    [SerializeField]
    private Transform _camLookAtTransform;

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
            
            CheckForFail();
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
    
    private void CheckForFail()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5f))
        {
            if((hit.transform.gameObject.layer & (1 << _failLayer)) != 0)
            {
                _rigidbody.constraints = RigidbodyConstraints.None;
                _signalBus.Fire(new GameStateChangedSignal(GameState.Fail));

                _animator.enabled = false;
            }
        };
    }

    private async void PlaySuccessAnimationAsync()
    {
        const float animationLength = 2f;
        const float cameraMovementLength = 0.5f;

        _animator.Play(DanceAnimationHash);
        _rotatorCamera.gameObject.SetActive(true);
        
        await Task.Delay(TimeSpan.FromSeconds(cameraMovementLength));
        
        RotateCamAroundPlayer();
        
        await Task.Delay(TimeSpan.FromSeconds(animationLength));
        
        _rotatorCamera.gameObject.SetActive(false);
        
        _signalBus.Fire(new GameStateChangedSignal(GameState.Success));
    }

    private void RotateCamAroundPlayer()
    {
        const float rotateDuration = 3f;

        _camLookAtTransform.DORotate(new Vector3(0f, 360f, 0f), rotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }
}