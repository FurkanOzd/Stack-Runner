using PathBlocksModule;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
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
    
    [Inject]
    private BlockController _blockController;
    
    private void Start()
    {
        _blockController.Initialize(_blockParent, _xSpawnOffset, _blockMoveDuration, _blockMaterials, _baseBlock);
    }

    private void Update()
    {
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