using Signals;
using UnityEngine;
using Zenject;

namespace PathBlocksModule
{
    public class BlockController
    {
        private readonly Block.Factory _blockFactory;

        private readonly SignalBus _signalBus;
        
        private Material[] _blockMaterials;
        private Transform _blockParentTransform;

        private Block _lastBlock;
        private SlidingBlock _slidingBlock;

        private FinishBlock _finishBlock;

        private int _moveDirection;
        private int _materialIndex;
        private int _materialArrayLength;

        private float _moveDuration;
        private float _xSpawnOffset;
        
        public BlockController(Block.Factory blockFactory, SignalBus signalBus)
        {
            _blockFactory = blockFactory;
            _signalBus = signalBus;
        }

        public void Initialize(Transform blockParentTransform, float xSpawnOffset, float moveDuration,
            Material[] blockMaterials, Block lastBlock)
        {
            _blockParentTransform = blockParentTransform;
            _blockMaterials = blockMaterials;
            _lastBlock = lastBlock;
            _moveDuration = moveDuration;
            _xSpawnOffset = xSpawnOffset;
            
            _moveDirection = Random.Range(-100, 100) >= 0 
                ? 1 
                : -1;

            _materialArrayLength = _blockMaterials.Length;
        }

        public void SetupLevel(int blockCountToComplete)
        {
            CreateFinishBlock(blockCountToComplete);
        }

        public void StartLevel()
        {
            SpawnNewSlidingBlock(false, _lastBlock.transform.localScale);
        }

        public float GetPathLength()
        {
            Vector3 position = _finishBlock.transform.position - _lastBlock.transform.position;
            return position.z;
        }

        private void SpawnNewSlidingBlock(bool perfectFit, Vector3 scale)
        {
            if (!perfectFit)
            {
                _materialIndex++;
                if (_materialIndex == _materialArrayLength)
                {
                    _materialIndex = 0;
                }
            }

            Vector3 position = new Vector3(_xSpawnOffset * _moveDirection, _lastBlock.transform.position.y,
                _lastBlock.transform.position.z + scale.z);

            _moveDirection *= -1;
            
            BlockSpawnOptions blockSpawnOptions = new BlockSpawnOptions(_blockParentTransform,
                _blockMaterials[_materialIndex], position, scale.x, _moveDuration, Block.BlockType.SlidingBlock);

            _slidingBlock = _blockFactory.Create(new object[]{blockSpawnOptions}) as SlidingBlock;
        }

        private void CreateFinishBlock(int blocksToSpawn)
        {
            float scaleOffset = _lastBlock.transform.localScale.z;
            
            Vector3 lastBlockPosition = _lastBlock.transform.position;
            Vector3 position = new Vector3(lastBlockPosition.x, lastBlockPosition.y,
                lastBlockPosition.z + scaleOffset + (blocksToSpawn * scaleOffset));

            BlockSpawnOptions blockSpawnOptions = new BlockSpawnOptions(_blockParentTransform,
                default, position, 0, _moveDuration, Block.BlockType.FinishBlock);
            
            _finishBlock = _blockFactory.Create(new object[]{blockSpawnOptions}) as FinishBlock;
        }

        private void SpawnBrokenBlock(Vector3 position, Vector3 scale)
        {
            BlockSpawnOptions blockSpawnOptions = new BlockSpawnOptions(_blockParentTransform,
                _blockMaterials[_materialIndex], position, scale.x, _moveDuration, Block.BlockType.BrokenBlock);
            
            _blockFactory.Create(new object[]{blockSpawnOptions});
        }

        public GameObject StackBlock(bool spawnNewBlock)
        {
            const float toleranceMultiplier = 0.05f;
            bool perfectFit = false;

            Vector3 lastScale = _slidingBlock.transform.localScale;
            
            float difference = (_lastBlock.transform.position - _slidingBlock.transform.position).x;
            float absDifference = Mathf.Abs(difference);
            float brokeScaleFactor = (lastScale.x - absDifference);
            float perfectFitToleranceValue = lastScale.x * toleranceMultiplier;
            
            lastScale.x = brokeScaleFactor;

            bool isUpper = difference < 0;
            
            if (absDifference >= perfectFitToleranceValue)
            {
                bool hasBroken = _slidingBlock.Broke(brokeScaleFactor, isUpper);

                if (!hasBroken)
                {
                    return null;
                }
                
                Vector3 brokenScale = _slidingBlock.transform.localScale;
                Vector3 position = _slidingBlock.transform.position;

                float xOffset = (brokenScale.x + absDifference) / 2;
                position.x += isUpper ? xOffset : -xOffset;
                
                brokenScale.x = absDifference;
                SpawnBrokenBlock(position, brokenScale);
            }
            else
            {
                _slidingBlock.Stack(_lastBlock.transform);
                perfectFit = true;
            }
            
            _signalBus.Fire(new BlockFitSignal(perfectFit));

            _lastBlock = _slidingBlock;
            
            if (spawnNewBlock)
            {
                SpawnNewSlidingBlock(perfectFit, lastScale);

            }
            return _lastBlock.gameObject;
        }

        public Vector3 GetStarterBlockPosition()
        {
            return _lastBlock.transform.position;
        }

        public Block GetFinishBlock()
        {
            _lastBlock = _finishBlock;
            return _lastBlock;
        }
    }
}