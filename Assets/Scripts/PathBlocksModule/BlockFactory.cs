using PoolingModule;
using UnityEngine;
using Zenject;

namespace PathBlocksModule
{
    public class BlockFactory : IFactory<BlockSpawnOptions, Block>
    {
        private readonly DiContainer _diContainer;

        private StackPool<Block> _blockPool;

        public BlockFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;

            _blockPool = new StackPool<Block>(CreateInternal);
        }

        private Block CreateInternal(object[] args = null)
        {
            Block block = _diContainer.InstantiatePrefabResourceForComponent<Block>(
                $"Prefabs/{nameof(Block)}");
            
            block.Disable();

            return block;
        }

        public Block Create(BlockSpawnOptions blockSpawnOptions)
        {
            Block block = _blockPool.Pull();
            
            Transform blockTransform = block.transform;
            blockTransform.SetParent(blockSpawnOptions.ParentTransform);
            blockTransform.localScale = blockSpawnOptions.Scale;
            blockTransform.position = blockSpawnOptions.Position;
            
            block.Construct(blockSpawnOptions.MoveDuration, blockSpawnOptions.Material, blockSpawnOptions.BlockType);

            return block;
        }
    }
}