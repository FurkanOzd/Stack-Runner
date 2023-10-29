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
        private Material _finishBlockMaterial;

        private Transform _blockParentTransform;

        private Block _lastBlock;
        private Block _slidingBlock;

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
            Material finishBlockMaterial, Material[] blockMaterials, Block lastBlock)
        {
            _blockParentTransform = blockParentTransform;
            _finishBlockMaterial = finishBlockMaterial;
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
            SpawnNewSlidingBlock(false, _lastBlock.transform.localScale);
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
                _blockMaterials[_materialIndex], position, scale.x, _moveDuration, Block.BlockType.Solid);

            _slidingBlock = _blockFactory.Create(blockSpawnOptions);
        }

        private void SpawnBrokenBlock(Vector3 position, Vector3 scale)
        {
            BlockSpawnOptions blockSpawnOptions = new BlockSpawnOptions(_blockParentTransform,
                _blockMaterials[_materialIndex], position, scale.x, _moveDuration, Block.BlockType.Broken);
            
            _blockFactory.Create(blockSpawnOptions);
        }

        public Block StackBlock()
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
                _slidingBlock.Stack(_lastBlock);
                perfectFit = true;
            }
            
            _signalBus.Fire(new BlockFitSignal(perfectFit));

            _lastBlock = _slidingBlock;
            
            SpawnNewSlidingBlock(perfectFit, lastScale);
            
            return _lastBlock;
        }
    }
}