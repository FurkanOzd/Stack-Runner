using UnityEngine;

namespace PathBlocksModule
{
    public readonly struct BlockSpawnOptions
    {
        public readonly Vector3 Position;
        
        public readonly Transform ParentTransform;

        public readonly Material Material;

        public readonly float MoveDuration;
        public readonly float ScaleFactorX;

        public readonly Block.BlockType BlockType;

        public BlockSpawnOptions(Transform parentTransform, Material material, Vector3 position, float scaleFactorX,
            float moveDuration, Block.BlockType blockType)
        {
            ParentTransform = parentTransform;
            Material = material;
            Position = position;
            ScaleFactorX = scaleFactorX;
            MoveDuration = moveDuration;
            BlockType = blockType;
        }
    }
}