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
            
            SpawnNewBlock(false, _lastBlock.transform.localScale);
        }
        
        private void SpawnNewBlock(bool perfectFit, Vector3 scale)
        {
            if (perfectFit)
            {
                _materialIndex++;
            }

            Vector3 position = new Vector3(_xSpawnOffset * _moveDirection, _lastBlock.transform.position.y,
                _lastBlock.transform.position.z + scale.z);

            _moveDirection *= -1;
            
            BlockSpawnOptions blockSpawnOptions = new BlockSpawnOptions(_blockParentTransform,
                _blockMaterials[_materialIndex], position, scale, _moveDuration, Block.BlockType.Solid);

            _slidingBlock = _blockFactory.Create(blockSpawnOptions);
        }

        public void FitBlock()
        {
            float difference = (_lastBlock.transform.position - _slidingBlock.transform.position).x;
            float absDifference = Mathf.Abs(difference);

            float brokeScaleFactor = (_slidingBlock.transform.localScale.x - absDifference);
            
            Debug.Log(absDifference);
            
            if (absDifference >= 0.05f)
            {
                _slidingBlock.Broke(brokeScaleFactor);
            }
            else
            {
                _slidingBlock.Stop();
            }
        }
    }
}