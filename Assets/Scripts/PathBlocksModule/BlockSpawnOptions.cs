using UnityEngine;

namespace PathBlocksModule
{
    public readonly struct BlockSpawnOptions
    {
        public readonly Vector3 Position;
        public readonly Vector3 Scale;
        
        public readonly Transform ParentTransform;

        public readonly Material Material;

        public readonly float MoveDuration;

        public readonly Block.BlockType BlockType;

        public BlockSpawnOptions(Transform parentTransform, Material material, Vector3 position, Vector3 scale,
            float moveDuration, Block.BlockType blockType)
        {
            ParentTransform = parentTransform;
            Material = material;
            Position = position;
            Scale = scale;
            MoveDuration = moveDuration;
            BlockType = blockType;
        }
    }
}