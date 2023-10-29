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
        
        ListenEvents();
    }

    private void ListenEvents()
    {
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedSignal gameStateChangedSignal)
    {
        _gameState = gameStateChangedSignal.GameState;
    }

    public void OnUIButtonClicked()
    {
        switch (_gameState)
        {
            case GameState.ReadyToPlay:
                SetUpNewLevel();
                break;
            case GameState.Fail:
                RestartGame();
                break;
            case GameState.Success:
                _levelCount++;
                SetUpNewLevel();
                break;
        }
        
        _signalBus.Fire(new GameStateChangedSignal(_gameState));
    }

    private void SetUpNewLevel()
    {
        _gameState = GameState.Playing;
        _blockController.SetupLevel(_defaultBlockToFinishLevel + _levelCount * _blockCountIncreaserOnNewLevels);
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
        
        if (Input.GetMouseButtonDown(0))
        {
            Block blockToMove = _blockController.StackBlock();
            if (blockToMove != null)
            {
                _playerController.UpdateMoveDirection(blockToMove.transform.position.x);
            }
        }
    }
}