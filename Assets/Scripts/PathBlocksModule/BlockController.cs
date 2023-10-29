using UnityEngine;

namespace PathBlocksModule
{
    public class BlockController
    {
        private readonly Block.Factory _blockFactory;

        private Material[] _blockMaterials;

        private Transform _blockParentTransform;

        private Block _lastBlock;
        private Block _slidingBlock;

        private int _moveDirection;
        private int _materialIndex;

        private float _moveDuration;
        private float _xSpawnOffset;

        public BlockController(Block.Factory blockFactory)
        {
            _blockFactory = blockFactory;
        }
        
        public void Initialize(Transform blockParentTransform, float xSpawnOffset, float moveDuration, Material[] blockMaterials, Block lastBlock)
        {
            _blockParentTransform = blockParentTransform;
            _blockMaterials = blockMaterials;
            _lastBlock = lastBlock;
            _moveDuration = moveDuration;
            _xSpawnOffset = xSpawnOffset;
            
            _moveDirection = Random.Range(-100, 100) >= 0 
                ? 1 
                : -1;
            
            SpawnNewSlidingBlock(false, _lastBlock.transform.localScale);
        }
        
        private void SpawnNewSlidingBlock(bool perfectFit, Vector3 scale)
        {
            if (!perfectFit)
            {
                _materialIndex++;
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

        public Block FitBlock()
        {
            bool perfectFit = false;

            Vector3 lastScale = _slidingBlock.transform.localScale;
            
            float difference = (_lastBlock.transform.position - _slidingBlock.transform.position).x;
            float absDifference = Mathf.Abs(difference);
            float brokeScaleFactor = (lastScale.x - absDifference);

            lastScale.x = brokeScaleFactor;

            bool isUpper = difference < 0;
            
            if (absDifference >= 0.05f)
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
                _slidingBlock.Stop();
                perfectFit = true;
            }

            _lastBlock = _slidingBlock;
            
            SpawnNewSlidingBlock(perfectFit, lastScale);
            
            return _lastBlock;
        }
    }
}