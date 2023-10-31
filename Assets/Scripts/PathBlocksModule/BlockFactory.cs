using System;
using System.Collections.Generic;
using PoolingModule;
using Zenject;

namespace PathBlocksModule
{
    public class BlockFactory : IFactory<object[], Block>
    {
        private readonly DiContainer _diContainer;

        private Dictionary<Block.BlockType, StackPool<Block>> _blockPoolDictionary;

        public BlockFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;

            _blockPoolDictionary = new Dictionary<Block.BlockType, StackPool<Block>>();

            Block.BlockType[] blockTypes  = (Block.BlockType[])(Enum.GetValues(typeof(Block.BlockType)));
            int blockTypeCount = Enum.GetNames(typeof(Block.BlockType)).Length;

            for (int index = 0; index < blockTypeCount; index++)
            {
                _blockPoolDictionary.Add(blockTypes[index], new StackPool<Block>(CreateInternal));
            }
        }

        private Block CreateInternal(object[] args = null)
        {
            BlockSpawnOptions blockSpawnOptions = (BlockSpawnOptions)args[0];
            
            Block block = _diContainer.InstantiatePrefabResourceForComponent<Block>(
                $"Prefabs/{blockSpawnOptions.BlockType}");
            
            block.Construct(blockSpawnOptions);
            return block;
        }

        public Block Create(object[] args = null)
        {
            BlockSpawnOptions blockSpawnOptions = (BlockSpawnOptions)args[0]; 
            
            Block block = _blockPoolDictionary[blockSpawnOptions.BlockType].Pull(args);
            
            block.Construct(blockSpawnOptions);
            return block;
        }
    }
}