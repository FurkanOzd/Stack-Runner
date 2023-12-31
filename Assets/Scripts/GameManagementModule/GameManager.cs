using GameManagementModule;
using PathBlocksModule;
using Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject]
    private readonly BlockController _blockController;

    [Inject]
    private readonly SignalBus _signalBus;
    
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
    private int _defaultBlockToFinishLevel;
    [SerializeField]
    private int _blockCountIncreaserOnNewLevels;

    [SerializeField]
    private BoxCollider _failCheckerBox;

    private int _levelCount;
    private int _currentLevelBlockCounter = 0;
    private int _blocksToFinishLevel;

    private GameState _gameState;
    
    private void Start()
    {
        _blocksToFinishLevel = _defaultBlockToFinishLevel;
        
        _blockController.Initialize(_blockParent, _xSpawnOffset, _blockMoveDuration, _blockMaterials, _baseBlock);
        _gameState = GameState.ReadyToPlay;
        
        ListenEvents();
    }

    private void ListenEvents()
    {
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedSignal gameStateChangedSignal)
    {
        _gameState = gameStateChangedSignal.GameState;

        if (_gameState == GameState.ReachToFinalPlatform)
        {
            Block finishBlock = _blockController.GetFinishBlock();
            _playerController.MoveToFinishBlock(finishBlock);
        }
    }

    private void ArrangeFailChecker(Vector3 starterPosition)
    {
        float length = _blockController.GetPathLength();
        
        _failCheckerBox.size = new Vector3(100, 1, length * 2);
        _failCheckerBox.center = new Vector3(starterPosition.x, _failCheckerBox.center.y, starterPosition.z);
    }

    public void OnUIButtonClicked()
    {
        switch (_gameState)
        {
            case GameState.ReadyToPlay:
                StartLevel();
                break;
            case GameState.Fail:
                RestartGame();
                break;
            case GameState.Success:
                _currentLevelBlockCounter = 0;
                _levelCount++;
                _blocksToFinishLevel = _defaultBlockToFinishLevel + (_levelCount * _blockCountIncreaserOnNewLevels);
                StartLevel();
                break;
        }
        
        _signalBus.Fire(new GameStateChangedSignal(_gameState));
    }

    private void StartLevel()
    {
        _gameState = GameState.Playing;
        _blockController.StartLevel(_blocksToFinishLevel);
        ArrangeFailChecker(_blockController.GetStarterBlockPosition());
    }

    private void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void Update()
    {
        if (_gameState != GameState.Playing)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && _currentLevelBlockCounter < _blocksToFinishLevel)
        {
            _currentLevelBlockCounter++;
            
            bool isItLastBlock = _currentLevelBlockCounter >= _blocksToFinishLevel;

            GameObject blockToMove = _blockController.StackBlock(!isItLastBlock);
            if (blockToMove != null)
            {
                _playerController.UpdateMoveDirection(blockToMove.transform.position.x);
            }
        }
    }
}