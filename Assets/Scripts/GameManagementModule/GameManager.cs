using GameManagementModule;
using PathBlocksModule;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject]
    private BlockController _blockController;
    
    [SerializeField]
    private Transform _blockParent;

    [SerializeField]
    private PlayerController _playerController;

    [SerializeField]
    private Block _baseBlock;

    [SerializeField]
    private float _blockMoveDuration;
    [SerializeField]
    private float _xSpawnOffset;

    [SerializeField]
    private Material[] _blockMaterials;
    [SerializeField]
    private Material _finishBlockMaterial;

    [SerializeField]
    private int _defaultBlockToFinishLevel;
    [SerializeField]
    private int _blockCountIncreaserOnNewLevels;
    private int _levelCount;

    private GameState _gameState;
    
    private void Start()
    {
        _blockController.Initialize(_blockParent, _xSpawnOffset, _blockMoveDuration, _finishBlockMaterial,
            _blockMaterials, _baseBlock);
        _gameState = GameState.ReadyToPlay;
    }

    private void ListenEvents()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_gameState == GameState.Playing)
            {
                Block blockToMove = _blockController.StackBlock();

                if (blockToMove != null)
                {
                    _playerController.UpdateMoveDirection(blockToMove.transform.position.x);
                }
            }

            if (_gameState == GameState.ReadyToPlay)
            {
                _gameState = GameState.Playing;
                _blockController.SetupLevel(_defaultBlockToFinishLevel + _levelCount * _blockCountIncreaserOnNewLevels);
            }
        }
    }
}