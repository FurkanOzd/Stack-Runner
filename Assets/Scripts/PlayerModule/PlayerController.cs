using DG.Tweening;
using GameManagementModule;
using Signals;
using UnityEngine;
using Zenject;

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
    private LayerMask _finishLayer;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isReadyToMove)
        {
            return;
        }
        
        if(_finishLayer == (_finishLayer | (1 << collision.gameObject.layer)))
        {
            _signalBus.Fire(new GameStateChangedSignal(GameState.SuccessAnimation));
            Vector3 movePosition = collision.transform.position + collision.transform.localScale / 2;
            movePosition.y = transform.position.y;
            MoveToFinishBlockCenter(movePosition);
        }
    }

    private void MoveToFinishBlockCenter(Vector3 position)
    {
        transform.DOMove(position, _finalBlockMoveDuration).OnComplete(() =>
        {
            _animator.Play(DanceAnimationHash);
        });
    }
}
